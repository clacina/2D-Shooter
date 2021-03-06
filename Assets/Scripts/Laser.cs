﻿#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1;
    private float _laserRange = 8.0f;
    [SerializeField]
    private bool _isPlayerLaser = true;  // down

 
    // Update is called once per frame
    void Update()
    {
        // translate laser up
        if (_isPlayerLaser)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            if (transform.position.y > _laserRange)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                //Logger.Log(Channel.Laser, "Destroy Laser from position > laserRange");
                Destroy(this.gameObject);
            }
        } else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            if (transform.position.y < -2.0f)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                //Logger.Log(Channel.Laser, "Destroy Laser < 2.0fy");
                Destroy(this.gameObject);
            }
        }
    }

    public void SetDirection(bool dir)
    {
        _isPlayerLaser = dir;
        if (!_isPlayerLaser)
        {
            _speed = 5.0f;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        //Logger.Log(Channel.Laser, "Laser Hit " + other.tag);


        // is player
        //  if not ispalyerlaser
        //      Damage player
        //      Destroy laser

        // if not player laser - enemy on enemy
        //  do nothing

        // if enemy
        //  if not player laser
        //      Destroy laser
        //  else
        //      Do nothing - enemy hit by enemy

        // else - not player or enemy
        //  Destroy laser


        if (other.tag == "Player")
        {
            if (!_isPlayerLaser)
            {
                // Player takes damage
                Player player = other.transform.GetComponent<Player>();
                if (player != null)
                {
                    player.Damage();
                }
                //Logger.Log(Channel.Laser, "Destroy Laser(a) " + this.GetInstanceID());
                Destroy(gameObject);
            }
            // Else, player hit player so ignore
        }
        else if (other.tag == "Enemy")
        {
            if (!_isPlayerLaser)  // Enemy hits enemy, so do nothing 
            {
                // handled by Enemy::OnTriggerEnter2D
                // doesn't know about player laser vs enemy laser
                //Logger.Log(Channel.Laser, "Enemy hits Enemy!");
            }
        }
        else  // hit something besides Player or Enemy so destroy laser
        {
            //Logger.Log(Channel.Laser, "Destroy Laser(b) " + this.GetInstanceID());
            Destroy(gameObject);
        }
    }
}
