#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AsteroidClass
{
    AC_LARGE,
    AC_MEDIUM,
    AC_SMALL
};

public class Asteroid : MonoBehaviour
{
    // Define scale of different asteroids
    public const float AC_LARGE_SIZE = 1.0f;
    public const float AC_MEDIUM_SIZE = .4F;
    public const float AC_SMALL_SIZE = .2f;

    [SerializeField]
    AsteroidClass _asteroidClass = AsteroidClass.AC_LARGE;

    [SerializeField]
    private float _rotationSpeed = 7.0f;

    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    public bool isSpawned = true;

    private float _xMovementSpeed, _yMovementSpeed;

    [SerializeField]
    private int _Value = 10;

    private void Start()
    {
        // Define the movement speed for this object
        _xMovementSpeed = Random.Range(-1f, 1f);
        _yMovementSpeed = Random.Range(0f, -1f);
        //Logger.Log(Channel.Asteroid, "Speed of " + _xMovementSpeed + " and " + _yMovementSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawned)
        {   // x (Left to Right), Y (Up / Down), ?
            transform.Translate(new Vector3(_xMovementSpeed, _yMovementSpeed, 0) * Time.deltaTime);
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, 2) * _rotationSpeed * Time.deltaTime);
        }

        // Check for out of field
        // if bottom of screen, respawn at top.
        if (transform.position.y < -2.15f || transform.position.x > 8.0f || transform.position.x < -9.0f)
        {
            //Logger.Log(Channel.Asteroid, "Passed bottom");
            Destroy(this.gameObject);
        }
    }

    // Collision Routines
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Logger.Log(Channel.Asteroid, "Hit: " + other.transform.name + " - " + Time.time);

        // if other is Player, damage the player, then destroy us
        if (other.transform.tag == "Player")
        {
            Explode();

            // Damage Player
            Player player = other.transform.GetComponent<Player>();
            Debug.Assert(player, "Asteroid can't find Player");
            if (player != null)
            {
                player.Damage();
            }
        }

        // if other is laser, destroy the laser, then destroy us
        else if (other.transform.tag == "Laser")
        {
            Explode();
            Logger.Log(Channel.Asteroid, "Calling start wave");
            Player player = GameObject.Find("Player").GetComponent<Player>();
            Debug.Assert(player);
            player.StartWave(_Value);
        }
    }

    // Internal Functions
    private void Explode()
    {
        // start the explosion animation
        //Logger.Log(Channel.Asteroid, "Launch explosion " + Time.time);
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        // Destroy us (Asteroid)
        //Logger.Log(Channel.Asteroid, "Destroy asteroid " + Time.time);
        Destroy(this.gameObject, .45f);

        // Destroy the animation in 2.5 seconds.
        Destroy(explosion, 2.5f);
        //Logger.Log(Channel.Asteroid, "Leaving Explode " + Time.time);
    }

    // External Functions
    public void Spawn()
    {
        isSpawned = true;
        _asteroidClass = (AsteroidClass) Random.Range(0, 3);   // use '3' because rand 'int' not float.
        //Logger.Log(Channel.Asteroid, "Spawning type " + _asteroidClass);
        if (_asteroidClass != AsteroidClass.AC_LARGE) {  // change scale
            float scale = AC_MEDIUM_SIZE;
            _Value = 20;
            if (_asteroidClass == AsteroidClass.AC_SMALL)
            {
                scale = AC_SMALL_SIZE;
                _Value = 40;                     
            }
            this.gameObject.transform.localScale = new Vector3(scale, scale, 0);
        }
    }
}
