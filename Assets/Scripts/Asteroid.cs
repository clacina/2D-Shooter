﻿#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 7.0f;

    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    public bool isSpawned = true;

    private float _xMovementSpeed, _yMovementSpeed;

    private void Start()
    {
        // Define the movement speed for this object
        _xMovementSpeed = Random.Range(-1f, 1f);
        _yMovementSpeed = Random.Range(0f, -1f);
        Logger.Log(Channel.Asteroid, "Speed of " + _xMovementSpeed + " and " + _yMovementSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawned)
        {   // x (Left to Right), Y (Up / Down), ?
            transform.Translate(new Vector3(_xMovementSpeed, _yMovementSpeed, 0) * Time.deltaTime);
        }
        else
        {   // Rotation only
            transform.Rotate(new Vector3(0, 0, 2) * _rotationSpeed * Time.deltaTime);
        }
    }

    // Collision Routines
    private void OnTriggerEnter2D(Collider2D other)
    {
        Logger.Log(Channel.Asteroid, "Hit: " + other.transform.name + " - " + Time.time);

        // if other is Player, damage the player, then destroy us
        if (other.transform.tag == "Player")
        {
            Explode();

            // Damage Player
            Player player = other.transform.GetComponent<Player>();
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
            player.StartWave();
        }
    }

    //void OnTriggerStay2D(Collider2D collider)
    //{
    //    Logger.Log(Channel.Asteroid, "Trigger stay: " + Time.time);
    //}

    //void OnTriggerExit2D(Collider2D other)
    //{
    //    Logger.Log(Channel.Asteroid, "exit trigger: " + Time.time);
    //}

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Enemy")
    //    {
    //        Logger.Log(Channel.Asteroid, "Got Enter 2d " + Time.time);
    //    }
    //}

    // Internal Functions
    private void Explode()
    {
        // start the explosion animation
        Logger.Log(Channel.Asteroid, "Launch explosion " + Time.time);
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        // Destroy us (Asteroid)
        Logger.Log(Channel.Asteroid, "Destroy asteroid " + Time.time);
        Destroy(this.gameObject, .45f);

        // Destroy the animation in 2.5 seconds.
        Destroy(explosion, 2.5f);
        Logger.Log(Channel.Asteroid, "Leaving Explode " + Time.time);
    }

    public void Spawn()
    {
        isSpawned = true;
    }
}
