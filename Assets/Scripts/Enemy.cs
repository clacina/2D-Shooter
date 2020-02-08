#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _dropSpeed = 2.0f;
    [SerializeField]
    private int _killPoints = 10;
    private AudioSource _audioSource;

    private Player _player;
    private Animator _deathAnimation;

    [SerializeField]
    private GameObject _laserPrefabSingle;

    [SerializeField]
    private float _canFire = -1;
    private bool _isDead = false;
    private float _laserOffset = -0.9f;
    private float _nextFireMin = 3.0f, _nextFireMax = 5.0f;

    private float _deathSequenceLength = 2.5f;
    private System.DateTime _startTime;

    // --Old School
    // Start is called before the first frame update
    void Start()
    {
        _startTime = System.DateTime.Now;
        _canFire = Time.time + Random.Range(_nextFireMin, _nextFireMax);
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _deathAnimation = GetComponent<Animator>();
        Debug.Assert(_player == null || _audioSource == null || _deathAnimation == null,
            "Enemy Start Failed: " + _player + "," + _audioSource + "," + _deathAnimation);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (_canFire < Time.time && !_isDead)
        {
            GameObject enemyLaser = Instantiate(_laserPrefabSingle, transform.position + new Vector3(0, _laserOffset, 0), Quaternion.identity);
            enemyLaser.transform.parent = this.transform;

            Laser[] enemyLasers = enemyLaser.GetComponentsInChildren<Laser>();
            for(int i=0;i<enemyLasers.Length;i++)
            {
                enemyLasers[i].SetDirection(false);
            }
            _canFire = Time.time + Random.Range(_nextFireMin, _nextFireMax);
        }
    }

    void CalculateMovement()
    {
        // once started, move down
        transform.Translate(Vector3.down * _dropSpeed * Time.deltaTime);

        // if bottom of screen, respawn at top with a new random x pos.
        if (transform.position.y < -2.15f)
        {
            float new_x_pos = Random.Range(-8f, 10f);
            transform.position = new Vector3(new_x_pos, 8.0f, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Logger.Log(Channel.Enemy, "Hit: " + other.transform.name);
        //Logger.Log(Channel.Enemy, "Hit By: " + other.transform.tag);

        // Enemy hits enemy, do nothing
        if (other.transform.tag == "Enemy" || other.transform.name == "Laser_Left" || other.transform.name == "Laser_Right")
        {
            //Logger.Log(Channel.Enemy, "--Bail");
            return;
        }

        // if other is Player, damage the player, then destroy us
        if (other.transform.tag == "Player")
        {
            // Damage Player
            _player.Damage();
            EnemyDeath();
        }
        // if other is laser, destroy the laser, then destroy us
        else if(other.tag == "Laser")
        {
            _player.AddScore(_killPoints);

            Destroy(other.gameObject);
            EnemyDeath();
        }
    }

    void EnemyDeath()
    {
        // Destroy our collider so we can't be hit more or do more damage.
        Destroy(GetComponent<Collider2D>());

        _dropSpeed = 0f;  // Stop moving

        _isDead = true;   // Stop shooting

        _audioSource.Play();  // Start the audio

        _deathAnimation.SetTrigger("OnEnemyDeath"); // trigger the exposion animation

        // Wait some period of time before really going away - so we can see the animation
        Destroy(this.gameObject, _deathSequenceLength);
    } 
}
