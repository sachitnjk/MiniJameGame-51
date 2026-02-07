using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#region UpgradeInfo

public enum UpgradeType
{
    XPGain,
    SpawnRate,
    InstantXP,
    ClickMultiplier
}

public class Upgrade
{
    public UpgradeType type;
    public int tier;
    public int multiplier = 1;

    public Upgrade(UpgradeType type, int tier)
    {
        this.type = type;
        this.tier = tier;
    }
}

#endregion

public class GameManager : MonoBehaviour
{
    public float spawnRate = 0.5f;
    public float trashSpawnRate = 0.5f;
    [field: SerializeField] public float consumeRadius { get; private set; }

    [Header("XP related")]
    public float playerXP = 0f;
    public float xpGain = 0.1f;
    public float currentXPThreshold = 1f;
    public float maxThreshold = 100f;

    public int clickMultiplier = 1;

    private int thresholdsBanked = 0;

    private Dictionary<UpgradeType, Upgrade> activeUpgrades = new Dictionary<UpgradeType, Upgrade>();
    private List<Upgrade> upgradePool;

    [Header("Object Refs")]
    [field: SerializeField] public InputProvider inputProvider { get; private set; }
    [field: SerializeField] public FishSpawner FishSpawner { get; private set; }

    public Transform registeredMainSpeakerTransform { get; private set; }

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUpgradePool();
    }

    void InitializeUpgradePool()
    {
        upgradePool = new List<Upgrade>
        {
            new Upgrade(UpgradeType.XPGain, 1),

            new Upgrade(UpgradeType.SpawnRate, 2),
            new Upgrade(UpgradeType.InstantXP, 2),

            new Upgrade(UpgradeType.ClickMultiplier, 3)
        };

        Debug.Log("Upgrade Pool Initialized:");
        foreach (var u in upgradePool)
        {
            Debug.Log($"- Upgrade Type: {u.type}, Tier: {u.tier}");
        }
    }

    #region EXP
    public void RollForXP()
    {
        float roll = Random.value;

        if (roll <= spawnRate)
        {
            playerXP += xpGain;
            CheckThreshold();
            Debug.Log($"{xpGain} EXP Gained. Current Player EXP: {playerXP}");
        }
    }

    public void AddToTrash()
    {
        // WIP
    }

    void CheckThreshold()
    {
        while (playerXP >= currentXPThreshold)
        {
            thresholdsBanked++;
            playerXP -= currentXPThreshold;

            Debug.Log($"Threshold reached! Thresholds Banked: {thresholdsBanked}");

            if (currentXPThreshold < maxThreshold)
            {
                currentXPThreshold *= 2f;
                if (currentXPThreshold > maxThreshold)
                    currentXPThreshold = maxThreshold;
            }

            Debug.Log($"Next XP Threshold set to: {currentXPThreshold}");
        }
    }

    public void RegisterSpeakerTransform(Transform speakerTransform)
    {
        registeredMainSpeakerTransform = speakerTransform;
    }
    #endregion

    #region Upgrades
    public void UseUpgrade()
    {
        if (thresholdsBanked <= 0)
        {
            Debug.Log("UseUpgrade called but no thresholds banked.");
            return;
        }

        int tierToUse = thresholdsBanked;

        Debug.Log($"Using Upgrade at Tier {tierToUse}");

        var possible = upgradePool
            .Where(u => u.tier == tierToUse)
            .ToList();

        if (possible.Count == 0)
        {
            Debug.Log($"No upgrades available for Tier {tierToUse}");
            thresholdsBanked = 0;
            playerXP = 0f;
            return;
        }

        Upgrade rolled = possible[Random.Range(0, possible.Count)];

        Debug.Log($"Rolled Upgrade: {rolled.type} (Tier {rolled.tier})");

        ApplyOrStackUpgrade(rolled.type, rolled.tier);

        thresholdsBanked = 0;
        playerXP = 0f;

        Debug.Log("Upgrade consumed. Thresholds reset. Player XP reset.");
    }

    void ApplyOrStackUpgrade(UpgradeType type, int tier)
    {
        if (activeUpgrades.ContainsKey(type))
        {
            activeUpgrades[type].multiplier++;
            Debug.Log($"Stacking Upgrade: {type} now at Multiplier {activeUpgrades[type].multiplier}");
        }
        else
        {
            activeUpgrades[type] = new Upgrade(type, tier);
            Debug.Log($"New Upgrade Applied: {type} at Tier {tier}");
        }

        ApplyUpgradeEffect(activeUpgrades[type]);
    }

    void ApplyUpgradeEffect(Upgrade upgrade)
    {
        Debug.Log($"Applying Upgrade Effect for {upgrade.type} | Multiplier: {upgrade.multiplier}");

        switch (upgrade.type)
        {
            case UpgradeType.XPGain:
                float baseXP = currentXPThreshold;
                xpGain = baseXP * upgrade.multiplier;
                Debug.Log($"XP Gain recalculated. Base: {baseXP}, Multiplier: {upgrade.multiplier}, New xpGain: {xpGain}");
                break;

            case UpgradeType.SpawnRate:
                float increase = 0.1f * upgrade.multiplier;
                spawnRate += increase;
                Debug.Log($"Spawn Rate increased by {increase}. New spawnRate: {spawnRate}");
                break;

            case UpgradeType.InstantXP:
                float totalInstant = xpGain * upgrade.multiplier;
                playerXP += totalInstant;
                Debug.Log($"Instant XP triggered. Gained: {totalInstant}, New Player XP: {playerXP}");
                CheckThreshold();
                break;

            case UpgradeType.ClickMultiplier:
                clickMultiplier = 1 + upgrade.multiplier;
                Debug.Log($"Click Multiplier updated. New Click Multiplier: {clickMultiplier}");
                break;
        }
    }
    #endregion
}
