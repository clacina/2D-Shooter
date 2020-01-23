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
        if (other.tag == "Player" && _isPlayerLaser == false)
        {
            // Player takes damage
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
        }
    }
}
