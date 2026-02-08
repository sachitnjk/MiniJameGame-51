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
        GameManager.instance.GenerateUpgradeChoices();
        var upgrades = GameManager.instance.GetAvailableUpgrades();

        if (upgrades.Count > 0)
        {
            if (isMenuVisible)
            {
                HideMenu();
            }
            else
            {
                RefreshButtons(upgrades);
                ShowMenu();
            }
        }
        else
        {
            UIManager.instance.UI_NotificationManager.Show("No upgrades available.");
        }
    }

    private void ShowMenu()
    {
        isMenuVisible = true;
        targetPosition = new Vector2(visibleXPosition, upgradeMenuPanel.anchoredPosition.y);
    }

    private void HideMenu()
    {
        isMenuVisible = false;
        targetPosition = new Vector2(hiddenXPosition, upgradeMenuPanel.anchoredPosition.y);
    }

    public void ShowUpgradeSelection(List<UpgradeDefinition> upgrades, int tier)
    {
        RefreshButtons(upgrades);
        ShowMenu();
    }

    private void RefreshButtons(List<UpgradeDefinition> upgrades)
    {
        ClearButtons();

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
        }
    }

    private void OnUpgradeSelected(int choiceIndex)
    {
        GameManager.instance.SelectUpgrade(choiceIndex);
        RefreshButtons(GameManager.instance.GetAvailableUpgrades());
    }

    private void ClearButtons()
    {
        foreach (GameObject button in activeButtons)
        {
            Destroy(button);
        }

        activeButtons.Clear();
    }

    public void RefreshIfOpen()
    {
        if (!isMenuVisible)
            return;

        RefreshButtons(GameManager.instance.GetAvailableUpgrades());
    }
}
