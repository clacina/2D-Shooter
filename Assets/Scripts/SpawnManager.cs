#pragma warning disable 0649   // Object never assigned, this is because they are assigned in the inspector.  Always Null Check
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
    private GameObject _enemyContainer, _enemyPrefab, _asteroidPrefab, _asteroidContainer;

    private bool _stopSpawning = true;
    private float _spawnMinX = -6.6f, _spawnMaxX = 6.6f, _spawnY = 7.0f;

    public void PlayerKilled()
    {
        _stopSpawning = true;
    }

    Vector3 CalculateSpawnPosition()
    {
        // Figure out spawn position
        return new Vector3(Random.Range(_spawnMinX, _spawnMaxX), _spawnY, 0);
    }

    // Spawn Game objects every 5 seconds - Coroutines
    IEnumerator SpawnEnemy()
    {
        while (!_stopSpawning)
        {
            // Spawn Enemy 
            GameObject newEnemy = Instantiate(_enemyPrefab, CalculateSpawnPosition(), Quaternion.identity);
            // Set its container
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    IEnumerator SpawnPowerUp()
    {
        while (!_stopSpawning)
        {
            // Spawn Random Power Up
            int randomPowerUp = Random.Range(0, _powerups.Length);
            //Debug.Log("Spawning powerup of " + randomPowerUp);

            Instantiate(_powerups[randomPowerUp], CalculateSpawnPosition(), Quaternion.identity);

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

    // Ad Hoc Spawn Methods
    public void AdhocSpawnEnemy()
    {
        // Spawn Enemy 
        GameObject newEnemy = Instantiate(_enemyPrefab, CalculateSpawnPosition(), Quaternion.identity);
        // Set its container
        newEnemy.transform.parent = _enemyContainer.transform;
    }

    public void AdhocSpawnAsteroid()
    {
        Logger.Log(Channel.UI, "Spawn Asteroid");
        // Spawn Enemy 
        GameObject newAsteroid = Instantiate(_asteroidPrefab, CalculateSpawnPosition(), Quaternion.identity);

        // Set its _isSpawned attribute
        Asteroid asteroid = newAsteroid.GetComponent<Asteroid>();
        asteroid.Spawn();

        // Set its container
        newAsteroid.transform.parent = _asteroidContainer.transform;
    }
}
