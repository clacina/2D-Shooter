#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Player movement speed and speed boost ramp up
    [SerializeField]
    private float _movementSpeed = 4f, _defaultMovementSpeed=4f, _boostSpeed=10f;

    // Player Damage Animations
    [SerializeField]
    private GameObject _damageLeft, _damageRight;

    // Lasers Prefabs
    [SerializeField]
    private GameObject _laserPrefabSingle, _laserPrefabTriple;
    private float _firePositionOffset = 0.8f;

    // Rate of Fire Control
    [SerializeField]
    private float _defaultFireRate=0.05f, _fireBoostRate=0.16f;
    private float _fireRate;
    private float _canFire=0.0f;

    // Lives and Score
    [SerializeField]
    private int _lives = 3, _score;

    // Shield Stuff
    [SerializeField]
    private int _shieldCount = 0;
    private const int _maxShields = 10;
    [SerializeField]
    private GameObject _shieldVisualizer;

    // Ramp Ups
    [SerializeField]
    private bool _useTripleShot=false, _speedMode=false;
    private float _speedBoostDuration=5f, _tripleshotBoostDuration=15f;
    private System.DateTime _tripleShotExpiration, _speedBoostExpiration;
    private const float _maxBoostDuration = 55f;

    // Screen Locations
    [SerializeField]
    private float _minXRange=-7.46f, _maxXRange=6.6f, _minYRange=-1.75f, _maxYRange=2f;

    // High Level Manager Objects
    private UI_Manager _uiManager;
    private SpawnManager _spawnManager;

    // Left / Right movement animation
    [SerializeField]
    private GameObject _swingLeftAnimation, _swingRightAnimation;
    private Animator _anim;
    private int _lastMovement=0;  // -1 (left), 0, 1 (right)
    private Vector3 _lastPosition;
    private bool _isAnimLeft=false, _isAnimRight = false;


    private Animator _deathAnimation;
    private float _deathSequenceLength = 2.5f;


    void Start()
    {
        _fireRate = _defaultFireRate;

        // Turn off all effects
        _shieldVisualizer.SetActive(false);
        _damageLeft.SetActive(false);
        _damageRight.SetActive(false);

        // Position our ship in the middle of the screen
        transform.position = new Vector3(0, 0, 0);

        // Get access to the spawn manager 
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        Debug.Assert(_spawnManager, "Player cant find Spawn Manager");

        // Get access to the game manager
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UI_Manager>();
        Debug.Assert(_uiManager, "Player cant find UI Manager");

        _anim = gameObject.GetComponent<Animator>();
        Debug.Assert(_anim, "Player cant find animation manaager");

        _deathAnimation = GetComponent<Animator>();

        _lastPosition = transform.position;
        _lastMovement = 0;
    }

    void Update()
    {
        CalculateMovement();

        // If we hit the space key, spawn laser
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

        // check for triple shot time out
        if (_useTripleShot && System.DateTime.Now > _tripleShotExpiration)
        {
            //Logger.Log(Channel.Laser, "Expiring Tripleshot " + System.DateTime.Now + " > " + _tripleShotExpiration);
            _useTripleShot = false;
        }

        // check for speed boost time out
        if (_speedMode && System.DateTime.Now > _speedBoostExpiration)
        {
            //Logger.Log(Channel.UI, "Expiring Speed Bost " + System.DateTime.Now + " > " + _speedBoostExpiration);
            _speedMode = false;
            _movementSpeed = _defaultMovementSpeed;
        }

        // Update UI Elements
        _uiManager.TripleShot(_tripleShotExpiration);
        _uiManager.SpeedBoost(_speedBoostExpiration);
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        if(horizontalInput == 0.0f && verticalInput == 0.0f)
        {
            // Stop any animation
            if(_isAnimLeft)
            {

            } else if(_isAnimRight)
            {

            }
            return;
        }

        //Logger.Log(Channel.AI, "Horizontal input: " + horizontalInput);

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        // Move Us as described by the Axis Inputs
        transform.Translate(direction * _movementSpeed * Time.deltaTime);

        // Restrict / clamp the Y axis - can only go so high on the screen
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _minYRange, _maxYRange), 0);

        // Wrap from side to side if we go out of bounds
        if (transform.position.x > _maxXRange)
        {
            transform.position = new Vector3(_minXRange, transform.position.y, 0);
        }
        else if (transform.position.x < _minXRange)
        {
            transform.position = new Vector3(_maxXRange, transform.position.y, 0);
        }

        // where did we move?

        _lastPosition = transform.position;
        if (horizontalInput < 0.0f)
        {
            // if left animation not running, start
            Logger.Log(Channel.AI, "Moving Left");
            // Stop any animation
            if (_isAnimLeft)
            {

            }
            else if (_isAnimRight)
            {

            }
            //_deathAnimation.SetTrigger("OnEnemyDeath"); // trigger the exposion animation

        }
        else if (horizontalInput > 0.0f)
        {
            // if right animation not running, start
            Logger.Log(Channel.AI, "Moving Right");
            // Stop any animation
            if (_isAnimLeft)
            {

            }
            else if (_isAnimRight)
            {

            }
            //_deathAnimation.SetTrigger("OnEnemyDeath"); // trigger the exposion animation
        }
    }

    // Only called when within fire threshold
    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_useTripleShot)
        {
            Instantiate(_laserPrefabTriple,
                transform.position + new Vector3(0, _firePositionOffset, 0),
                Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefabSingle,
                transform.position + new Vector3(0, _firePositionOffset, 0),
                Quaternion.identity);
        }
    }

    public void Damage()
    {
        if (_shieldCount > 1)
        {
            _shieldCount--;
            return;
        } else if(_shieldCount == 1)
        {
            // kill shield icon - no more shields
            _shieldVisualizer.SetActive(false);
            _shieldCount--;
            return;
        }

        // Adjust our lives and update the UI
        _lives--;
        UpdateLifeIcons();
    }

    void UpdateLifeIcons() {
        _uiManager.UpdateLives(_lives);

        // Show Damage depending on lives left
        if (_lives > 2)
        {
            _damageLeft.SetActive(false);
            _damageRight.SetActive(false);
        }
        if (_lives == 2)
        {
            _damageLeft.SetActive(false);
            _damageRight.SetActive(true);
        }
        if(_lives == 1)
        {
            _damageLeft.SetActive(true);
            _damageRight.SetActive(true);
        }

        if(_lives < 1)
        {
            // Tell spawn manager to stop spawning
            _spawnManager.PlayerKilled();

            _deathAnimation.SetTrigger("OnEnemyDeath"); // trigger the exposion animation

            // Destroy our object after animation runs
            Destroy(this.gameObject, _deathSequenceLength);
        }
    }

    // Update Functions
    public void AddLife()
    {
        _lives++;
        UpdateLifeIcons();
    }

    public void EnableTripleShot()
    {
        if (!_useTripleShot)
        {
            _useTripleShot = true;
            _tripleShotExpiration = System.DateTime.Now;
        }

        _tripleShotExpiration = _tripleShotExpiration.Add(new System.TimeSpan(0, 0, 0, (int)_tripleshotBoostDuration));
        if (_tripleShotExpiration > System.DateTime.Now + System.TimeSpan.FromSeconds(_maxBoostDuration))
        {
            // Max out at 55 seconds
            _tripleShotExpiration = System.DateTime.Now + System.TimeSpan.FromSeconds(_maxBoostDuration);
        }
        
        _uiManager.TripleShot(_tripleShotExpiration);
        //Logger.Log(Channel.Laser, "Extending triple shot to " + _tripleShotExpiration);
    }

    public void SpeedUp()
    {
        if (!_speedMode)
        {
            _speedMode = true;
            _speedBoostExpiration = System.DateTime.Now;
        }

        _speedBoostExpiration = _speedBoostExpiration.Add(new System.TimeSpan(0, 0, 0, (int)_speedBoostDuration));
        if(_speedBoostExpiration > System.DateTime.Now+System.TimeSpan.FromSeconds(_maxBoostDuration))
        {
            // Max out at 55 seconds
            _speedBoostExpiration = System.DateTime.Now + System.TimeSpan.FromSeconds(_maxBoostDuration);
        }
        _uiManager.SpeedBoost(_speedBoostExpiration);
        //Logger.Log(Channel.Player, "Extending speed boost to " + _speedBoostExpiration);
        _movementSpeed = _boostSpeed;
    }

    public void ShieldUp()
    {
        if (_shieldCount < _maxShields)  // max shields
        {
            _shieldCount++;
        }
        //Logger.Log(Channel.Player, "Shield Up!!");
        _shieldVisualizer.SetActive(true);
        _uiManager.Shields(_shieldCount);
    }

    public void AddScore(int iVal)
    {
        //Logger.Log(Channel.Player, "Taking score from " + _score + " to " + iVal);
        _score += iVal;
        _uiManager.UpdateScore(_score);
    }

    public void StartWave(int iVal)
    {
        _score += iVal;
        _uiManager.UpdateScore(_score);

        if (!_spawnManager.IsSpawning())
        {
            Logger.Log(Channel.Player, "Player::StartWave");
            _spawnManager.StartSpawning();
        }
    }
}
