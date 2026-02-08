using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

#region UpgradeInfo

public enum UpgradeType
{
    BetterFish,
    LessTrash,
    FasterSpeaker,
    CastNet,
    PremiumFish,
    CleanerWaters,
    BiggerNet,
    IncreaseConsumeRadius,
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
    public int clicksRequiredForSpawn = 8;
    private int currentClickCount = 0;
    public float fishWeight = 0.4f;

    [field: SerializeField] public float consumeRadius { get; private set; }

    [Header("XP Settings")]
    public float playerXP = 0f;
    public float xpPerFish = 2f;

    [Header("Tier Unlock System")]
    private int highestUnlockedTier = 1;
    private float[] tierUnlockThresholds = { 0f, 0f, 50f, 150f, 400f };
    private float[] tierUpgradeCosts = { 0f, 10f, 40f, 120f, 300f };

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

    [Header("Game UI")]
    [SerializeField] private Canvas gameUICanvas;
    [SerializeField] private TMP_Text playerXPText;
    [SerializeField] private TMP_Text nextTierText;
    [SerializeField] private TMP_Text clickFeedbackPrefab;

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
        UpdateUI();
        trashSliderUI.minValue = 0f;
		trashSliderUI.maxValue = 1f;
		trashSliderUI.value = 0f;
    }

    void InitializeUpgrades()
    {
        allUpgrades = new List<UpgradeDefinition>
        {
            new UpgradeDefinition(UpgradeType.BetterFish, 1, "Better Fish", "XP per fish +1"),
            new UpgradeDefinition(UpgradeType.LessTrash, 1, "Less Trash", "Fish weight +5%"),
            new UpgradeDefinition(UpgradeType.FasterSpeaker, 1, "Faster Speaker", "Clicks needed -1"),
            new UpgradeDefinition(UpgradeType.CastNet, 1, "Cast Net", "Instant XP boost"),

            new UpgradeDefinition(UpgradeType.PremiumFish, 2, "Premium Fish", "XP per fish +2"),
            new UpgradeDefinition(UpgradeType.CleanerWaters, 2, "Cleaner Waters", "Fish weight +10%"),

            new UpgradeDefinition(UpgradeType.BiggerNet, 3, "Bigger Net", "Net gives more XP"),
            new UpgradeDefinition(UpgradeType.IncreaseConsumeRadius, 3, "Bigger Speaker", "Increase consume radius"),

            new UpgradeDefinition(UpgradeType.MinionSpeaker, 4, "Minion Speaker", "Add extra spawn per roll"),
            new UpgradeDefinition(UpgradeType.MinionFishQuality, 4, "Minion Fish Quality", "Minions grant +1 XP per fish")
        };
    }

    #region XP & Progression

    public void AddFishXP()
    {
        float totalXP = xpPerFish + minionXPBonus;
        playerXP += totalXP;
        CheckTierProgress();
        UpdateUI();
    }

    void CheckTierProgress()
    {
        for (int i = highestUnlockedTier + 1; i < tierUnlockThresholds.Length; i++)
        {
            if (playerXP >= tierUnlockThresholds[i])
                highestUnlockedTier = i;
        }
    }

    public void RegisterSpeakerTransform(Transform speakerTransform)
    {
        registeredMainSpeakerTransform = speakerTransform;
    }

    #endregion

    #region Upgrade System

    public void GenerateUpgradeChoices()
    {
        availableUpgrades.Clear();

        for (int tier = 1; tier <= highestUnlockedTier; tier++)
        {
            var tierUpgrades = allUpgrades
                .Where(u => u.tier == tier)
                .Where(u => !IsUpgradeMaxed(u.type))
                .ToList();

            availableUpgrades.AddRange(tierUpgrades);
        }
    }

    public void SelectUpgrade(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= availableUpgrades.Count)
            return;

        UpgradeDefinition selectedUpgrade = availableUpgrades[choiceIndex];
        int tier = selectedUpgrade.tier;
        float cost = tierUpgradeCosts[tier];

        if (playerXP < cost)
        {
            UIManager.instance.UI_NotificationManager.Show("Not enough XP!");
            return;
        }

        playerXP -= cost;
        ApplyUpgrade(selectedUpgrade.type);
        GenerateUpgradeChoices();
        UpdateUI();
    }

    bool IsUpgradeMaxed(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.LessTrash:
            case UpgradeType.CleanerWaters:
                return fishWeight >= 1f;

            case UpgradeType.MinionSpeaker:
                return minionCount >= MAX_MINIONS;

            case UpgradeType.FasterSpeaker:
                return clicksRequiredForSpawn <= 1;

            default:
                return false;
        }
    }

    void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.BetterFish:
                xpPerFish += 1f;
                break;

            case UpgradeType.LessTrash:
                fishWeight = Mathf.Min(fishWeight + 0.05f, 1f);
                break;

            case UpgradeType.FasterSpeaker:
                clicksRequiredForSpawn = Mathf.Max(1, clicksRequiredForSpawn - 1);
                break;

            case UpgradeType.CastNet:
                playerXP += xpPerFish * netMultiplier;
                CheckTierProgress();
                break;

            case UpgradeType.PremiumFish:
                xpPerFish += 2f;
                break;

            case UpgradeType.CleanerWaters:
                fishWeight = Mathf.Min(fishWeight + 0.10f, 1f);
                break;

            case UpgradeType.BiggerNet:
                netMultiplier += 2;
                break;

            case UpgradeType.IncreaseConsumeRadius:
                consumeRadius += 0.5f;
                break;

            case UpgradeType.MinionSpeaker:
                if (minionCount < MAX_MINIONS)
                {
                    minionCount++;

                    if (minionSpeakerPrefab != null && registeredMainSpeakerTransform != null)
                    {
                        Vector3 basePos = registeredMainSpeakerTransform.position;
                        Vector3 offset = Vector3.zero;

                        if (minionCount == 1)
                            offset = new Vector3(-50f, 0f, 0f);
                        else if (minionCount == 2)
                            offset = new Vector3(50f, 0f, 0f);
                        else if (minionCount == 3)
                            offset = new Vector3(0f, -50f, 0f);

                        Instantiate(minionSpeakerPrefab, basePos + offset, Quaternion.identity);
                    }
                }
                break;

            case UpgradeType.MinionFishQuality:
                minionXPBonus += 1f;
                break;
        }
    }

    #endregion

    #region Click & Spawn Logic

    public void ProcessClick()
    {
        currentClickCount++;

        SpawnClickFeedback();

        if (currentClickCount >= clicksRequiredForSpawn)
        {
            currentClickCount = 0;

            RollForSpawn("Main Speaker");

            for (int i = 0; i < minionCount; i++)
            {
                RollForSpawn($"Minion {i + 1}");
            }
        }
    }

    void RollForSpawn(string source)
    {
        bool isFish = Random.value <= fishWeight;

        if (isFish)
            FishSpawner.SpawnFish(false);
        else
            FishSpawner.SpawnFish(true);
    }

    void SpawnClickFeedback()
    {
        if (clickFeedbackPrefab == null || gameUICanvas == null)
            return;

        TMP_Text instance = Instantiate(clickFeedbackPrefab, gameUICanvas.transform);
        instance.text = currentClickCount + "x";

        RectTransform rect = instance.rectTransform;
        Vector2 screenPos = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameUICanvas.transform as RectTransform,
            screenPos,
            gameUICanvas.worldCamera,
            out Vector2 localPoint
        );

        rect.anchoredPosition = localPoint;
        rect.localScale = Vector3.one * 1.5f;

        StartCoroutine(AnimateClickFeedback(rect, instance));
    }

    System.Collections.IEnumerator AnimateClickFeedback(RectTransform rect, TMP_Text text)
    {
        float duration = 1.0f;
        float elapsed = 0f;

        Vector3 startScale = rect.localScale;
        Vector3 endScale = Vector3.one * 0.7f;

        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * 200f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float eased = 1f - Mathf.Pow(1f - t, 3f);

            rect.localScale = Vector3.Lerp(startScale, endScale, eased);
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);

            yield return null;
        }

        Destroy(text.gameObject);
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
        FishSpawner.SpawnFish(false);
        FishSpawner.SpawnFish(false);
    }

    public float GetTrashMeterFill()
    {
        return currentTrashMeter / trashMeterThreshold;
    }

    #endregion

    #region UI

    private void UpdateUI()
    {
        if (playerXPText != null)
            playerXPText.text = $"{Mathf.FloorToInt(playerXP)}";

        if (nextTierText != null)
        {
            if (highestUnlockedTier < tierUnlockThresholds.Length - 1)
            {
                float nextThreshold = tierUnlockThresholds[highestUnlockedTier + 1];
                nextTierText.text = $"{nextThreshold}";
            }
            else
            {
                nextTierText.text = "Max Tier Reached";
            }
        }
    }

    #endregion

    #region Public Getters

    public int GetHighestUnlockedTier() => highestUnlockedTier;
    public float GetPlayerXP() => playerXP;
    public List<UpgradeDefinition> GetAvailableUpgrades() => availableUpgrades;
    public int GetMinionCount() => minionCount;

    public bool HasAvailableUpgrades()
    {
        GenerateUpgradeChoices();
        return availableUpgrades.Count > 0;
    }

    #endregion
}
