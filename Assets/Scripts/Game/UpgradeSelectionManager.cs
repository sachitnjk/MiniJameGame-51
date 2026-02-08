using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class UpgradeSelectionManager : MonoBehaviour
{
    public static UpgradeSelectionManager instance;

    [Header("UI References")]
    [SerializeField] private RectTransform upgradeMenuPanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject upgradeButtonPrefab;

    [Header("Slide Animation Settings")]
    [SerializeField] private float slideSpeed = 800f;
    [SerializeField] private float hiddenXPosition = 400f;
    [SerializeField] private float visibleXPosition = 0f;

    private List<GameObject> activeButtons = new List<GameObject>();
    private bool isMenuVisible = false;
    private int currentDisplayedTier = 0;
    private Vector2 targetPosition;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        if (upgradeMenuPanel != null)
        {
            upgradeMenuPanel.anchoredPosition = new Vector2(hiddenXPosition, upgradeMenuPanel.anchoredPosition.y);
            targetPosition = upgradeMenuPanel.anchoredPosition;
        }
    }

    private void Update()
    {
        if (upgradeMenuPanel != null)
        {
            upgradeMenuPanel.anchoredPosition = Vector2.MoveTowards(
                upgradeMenuPanel.anchoredPosition,
                targetPosition,
                slideSpeed * Time.unscaledDeltaTime
            );
        }
    }

    public void ToggleMenu()
    {
        if (GameManager.instance.HasAvailableUpgrades())
        {
            if (isMenuVisible)
            {
                HideMenu();
            }
            else
            {
                ShowMenu();
            }
        }
        else
        {
            Debug.Log("No upgrades available yet. Reach a tier threshold first!");
        }
    }

    private void ShowMenu()
    {
        isMenuVisible = true;
        targetPosition = new Vector2(visibleXPosition, upgradeMenuPanel.anchoredPosition.y);
        Debug.Log("Showing upgrade menu");
    }

    private void HideMenu()
    {
        isMenuVisible = false;
        targetPosition = new Vector2(hiddenXPosition, upgradeMenuPanel.anchoredPosition.y);
        Debug.Log("Hiding upgrade menu");
    }

    public void ShowUpgradeSelection(List<UpgradeDefinition> upgrades, int tier)
    {
        if (upgradeMenuPanel == null || buttonContainer == null || upgradeButtonPrefab == null)
        {
            Debug.LogError("UpgradeSelectionManager: Missing UI references!");
            return;
        }

        // Refresh if tier changed or no buttons
        bool tierChanged = (tier != currentDisplayedTier);
        bool noButtonsExist = (activeButtons.Count == 0);

        if (tierChanged || noButtonsExist)
        {
            if (tierChanged)
            {
                Debug.Log($"Tier changed from {currentDisplayedTier} to {tier}! Refreshing upgrade buttons.");
                currentDisplayedTier = tier;
            }

            RefreshButtons(upgrades);
        }

        // Auto-show menu
        ShowMenu();

        Debug.Log($"Upgrade selection ready with {upgrades.Count} choices from Tier {tier}. Right-click to toggle menu.");
    }

    private void RefreshButtons(List<UpgradeDefinition> upgrades)
    {
        ClearButtons();

        // Spawn button
        for (int i = 0; i < upgrades.Count; i++)
        {
            UpgradeDefinition upgrade = upgrades[i];
            int choiceIndex = i;

            GameObject buttonObj = Instantiate(upgradeButtonPrefab, buttonContainer);
            UpgradeButton upgradeButton = buttonObj.GetComponent<UpgradeButton>();

            if (upgradeButton != null)
            {
                upgradeButton.Initialize(upgrade, choiceIndex, OnUpgradeSelected);
                activeButtons.Add(buttonObj);
            }
            else
            {
                Debug.LogError("UpgradeButton component not found on prefab!");
            }
        }
    }

    private void OnUpgradeSelected(int choiceIndex)
    {
        Debug.Log($"Player selected upgrade at index {choiceIndex}");

        // Apply through GameManager
        GameManager.instance.SelectUpgrade(choiceIndex);

        // Refresh with remaining upgrades
        var remainingUpgrades = GameManager.instance.GetAvailableUpgrades();

        if (remainingUpgrades.Count > 0)
        {
            RefreshButtons(remainingUpgrades);
        }
        else
        {
            // No upgrades left
            ClearButtons();
            currentDisplayedTier = 0;
            HideMenu();
        }
    }

    private void ClearButtons()
    {
        foreach (GameObject button in activeButtons)
        {
            Destroy(button);
        }
        activeButtons.Clear();
    }
}