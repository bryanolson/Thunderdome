using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public static GameState current;

    private HashSet<GameObject> _livingEnemies;
    private GameEvents _ge;

    public int EnemiesKilled { get; private set; }
    public int LevelEnemies { get; private set; } = 1;
    public int MaxConcurrentEnemies { get; private set; } = 5;
    public int CurrentLevel { get; private set; } = 1;
    public bool CanSpawnEnemy { get; private set; } = true;
    public double SpawnMultiplier { get; private set; } = 1;
    public bool Paused { get; private set; } = true;
    public bool PlayerAlive { get; private set; }
    public bool BuyingBool { get; private set; } = false;
    public int CurrentGold { get; private set; } = 250;

    public bool LevelEnding { get; private set; } = false;
    public bool Invincible { get; private set; } = false;
    public float InvincibleTime { get; set; } = 1.0f;

    public float SpikeDamage { get; set; } = 5.0f;

    private int maxAmmo = 200;
    public float PlayerAttackPower { get; private set; } = 40;
    public float PlayerShotSpeed { get; private set; } = 50;

    public AudioSource goldSource;
    public AudioSource ammoSource;
    public AudioSource cannonballSource;

    public AudioSource[] CollisionAudioSources;

    //TODO: Power-up?
    private readonly int _ammoPickupAmount;
    private readonly int _goldPickupAmount = 25;

    public int CurrentAmmo { get; private set; }
    public float CurrentHealth { get; private set; } = 100;
    public float CurrentArmor { get; private set; } = 20;


    public GameState()
    {
        _ammoPickupAmount = maxAmmo / 10;
        CurrentAmmo = maxAmmo / 10;
    }

    private void OnEnable()
    {
        current = this;
        _livingEnemies = new HashSet<GameObject>();
        _ge = GameEvents.current;
        _ge.OnRestartGame += RestartGame;
        _ge.OnGoldCollected += GoldPickedUp;
        _ge.OnAmmoCollected += AmmoPickedUp;
        _ge.OnShotFired += ShotFired;
        _ge.OnEnemyKilled += EnemyKilled;
        _ge.OnEnemySpawned += EnemySpawned;
        _ge.OnTogglePause += HandleTogglePause;
        _ge.OnPlayerKilled += PlayerKilled;
        _ge.OnPlayerMeleeAttacked += PlayerMeleeAttacked;
        _ge.OnPlayerRangedAttacked += PlayerRangedAttacked;
        _ge.OnPlayerCollidedWithSpike += PlayerCollidedWithSpike;
        _ge.OnProjectileCollision += PlayProjectileSound;
        _ge.OnUpgrade += PlayerUpgrade;
        _ge.OnInvestment += PlayerInvestment;
        _ge.TogglePause(true);
        _ge.UpdateUI();
        _ge.OnBuyRound += HandleBuyRound;
        _ge.ToggleBuyRound(BuyingBool);
        _ge.OnExplosionDamage += ExplosionDamage;
        InitializePlayer();
    }

    private void PlayProjectileSound(Transform location)
    {
        var tempAudio = Instantiate(cannonballSource.gameObject, location.position, Quaternion.identity);
        tempAudio.GetComponent<AudioSource>().Play();
        StartCoroutine(DestroyLater(tempAudio));
    }

    private IEnumerator DestroyLater(GameObject tempAudio)
    {
        yield return new WaitForSeconds(1);
        Destroy(tempAudio);
    }


    private void InitializePlayer()
    {
        //Not init player in PlayerKilled because we may allow multiple lives or something. Just trigger on restart.
        EnemiesKilled = 0;
        PlayerAlive = true;
        CurrentLevel = 1;
        SetupEnemiesForLevel();
        CurrentGold = 250;
        CurrentAmmo = 50;
        CurrentHealth = 100;
        CurrentArmor = 20;
        Invincible = false;
    }

    private void SetupEnemiesForLevel()
    {
        CanSpawnEnemy = true;
        EnemiesKilled = 0;

        //TODO LOL Difficulty curve.
        LevelEnemies = (int) (Math.Pow(2, CurrentLevel) - 1);
        SpawnMultiplier = Math.Pow(1.5, CurrentLevel);
        MaxConcurrentEnemies = CurrentLevel + 1;
        // End difficulty section 

        //This should already be the case, but maybe we have some scenarios where you don't kill *all* the enemies?
        foreach (var livingEnemy in _livingEnemies)
        {
            Destroy(livingEnemy);
        }

        _livingEnemies.Clear();
    }

    private void PlayerInvestment(int amountInvested)
    {
        double floatGold = amountInvested * 1.1;
        CurrentGold = (int) floatGold;
    }

    private void PlayerUpgrade(string upgradeName, List<int> upgradeList)
    {
        // List format is { upgradeLevel, upgradeBonus, upgradeCost }
        CurrentGold -= upgradeList[2];

        if (upgradeName == "armor")
        {
            CurrentArmor += upgradeList[1];
        }

        if (upgradeName == "damage")
        {
            PlayerAttackPower += upgradeList[1];
        }

        if (upgradeName == "attackspeed")
        {
            PlayerShotSpeed += upgradeList[1];
        }
    }

    private void PlayerMeleeAttacked(Transform enemy)
    {
        float enemyMeleeAttackPower;
        var enemyState = enemy.gameObject.GetComponent<EnemyMovement>();
        if (enemyState != null)
            enemyMeleeAttackPower = enemyState.meleeAttackPower;
        else
            return;

        PlayerDamaged(enemyMeleeAttackPower);
    }

    private void PlayerCollidedWithSpike()
    {
        PlayerDamaged(SpikeDamage);
    }

    private void PlayerDamaged(float damage)
    {
        if (Invincible || LevelEnding)
            return;

        _ge.PlayerDamaged();
        var audioSourceIndex = UnityEngine.Random.Range(0, CollisionAudioSources.Length);
        var collisionSource = CollisionAudioSources[audioSourceIndex];
        collisionSource.PlayOneShot(collisionSource.clip);

        while (damage > 0)
        {
            if (CurrentArmor > 0)
            {
                CurrentArmor -= 1;
            }
            else
            {
                CurrentHealth -= 1;
            }

            damage -= 1;
        }

        Invincible = true;
        StartCoroutine(InvincibleTimerRoutine());

        if (CurrentHealth <= 0)
            _ge.PlayerKilled();
    }

    private void PlayerRangedAttacked(Transform enemyCannonball)
    {
        PlayerDamaged(1);
    }


    private void PlayerKilled()
    {
        PlayerAlive = false;
    }

    private void EnemySpawned(Transform enemy)
    {
        // TODO: Do something neat to let the player know there's a new enemy. Some kind of ping?
        _livingEnemies.Add(enemy.gameObject);
        CalcCanSpawnEnemy();
    }

    private void EnemyKilled(Transform enemy)
    {
        var enemyGameObject = enemy.gameObject;
        if (LevelEnding || !_livingEnemies.Remove(enemyGameObject))
            return;

        if (++EnemiesKilled >= LevelEnemies)
        {
            LevelEnding = true;
            StartCoroutine(nameof(LevelComplete));
        }

        _ge.UpdateUI();
        CalcCanSpawnEnemy();
    }

    private IEnumerator LevelComplete()
    {
        // Handle LevelVictory events before LevelComplete events
        _ge.LevelVictory();
        yield return new WaitForSeconds(3);


        // Handling this as a coroutine so that any other enemy killed handling can be processed first before triggering
        // the level completion. Needed for auto-pickup of items.
        yield return null;
        // New GameEvent for LevelComplete
        // TODO: Consider moving all logic below to function handled by LevelComplete Event
        _ge.LevelComplete();

        CurrentLevel++;
        SetupEnemiesForLevel();

        LevelEnding = false;
    }

    private void HandleBuyRound(bool buying)
    {
        BuyingBool = buying;
        // Have to recalc boolean based on buying phase
        CalcCanSpawnEnemy();
    }

    private void CalcCanSpawnEnemy()
    {
        // This way we don't calc every frame. Probably over-optimizing, but :shrug. Trying to build habits/thoughts.
        CanSpawnEnemy = (_livingEnemies.Count < MaxConcurrentEnemies &&
                         EnemiesKilled + _livingEnemies.Count < LevelEnemies && !BuyingBool);
    }

    private void HandleTogglePause(bool paused)
    {
        Paused = paused;
        if (Paused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            PlayerAlive = true;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // TODO: Maybe move these to player? I'm not 100% sure/confident in where we'd want to track that yet.
    // Maybe TBD with health?
    private void GoldPickedUp(Transform goldTransform)
    {
        goldSource.PlayOneShot(goldSource.clip);
        CurrentGold += _goldPickupAmount;
        _ge.UpdateUI();
    }

    private void AmmoPickedUp(Transform ammoTransform)
    {
        ammoSource.PlayOneShot(ammoSource.clip);
        CurrentAmmo = Math.Min(CurrentAmmo + _ammoPickupAmount, maxAmmo);
        _ge.UpdateUI();
    }

    private void ShotFired(Transform shotTransform)
    {
        CurrentAmmo -= 1;
        if (CurrentAmmo == 0)
        {
            // This should keep the player from being totally stuck.
            // Related code in LootSpawner if there is nothing left that's spawned and player has zero ammo.
            GameEvents.current.AmmoGone(Spawner.GetRandomSpawnOption());
        }

        _ge.UpdateUI();
    }

    private void RestartGame()
    {
        InitializePlayer();
        SceneManager.LoadScene("Scenes/DriveACube");
    }

    private IEnumerator InvincibleTimerRoutine()
    {
        yield return new WaitForSeconds(InvincibleTime);
        Invincible = false;
    }

    private void ExplosionDamage(GameObject gameObj)
    {
        if (gameObj.CompareTag("Player"))
        {  
            if (CurrentArmor > 0)
            {
                PlayerDamaged(10);
            }
            else
            {
                PlayerDamaged(25);
            }
        }
    }
}