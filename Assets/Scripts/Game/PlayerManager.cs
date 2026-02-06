using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public float multiClickThreshold = 0.3f;
    public int maxChain = 5;


    private int totalLeftClicks = 0;
    private int totalRightClicks = 0;

    private int leftClickChainCount = 0;
    private float lastLeftClickTime = 0f;

    private GameManager gameManager;
    private InputAction leftClickAction;
    private InputAction rightClickAction;

	private void OnEnable()
	{
        gameManager = GameManager.instance;

        PlayerInput playerInput = gameManager.inputProvider.GetPlayerInput();

        leftClickAction = playerInput.actions["LeftClick"];
        rightClickAction = playerInput.actions["RightClick"];

        leftClickAction.performed += HandleLeftClick;
        rightClickAction.performed += HandleRightClick;

        leftClickAction.Enable();
        rightClickAction.Enable();
	}

	private void OnDisable()
	{
		leftClickAction.performed -= HandleLeftClick;
		rightClickAction.performed -= HandleRightClick;
	}

    void HandleLeftClick(InputAction.CallbackContext ctx)
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

    void HandleRightClick(InputAction.CallbackContext ctx)
    {
        totalRightClicks++;
        OnRightClick();
    }

    void ProcessLeftClickChain(int chainCount)
    {
        if (chainCount == 3)
        {
            //Spawn fish
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
