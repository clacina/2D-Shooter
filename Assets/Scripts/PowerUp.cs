#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generic class to handle all power up behaviors
public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _dropSpeed = 3.0f;

    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _clip;

    void Update()
    {
        // once started, move down screen at specified speed
        transform.Translate(Vector3.down * _dropSpeed * Time.deltaTime);

        // if bottom of screen, respawn at top with a new random x pos.
        if (transform.position.y < -2.15f)
        {
            Logger.Log(Channel.AI, "Destroying powerup due to position");
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            // Boost Player
            Player player = other.transform.GetComponent<Player>();

            // Play our audio clip
            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                switch(_powerupID)
                {
                    case 0: // Triple Shot
                        player.EnableTripleShot();
                        break;
                    case 1: // Speed
                        player.SpeedUp();
                        break;
                    case 2: // Shield
                        player.ShieldUp();
                        break;
                    default:
                        Debug.Log("Unknown Powerup: " + _powerupID);
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }
}
