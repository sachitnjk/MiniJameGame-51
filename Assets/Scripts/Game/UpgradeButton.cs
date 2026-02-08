using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeButton : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button button;

    private int upgradeIndex;
    private Action<int> onClickCallback;

    public void Initialize(UpgradeDefinition upgrade, int index, Action<int> onClickCallback)
    {
        this.upgradeIndex = index;
        this.onClickCallback = onClickCallback;

        // Set text content
        if (titleText != null)
        {
            titleText.text = upgrade.displayName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = upgrade.description;
        }

        // Setup button click listener
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("Button component not assigned on UpgradeButton prefab!");
        }

        Debug.Log($"UpgradeButton initialized: {upgrade.displayName}");
    }

    private void OnButtonClicked()
    {
        Debug.Log($"Upgrade button clicked: Index {upgradeIndex}");
        onClickCallback?.Invoke(upgradeIndex);
    }

    private void OnDestroy()
    {
        // Clean up listener
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}