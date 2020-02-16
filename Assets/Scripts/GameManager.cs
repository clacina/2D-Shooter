#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*----------------------------------------------------------------------------
 * Keyboard Actions
 *
 *  'R' and game over: Restart Game
 *  'ESC'            : Exit Game
 *  'T' and game active : Triple Shot Enable
 *  'S' and game active : Shield Enable
 *  'P' and game active : Enable Speed
 *  'E' and game active : Add Enemy
 *  'A' and game active : Add Asteroid
 *  'L' and game active : Add Life
 * --------------------------------------------------------------------------*/

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver;
    private bool _isPaused=false, _isMuted=false;

    private Player _player;
    private SpawnManager _spawnManager;

    [SerializeField]
    private GameObject _helpText;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        Debug.Assert(_player, "GameManager cannot get handle to Player Object");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        Debug.Assert(_spawnManager, "GameManager cannot get handle to Spawn Manager");

        _helpText.gameObject.SetActive(false);
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.keyCode != KeyCode.None && e.type == EventType.KeyDown)
        {
            //Logger.Log(Channel.AI, "Detected key code: " + e.keyCode);

            switch (e.keyCode)
            {
                case KeyCode.T:         // Tiple Shot
                    _player.EnableTripleShot();
                    break;
                case KeyCode.S:         // Shield
                    _player.ShieldUp();
                    break;
                case KeyCode.P:         // Speed
                    _player.SpeedUp();
                    break;
                case KeyCode.E:         // Add Enemy
                    _spawnManager.AdhocSpawnEnemy();
                    break;
                case KeyCode.A:         // Add Asteroid
                    _spawnManager.AdhocSpawnAsteroid();
                    break;
                case KeyCode.L:         // Extra life
                    _player.AddLife();
                    break;
                case KeyCode.R:         // Restart
                    if (_isGameOver)
                    {
                        RestartGame();
                    }
                    break;
                case KeyCode.H:         // Help
                    if(!_isPaused)
                    {
                        Time.timeScale = 0;
                        _isPaused = true;
                        _helpText.gameObject.SetActive(true);
                    }
                    else
                    {
                        _helpText.gameObject.SetActive(false);
                        Time.timeScale = 1;
                        _isPaused = false;
                    }
                    break;
                case KeyCode.M:         // Mute
                    if(!_isMuted)
                    {
                        AudioListener.volume = 0f;
                        _isMuted = true;
                    } else
                    {
                        AudioListener.volume = 1f;
                        _isMuted = false;
                    }
                    break;
                case KeyCode.Escape:    // Exit
                                        // exit game
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    break;
                default:
                    break;
            }
        }
    }

    // Called from Player object
    public void GameOver()
    {
        _isGameOver = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
