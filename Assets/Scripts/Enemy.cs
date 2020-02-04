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
    private float _firePositionOffset = 8.0f;
    private float _canFire = -1;
    private bool _isDead = false;

    private float _deathSequenceLength = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        _canFire = Time.time + Random.Range(0.1f, 0.5f);   // fire .1 to .5 seconds after spawn
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _deathAnimation = GetComponent<Animator>();
        if (_player == null || _audioSource == null || _deathAnimation == null)
        {
            Debug.LogError("Cant find game element");
        }

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (_canFire < Time.time && _isDead == false)
        {
            GameObject enemyLaser = Instantiate(_laserPrefabSingle, transform.position + new Vector3(0, -.9f, 0), Quaternion.identity);
            enemyLaser.transform.parent = this.transform;

            Laser[] enemyLasers = enemyLaser.GetComponentsInChildren<Laser>();
            for(int i=0;i<enemyLasers.Length;i++)
            {
                enemyLasers[i].SetDirection(false);
            }
            _canFire = Time.time + Random.Range(3.0f, 5.0f);
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
        Debug.Log("Hit: " + other.transform.name);

        // if other is Player, damage the player, then destroy us
        if (other.transform.tag == "Player")
        {
            // Damage Player
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            EnemyDeath();
        }

        // if other is laser, destroy the laser, then destroy us
        else if(other.tag == "Laser")
        {
            if (_player)
            {
                _player.AddScore(_killPoints);
            }

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

        //this.gameObject.SetActive(false); // hide us?

        // Wait some period of time before really going away - so we can see the animation
        Destroy(this.gameObject, _deathSequenceLength);
    } 
}
