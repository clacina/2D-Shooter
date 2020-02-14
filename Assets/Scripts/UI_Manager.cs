#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private int _score=0;

    [SerializeField]
    private Image _LifeImg;

    [SerializeField]
    private Sprite[] _lifesSprites;

    [SerializeField]
    private Text _gameOverText, _restartText, _shieldsText, _tripleText, _speedText, _gameStartText;

    [SerializeField]
    private TextMeshProUGUI _clockText;

    private GameManager _gameManager;
    private bool _gameOverFlash = true;


    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        Debug.Assert(_gameManager, "Game manager is null");


        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameStartText.gameObject.SetActive(true);

        _scoreText.gameObject.SetActive(false);
        _shieldsText.gameObject.SetActive(false);
        _tripleText.gameObject.SetActive(false);
        _speedText.gameObject.SetActive(false);
        _clockText.gameObject.SetActive(false);

        if (_lifesSprites.Length > 0)
        {
            UpdateLives(_lifesSprites.Length-1);
        }
    }

    private void Update()
    {
        if(!_clockText)
        {
            return;
        }
        _clockText.text = DateTime.Now.ToString("HH:mm:ss");
    }

    public void StartPlaying()
    {
        Logger.Log(Channel.AI, "UM - StartPlaying");

        // Hide startup text
        _gameStartText.gameObject.SetActive(false);

        // Format our UI elements
        _scoreText.text = "Score: 0";
        _shieldsText.text = "0 Shields";
        _tripleText.text = "";
        _speedText.text = "";
        _clockText.text = DateTime.Now.ToString("HH:mm:ss");

        // Enable our UI elements
        _clockText.gameObject.SetActive(true);
        _scoreText.gameObject.SetActive(true);
        _shieldsText.gameObject.SetActive(true);
        _tripleText.gameObject.SetActive(true);
        _speedText.gameObject.SetActive(true);
    }

    public void UpdateScore(int score)
    {
        _score = score;
        _scoreText.text = "Score: " + _score;
    }

    public void UpdateLives(int cur)
    {
        Logger.Log(Channel.UI, "Cur Lives is " + cur);
        if(cur > _lifesSprites.Length)
        {
            Logger.Log(Channel.UI, "Invalid index " + cur + " of length: " + _lifesSprites.Length);
            return;
        }
        _LifeImg.sprite = _lifesSprites[cur];

        if (cur == 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        _gameOverFlash = true;  // enable timed text display
        _restartText.gameObject.SetActive(true);  // Display menu prompt
        StartCoroutine(FlashText());  // start blink routine
        _gameManager.GameOver();
    }

    // Flash our Game Over text
    IEnumerator FlashText()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(_gameOverFlash);
            _gameOverFlash = !_gameOverFlash;
            yield return new WaitForSeconds(1.0f);
        }
    }

    internal void Shields(int shieldCount)
    {
        if(shieldCount > 0)
        {
            _shieldsText.gameObject.SetActive(true);
            _shieldsText.text = shieldCount + " shields";
                 
        } else
        {
            _shieldsText.gameObject.SetActive(false);
        }
    }

    internal void TripleShot(DateTime tripleShotExpiration)
    {
        System.TimeSpan a = tripleShotExpiration - System.DateTime.Now;
        if (a > System.TimeSpan.Zero) {
            _tripleText.gameObject.SetActive(true);
            _tripleText.text = "Triple Shot - " + a.ToString("%s");
        } else
        {
            _tripleText.gameObject.SetActive(false);
        }
    }

    internal void SpeedBoost(DateTime speedBoostExpiration)
    {
        System.TimeSpan a = speedBoostExpiration - System.DateTime.Now;
        if (a > System.TimeSpan.Zero)
        {
            _speedText.gameObject.SetActive(true);
            _speedText.text = "Speed Boost - " + a.ToString("%s");
        }
        else
        {
            _speedText.gameObject.SetActive(false);
        }
    }
}
