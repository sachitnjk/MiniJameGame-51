using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

#region UpgradeInfo

public enum UpgradeType
{
    // Tier 1
    BetterFish,
    LessTrash,
    FasterSpeaker,

    // Tier 2
    PremiumFish,
    CleanerWaters,
    CastNet,
    BiggerNet,

    // Tier 3
    MinionSpeaker,
    MinionFishQuality
}

[System.Serializable]
public class UpgradeDefinition
{
    public UpgradeType type;
    public int tier;
    public string displayName;
    public string description;

    public UpgradeDefinition(UpgradeType type, int tier, string displayName, string description)
    {
        this.type = type;
        this.tier = tier;
        this.displayName = displayName;
        this.description = description;
    }
}

#endregion

public class GameManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int clicksRequiredForSpawn = 6;
    private int currentClickCount = 0;
    public float fishWeight = 0.3f;

    [field: SerializeField] public float consumeRadius { get; private set; }

    [Header("XP Settings")]
    public float playerXP = 0f;
    public float xpPerFish = 1f;

    [Header("Tier Thresholds")]
    private int currentTier = 1;
    private float[] tierThresholds = { 0f, 10f, 30f, 60f };

    [Header("Minion Speakers")]
    private int minionCount = 0;
    private const int MAX_MINIONS = 3;
    public float minionXPBonus = 0f;
    [SerializeField] private GameObject minionSpeakerPrefab;

    [Header("Net Upgrades")]
    private int netMultiplier = 5;
    [Header("Upgrade System")]
    private List<UpgradeDefinition> allUpgrades = new List<UpgradeDefinition>();
    private List<UpgradeDefinition> availableUpgrades = new List<UpgradeDefinition>();

    [Header("Object Refs")]
    [field: SerializeField] public InputProvider inputProvider { get; private set; }
    [field: SerializeField] public FishSpawner FishSpawner { get; private set; }

    [Header("Trash Meter")]
    [SerializeField] private Slider trashSliderUI;
    [SerializeField] private float trashMeterThreshold = 5f;
    [SerializeField] private float trashMeterPerClick = 1f;
    private float currentTrashMeter = 0f;

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

        InitializeUpgrades();
    }

	private void Start()
	{
		trashSliderUI.minValue = 0f;
		trashSliderUI.maxValue = 1f;
		trashSliderUI.value = 0f;
	}

	void InitializeUpgrades()
    {
        allUpgrades = new List<UpgradeDefinition>
        {
            // Tier 1
            new UpgradeDefinition(UpgradeType.BetterFish, 1, "Better Fish", "XP per fish +1"),
            new UpgradeDefinition(UpgradeType.LessTrash, 1, "Less Trash", "Fish weight +5%"),
            new UpgradeDefinition(UpgradeType.FasterSpeaker, 1, "Faster Speaker", "Clicks needed -1"),
            
            // Tier 2
            new UpgradeDefinition(UpgradeType.PremiumFish, 2, "Premium Fish", "XP per fish +2"),
            new UpgradeDefinition(UpgradeType.CleanerWaters, 2, "Cleaner Waters", "Fish weight +10%"),
            new UpgradeDefinition(UpgradeType.CastNet, 2, "Cast Net", "Instant XP boost"),
            new UpgradeDefinition(UpgradeType.BiggerNet, 2, "Bigger Net", "Net gives more XP"),
            
            // Tier 3
            new UpgradeDefinition(UpgradeType.MinionSpeaker, 3, "Minion Speaker", "Add extra spawn per roll"),
            new UpgradeDefinition(UpgradeType.MinionFishQuality, 3, "Minion Fish Quality", "Minions grant +1 XP per fish")
        };

        Debug.Log($"Upgrade System Initialized with {allUpgrades.Count} total upgrades");
    }

    #region XP & Progression
    public void AddFishXP()
    {
        float totalXP = xpPerFish + minionXPBonus;
        playerXP += totalXP;

        // cap at tier 3 threshold
        float maxXP = tierThresholds[3];
        if (playerXP > maxXP)
        {
            playerXP = maxXP;
        }

        Debug.Log($"+{totalXP} XP gained! Total XP: {playerXP}/{GetCurrentThreshold()}");

        CheckTierProgress();
    }

    public void AddToTrash()
    {
        // WIP - trash mechanic here
    }

    void CheckTierProgress()
    {
        float currentThreshold = GetCurrentThreshold();

        if (playerXP >= currentThreshold)
        {
            // Unlock next tier
            int nextTier = currentTier + 1;
            if (nextTier <= 3 && playerXP >= tierThresholds[nextTier])
            {
                currentTier = nextTier;
                Debug.Log($"Unlocked Tier {currentTier}!");
            }

            Debug.Log($"Tier {currentTier} threshold reached! ({currentThreshold} XP)");
            GenerateUpgradeChoices();
        }
    }

    float GetCurrentThreshold()
    {
        // clamp tier 1-3
        int tier = Mathf.Clamp(currentTier, 1, 3);
        return tierThresholds[tier];
    }

    void GenerateUpgradeChoices()
    {
        availableUpgrades.Clear();

        // get all upgrades for tier
        var tierUpgrades = allUpgrades.Where(u => u.tier == currentTier).ToList();

        // remove maxed ones
        tierUpgrades = tierUpgrades.Where(u => !IsUpgradeMaxed(u.type)).ToList();

        if (tierUpgrades.Count == 0)
        {
            Debug.LogWarning($"No available upgrades for tier {currentTier}!");
            return;
        }

        // show all upgrades for tier
        availableUpgrades.AddRange(tierUpgrades);

        Debug.Log($"Generated {availableUpgrades.Count} upgrade choices for Tier {currentTier}:");
        foreach (var upgrade in availableUpgrades)
        {
            Debug.Log($"  - {upgrade.displayName}: {upgrade.description}");
        }

        // tell UI to show menu
        if (UpgradeSelectionManager.instance != null)
        {
            UpgradeSelectionManager.instance.ShowUpgradeSelection(availableUpgrades, currentTier);
        }
    }

    bool IsUpgradeMaxed(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.LessTrash:
            case UpgradeType.CleanerWaters:
                return fishWeight >= 1f; // 100% fish = no more trash upgrades

            case UpgradeType.MinionSpeaker:
                return minionCount >= MAX_MINIONS; // 3 minions max

            case UpgradeType.FasterSpeaker:
                return clicksRequiredForSpawn <= 1; // min 1 click

            default:
                return false; // stackable forever
        }
    }

    public void RegisterSpeakerTransform(Transform speakerTransform)
    {
        registeredMainSpeakerTransform = speakerTransform;
    }
    #endregion

    #region Upgrades
    public void SelectUpgrade(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= availableUpgrades.Count)
        {
            Debug.LogError($"Invalid upgrade choice index: {choiceIndex}");
            return;
        }

        UpgradeDefinition selectedUpgrade = availableUpgrades[choiceIndex];
        ApplyUpgrade(selectedUpgrade.type);

        availableUpgrades.Clear();

        // Seset xp AND tier
        playerXP = 0f;
        currentTier = 1;

        Debug.Log($"Upgrade applied. XP and Tier reset. Back to Tier 1.");
    }

    void ApplyUpgrade(UpgradeType type)
    {
        Debug.Log($"Applying upgrade: {type}");

        switch (type)
        {
            // Tier 1
            case UpgradeType.BetterFish:
                xpPerFish += 1f;
                Debug.Log($"XP per fish increased to {xpPerFish}");
                break;

            case UpgradeType.LessTrash:
                fishWeight += 0.05f;
                fishWeight = Mathf.Min(fishWeight, 1f);
                Debug.Log($"Fish weight increased to {fishWeight * 100}%");
                break;

            // Tier 2
            case UpgradeType.PremiumFish:
                xpPerFish += 2f;
                Debug.Log($"XP per fish increased to {xpPerFish}");
                break;

            case UpgradeType.CleanerWaters:
                fishWeight += 0.10f;
                fishWeight = Mathf.Min(fishWeight, 1f);
                Debug.Log($"Fish weight increased to {fishWeight * 100}%");
                break;

            case UpgradeType.CastNet:
                float netXP = xpPerFish * netMultiplier;
                playerXP += netXP;
                Debug.Log($"Cast Net! Gained {netXP} instant XP");
                CheckTierProgress();
                break;

            case UpgradeType.BiggerNet:
                netMultiplier += 2;
                Debug.Log($"Net multiplier increased to {netMultiplier}x");
                break;

            case UpgradeType.FasterSpeaker:
                clicksRequiredForSpawn = Mathf.Max(1, clicksRequiredForSpawn - 1);
                Debug.Log($"Clicks required reduced to {clicksRequiredForSpawn}");
                break;

            // Tier 3
            case UpgradeType.MinionSpeaker:
                if (minionCount < MAX_MINIONS)
                {
                    minionCount++;

                    // Spawn minion speaker prefab
                    if (minionSpeakerPrefab != null && registeredMainSpeakerTransform != null)
                    {
                        Vector3 spawnOffset = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
                        Instantiate(minionSpeakerPrefab, registeredMainSpeakerTransform.position + spawnOffset, Quaternion.identity);
                    }

                    Debug.Log($"Minion speaker added! Total minions: {minionCount}");
                }
                break;

            case UpgradeType.MinionFishQuality:
                minionXPBonus += 1f;
                Debug.Log($"Minion XP bonus increased to +{minionXPBonus} per fish");
                break;
        }
    }

    #endregion

    #region Click & Spawn Logic

    public void ProcessClick()
    {
        currentClickCount++;

        if (currentClickCount >= clicksRequiredForSpawn)
        {
            currentClickCount = 0;

            RollForSpawn("Main Speaker");

            // Minion spawns
            for (int i = 0; i < minionCount; i++)
            {
                RollForSpawn($"Minion {i + 1}");
            }
        }
        else
        {
            Debug.Log($"Click accumulated ({currentClickCount}/{clicksRequiredForSpawn})");
        }
    }

    void RollForSpawn(string source)
    {
        float fishRoll = Random.value;
        bool isFish = fishRoll <= fishWeight;

        if (isFish)
        {
            FishSpawner.SpawnFish(false);
            Debug.Log($"[{source}] Fish spawned!");
        }
        else
        {
            FishSpawner.SpawnFish(true);
            Debug.Log($"[{source}] Trash spawned!");
        }
    }

    #endregion

    #region Trash
    public void ProcessTrashClick()
    {
        currentTrashMeter += trashMeterPerClick;

        if (currentTrashMeter >= trashMeterThreshold)
        {
            currentTrashMeter = 0f;
            TriggerTrashReward();
        }

        trashSliderUI.value = GetTrashMeterFill();
    }

    void TriggerTrashReward()
    {
        Debug.Log("Trash meter full! Spawning 2 guaranteed fish.");

        FishSpawner.SpawnFish(false);
        FishSpawner.SpawnFish(false);
    }

    public float GetTrashMeterFill()
    {
        return currentTrashMeter / trashMeterThreshold;
    }

    #endregion

    #region Public Getters (for UI)

    public int GetCurrentTier() => currentTier;
    public float GetProgressToNextTier() => playerXP / GetCurrentThreshold();
    public List<UpgradeDefinition> GetAvailableUpgrades() => availableUpgrades;
    public int GetMinionCount() => minionCount;
    public bool HasAvailableUpgrades() => availableUpgrades.Count > 0;

    #endregion
}