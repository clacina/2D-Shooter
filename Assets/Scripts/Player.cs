#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 7f, _defaultMovementSpeed=7f, _boostSpeed=10f;

    [SerializeField]
    private GameObject _laserPrefabSingle, _laserPrefabTriple;

    [SerializeField]
    private float _defaultFireRate=0.05f, _fireBoostRate=0.16f;
    private float _fireRate;
    private float _canFire=0.0f;
    private float _firePositionOffset=0.8f;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private bool _useTripleShot=false, _speedMode=false;
    private float _speedBoostDuration=5f, _shieldDuration=28f, _tripleshotBoostDuration=15f;
    private System.DateTime _tripleShotExpiration, _speedBoostExpiration;

    [SerializeField]
    private int _shieldCount = 0;

    [SerializeField]
    private float _minXRange=-7.46f, _maxXRange=6.6f, _minYRange=-1.75f, _maxYRange=2f;
    
    [SerializeField]
    private GameObject _shieldVisualizer, _damageLeft, _damageRight;

    private UI_Manager _uiManager;

    private SpawnManager _spawnManager;

    [SerializeField]
    private int _score;

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
        Debug.Assert(_spawnManager);

        // Get access to the game manager
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UI_Manager>();
        Debug.Assert(_uiManager);
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
            Logger.Log(Channel.Laser, "Expiring Tripleshot " + System.DateTime.Now + " > " + _tripleShotExpiration);
            _useTripleShot = false;
        }

        if (_speedMode && System.DateTime.Now > _speedBoostExpiration)
        {
            Logger.Log(Channel.Laser, "Expiring Speed Bost " + System.DateTime.Now + " > " + _speedBoostExpiration);
            _speedMode = false;
            _movementSpeed = _defaultMovementSpeed;
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

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
        if (_lives == 2)
        {
            _damageLeft.SetActive(true);
        }
        if(_lives == 1)
        {
            _damageRight.SetActive(true);
        }

        if(_lives < 1)
        {
            // Tell spawn manager to stop spawning
            _spawnManager.PlayerKilled();

            // Destroy our object
            Destroy(this.gameObject);
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
        Logger.Log(Channel.Laser, "Extending triple shot to " + _tripleShotExpiration);
    }

    public void SpeedUp()
    {
        if (!_speedMode)
        {
            _speedMode = true;
            _speedBoostExpiration = System.DateTime.Now;
        }

        _speedBoostExpiration = _speedBoostExpiration.Add(new System.TimeSpan(0, 0, 0, (int)_speedBoostDuration));
        Logger.Log(Channel.Player, "Extending speed boost to " + _speedBoostExpiration);
        _movementSpeed = _boostSpeed;
    }

    public void ShieldUp()
    {
        _shieldCount++;
        Logger.Log(Channel.Player, "Shield Up!!");
        _shieldVisualizer.SetActive(true);
    }

    public void AddScore(int iVal)
    {
        Logger.Log(Channel.Player, "Taking score from " + _score + " to " + iVal);
        _score += iVal;
        _uiManager.UpdateScore(_score);
    }

    public void StartWave()
    {
        Logger.Log(Channel.Player, "Player::StartWave");
        _spawnManager.StartSpawning();
    }
}
