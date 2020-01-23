using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private Text _gameOverText, _restartText;
    private bool _gameOverFlash;
    private GameManager _gameManager;


    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if(_gameManager == null)
        {
            Debug.Log("Game manager is null");
            Debug.Assert(_gameManager);
        }

        _scoreText.text = "Score: 0";
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        Debug.Log("Life Sprite: " + _lifesSprites.Length);
        if(_lifesSprites.Length > 0)
        {
            UpdateLives(_lifesSprites.Length-1);
        }
    }

    public void UpdateScore(int score)
    {
        _score = score;
        _scoreText.text = "Score: " + _score;
    }

    public void UpdateLives(int cur)
    {
        Debug.Log("Cur Lives is " + cur);
        if(cur > _lifesSprites.Length)
        {
            Debug.LogError("Invalid index " + cur + " of length: " + _lifesSprites.Length);
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

}
