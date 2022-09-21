using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void OnEnable()
    {
        current = this;
    }

    public event Action<Transform> OnEnemySpawned;

    public void EnemySpawned(Transform enemy) =>
        //TODO: Better type than "Transform" somehow?
        OnEnemySpawned?.Invoke(enemy);

    public event Action<Transform> OnEnemyKilled;

    public void EnemyKilled(Transform enemy) => OnEnemyKilled?.Invoke(enemy);

    public event Action<Transform> OnPlayerMeleeAttacked;

    public void PlayerMeleeAttacked(Transform enemy) => OnPlayerMeleeAttacked?.Invoke(enemy);

    public event Action<Transform> OnPlayerRangedAttacked;

    public void PlayerRangedAttacked(Transform enemyCannonball) => OnPlayerRangedAttacked?.Invoke(enemyCannonball);


    public event Action<Transform> OnProjectileCollision;

    public void ProjectileCollision(Transform location) => OnProjectileCollision?.Invoke(location);

    public event Action OnPlayerCollidedWithSpike;

    public void PlayerCollidedWithSpike() => OnPlayerCollidedWithSpike?.Invoke();

    public event Action OnPlayerDamaged;

    public void PlayerDamaged() => OnPlayerDamaged?.Invoke();

    public event Action OnPlayerKilled;

    public void PlayerKilled() => OnPlayerKilled?.Invoke();

    public event Action OnUpdateUI;

    public void UpdateUI() =>
        //TODO Different UI Events?
        OnUpdateUI?.Invoke();

    public event Action<bool> OnTogglePause;

    public void TogglePause(bool paused) => OnTogglePause?.Invoke(paused);

    public event Action OnLevelComplete;

    public void LevelComplete() => OnLevelComplete?.Invoke();

    public event Action<bool> OnBuyRound;

    public void ToggleBuyRound(bool buying) => OnBuyRound?.Invoke(buying);

    public event Action<Transform> OnGoldCollected;

    public void GoldCollected(Transform pickupLocation) => OnGoldCollected?.Invoke(pickupLocation);

    public event Action<Transform> OnAmmoCollected;

    public void AmmoCollected(Transform pickupLocation) => OnAmmoCollected?.Invoke(pickupLocation);

    public event Action<Transform> OnShotFired;
    public void ShotFired(Transform shotLocation) => OnShotFired?.Invoke(shotLocation);

    public event Action<Transform> OnAmmoGone;
    public void AmmoGone(Transform spawnLocation) => OnAmmoGone?.Invoke(spawnLocation);

    public event Action OnRestartGame;

    public void RestartGame() => OnRestartGame?.Invoke();

    public event Action<string, List<int>> OnUpgrade;
    public void Upgrade(string upgradeName, List<int> upgradeList) => OnUpgrade?.Invoke(upgradeName, upgradeList);

    public event Action<int> OnInvestment;
    public void Investment(int investmentAmount) => OnInvestment?.Invoke(investmentAmount);

    public event Action<bool> OnTutorial;
    public void ToggleTutorial(bool tutorialBool) => OnTutorial?.Invoke(tutorialBool);

    public event Action<bool> OnCredits;
    public void ToggleCredits(bool tutorialBool) => OnCredits?.Invoke(tutorialBool);

    public event Action<int> OnVictory;
    public void Victory(int tier) => OnVictory?.Invoke(tier);

    public event Action OnLevelVictory;
    public void LevelVictory() => OnLevelVictory?.Invoke();

    public event Action<GameObject> OnPickup;
    public void Pickup(GameObject gameObject) => OnPickup?.Invoke(gameObject);

    public event Action<GameObject> OnExplosionDamage;
    public void ExplosionDamage(GameObject gameObj) => OnExplosionDamage?.Invoke(gameObj);
}