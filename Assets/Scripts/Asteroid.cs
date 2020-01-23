using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 7.0f;

    [SerializeField]
    private GameObject _explosionPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 2) *_rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit: " + other.transform.name);

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
            Debug.Log("Calling start wave");
            Player player = GameObject.Find("Player").GetComponent<Player>();
            if (player != null)
            {
                player.StartWave();
            } else
            {
                Debug.Log("--Player is null");
            }
        }
    }

    private void Explode()
    {
        // start the explosion animation
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        // Destroy us (Asteroid)
        Destroy(this.gameObject, .45f);

        // Destroy the animation in 2.5 seconds.
        Destroy(explosion, 2.5f);
    }
}
