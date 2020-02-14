using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    // Only called when within fire threshold
    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_useTripleShot)
        {
            Instantiate(_laserPrefabTriple,
                transform.position + new Vector3(0, _firePositionOffset, 0),
                Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefabSingle,
                transform.position + new Vector3(0, _firePositionOffset, 0),
                Quaternion.identity);
        }
    }

    public void Damage()
    {
        if (_shieldCount > 1)
        {
            _shieldCount--;
            return;
        }
        else if (_shieldCount == 1)
        {
            // kill shield icon - no more shields
            _shieldVisualizer.SetActive(false);
            _shieldCount--;
            return;
        }

        // Adjust our lives and update the UI
        _lives--;
        UpdateLifeIcons();
    }

    void UpdateLifeIcons()
    {
        _uiManager.UpdateLives(_lives);

        // Show Damage depending on lives left
        if (_lives > 2)
        {
            _damageLeft.SetActive(false);
            _damageRight.SetActive(false);
        }
        if (_lives == 2)
        {
            _damageLeft.SetActive(false);
            _damageRight.SetActive(true);
        }
        if (_lives == 1)
        {
            _damageLeft.SetActive(true);
            _damageRight.SetActive(true);
        }

        if (_lives < 1)
        {
            // Tell spawn manager to stop spawning
            _spawnManager.PlayerKilled();

            _anim.SetTrigger("OnEnemyDeath"); // trigger the exposion animation

            // Destroy our object after animation runs
            Destroy(this.gameObject, _animatorSequenceLength);
        }
    }

    // Update Functions
    public void AddLife()
    {
        _lives++;
        UpdateLifeIcons();
    }

    public void EnableTripleShot()
    {
        if (!_useTripleShot)
        {
            _useTripleShot = true;
            _tripleShotExpiration = System.DateTime.Now;
        }

        _tripleShotExpiration = _tripleShotExpiration.Add(new System.TimeSpan(0, 0, 0, (int)_tripleshotBoostDuration));
        if (_tripleShotExpiration > System.DateTime.Now + System.TimeSpan.FromSeconds(_maxBoostDuration))
        {
            // Max out at 55 seconds
            _tripleShotExpiration = System.DateTime.Now + System.TimeSpan.FromSeconds(_maxBoostDuration);
        }

        _uiManager.TripleShot(_tripleShotExpiration);
    }

    public void SpeedUp()
    {
        if (!_speedMode)
        {
            _speedMode = true;
            _speedBoostExpiration = System.DateTime.Now;
        }

        _speedBoostExpiration = _speedBoostExpiration.Add(new System.TimeSpan(0, 0, 0, (int)_speedBoostDuration));
        if (_speedBoostExpiration > System.DateTime.Now + System.TimeSpan.FromSeconds(_maxBoostDuration))
        {
            // Max out at 55 seconds
            _speedBoostExpiration = System.DateTime.Now + System.TimeSpan.FromSeconds(_maxBoostDuration);
        }
        _uiManager.SpeedBoost(_speedBoostExpiration);
        _movementSpeed = _boostSpeed;
    }

    public void ShieldUp()
    {
        if (_shieldCount < _maxShields)  // max shields
        {
            _shieldCount++;
        }
        _shieldVisualizer.SetActive(true);
        _uiManager.Shields(_shieldCount);
    }

    public void AddScore(int iVal)
    {
        _score += iVal;
        _uiManager.UpdateScore(_score);
    }

    public void StartWave(int iVal)
    {
        _score += iVal;
        _uiManager.UpdateScore(_score);

        if (!_spawnManager.IsSpawning())
        {
            Logger.Log(Channel.Player, "Player::StartWave");
            _spawnManager.StartSpawning();
        }
    }
}
