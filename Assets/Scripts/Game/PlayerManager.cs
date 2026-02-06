using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float multiClickThreshold = 0.3f;
    public int maxChain = 5;

    [Header("GameManager")]
    [SerializeField] private GameManager gameManager;

    private int totalLeftClicks = 0;
    private int totalRightClicks = 0;

    private int leftClickChainCount = 0;
    private float lastLeftClickTime = 0f;

    void Update()
    {
        HandleLeftClick();
        HandleRightClick();
    }

    void HandleLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            totalLeftClicks++;

            if (Time.time - lastLeftClickTime <= multiClickThreshold)
                leftClickChainCount++;
            else
                leftClickChainCount = 1;

            lastLeftClickTime = Time.time;

            ProcessLeftClickChain(leftClickChainCount);

            if (leftClickChainCount >= maxChain)
            {
                leftClickChainCount = 0;
            }
        }
    }

    void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            totalRightClicks++;
            OnRightClick();
        }
    }

    void ProcessLeftClickChain(int chainCount)
    {
        if (chainCount == 3)
        {
            gameManager.RollForXP();
        }
    }

    void OnRightClick()
    {
        gameManager.UseUpgrade();
    }

    public int GetTotalLeftClicks() => totalLeftClicks;
    public int GetTotalRightClicks() => totalRightClicks;
}
