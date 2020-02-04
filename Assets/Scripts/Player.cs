#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 7f, _defaultSpeed=7f, _boostSpeed=10f;

    [SerializeField]
    private GameObject _laserPrefabSingle, _laserPrefabTriple;

    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = 0.0f;
    private float _firePositionOffset = 0.8f;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private bool _useTripleShot=false, _useSpeedBoost=false, _useShield=false;
    private float _speedBoostDuration=5f, _shieldDuration=28f, _tripleshotBoostDuration=5f;

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
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        // Move Us as described by the Axis Inputs
        transform.Translate(direction * _speed * Time.deltaTime);

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
        // Do we have a shield? Just kill it and continue
        if (_useShield)
        {
            _useShield = false;
            _shieldVisualizer.SetActive(false);
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
        _useTripleShot = true;
        StartCoroutine(PowerCoolDown());
    }

    public void SpeedUp()
    {
        _useSpeedBoost = true;
        _speed = _boostSpeed;
        StartCoroutine(SpeedCoolDown());
    }

    public void ShieldUp()
    {
        Logger.Log(Channel.Player, "Shield Up!!");
        _useShield = true;
        _shieldVisualizer.SetActive(true);
        StartCoroutine(ShieldCoolDown());
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


    //----------------------------------------------------------------------
    // Cool Down Routines - turn feature off after 'x' seconds
    IEnumerator PowerCoolDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(_tripleshotBoostDuration);
            _useTripleShot = false;
        }
    }

    IEnumerator SpeedCoolDown()
    {
        while(true)
        {
            yield return new WaitForSeconds(_speedBoostDuration);
            _useSpeedBoost = false;
            _speed = _defaultSpeed;
        }
    }

    IEnumerator ShieldCoolDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(_shieldDuration);
            _useShield = false;
        }
    }
}
