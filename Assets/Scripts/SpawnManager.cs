using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private float _enemySpawnRate = 5.0f;
    [SerializeField]
    private GameObject _enemyContainer, _enemyPrefab;

    private bool _stopSpawning = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void PlayerKilled()
    {
        _stopSpawning = true;
    }

    // Spawn Game objects every 5 seconds - Coroutines
    IEnumerator SpawnEnemy()
    {
        while (!_stopSpawning)
        {
            // Figure out spawn position
            Vector3 startPos = new Vector3(Random.Range(-6f, 6f), 7.0f, 0);
            // Spawn Enemy 
            GameObject newEnemy = Instantiate(_enemyPrefab, startPos, Quaternion.identity);
            // Set its container
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    IEnumerator SpawnPowerUp()
    {
        while (!_stopSpawning)
        {
            Vector3 startPos = new Vector3(Random.Range(-6.6f, 6.6f), 7.0f, 0);

            // Spawn Random Power Up
            int randomPowerUp = Random.Range(0, _powerups.Length);
            //Debug.Log("Spawning powerup of " + randomPowerUp);

            Instantiate(_powerups[randomPowerUp], startPos, Quaternion.identity);

            float spawnFreq = Random.Range(3f, 7f);
            //Debug.Log("Next spawn in " + spawnFreq + " seconds");

            yield return new WaitForSeconds(spawnFreq);
        }
    }

    public void StartSpawning()
    {
        Debug.Log("SpawnManager::StartWave " + _stopSpawning);
        if (_stopSpawning)
        {
            _stopSpawning = false;
            StartCoroutine(SpawnEnemy());
            StartCoroutine(SpawnPowerUp());
        }
    }
}
