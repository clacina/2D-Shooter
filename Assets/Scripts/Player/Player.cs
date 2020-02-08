#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
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
    private float _lastHorizontalPostion = 0f;

    private float _animatorSequenceLength = 2.5f;


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
            if(_isAnimLeft || _isAnimRight)
            {
                _anim.StopPlayback();
            }
            _lastHorizontalPostion = horizontalInput;
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
        UpdateMovementAnimation(horizontalInput);
    }
    
    private void UpdateMovementAnimation(float hInput)
    {
        if (hInput < 0.0f)
        {
            // if left animation not running, start
            Logger.Log(Channel.AI, "Moving Left");
            // Stop any animation
            if (_isAnimLeft)
            {
                // do nothing
            }
            else if (_isAnimRight)
            {
                // Stop Right anim, start left anim
                _anim.StopPlayback();
                _anim.SetTrigger("Player_Turn_Left");
            }
            //_deathAnimation.SetTrigger("OnEnemyDeath"); // trigger the exposion animation

        }
        else if (hInput > 0.0f)
        {
            // if right animation not running, start
            Logger.Log(Channel.AI, "Moving Right");
            // Stop any animation
            if (_isAnimLeft)
            {
                // stop left anim
                // start right anim
                // Stop Right anim, start left anim
                _anim.StopPlayback();
                _anim.SetTrigger("Player_Turn_Right");
            }
            else if (_isAnimRight)
            {
                // do nothing
            }
            //_deathAnimation.SetTrigger("OnEnemyDeath"); // trigger the exposion animation
        }
        _lastHorizontalPostion = hInput;

    }
}
