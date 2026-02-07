using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public float multiClickThreshold = 0.3f;
    public int maxChain = 5;

    [SerializeField] private Transform speakerTransform;

    private int totalLeftClicks = 0;
    private int totalRightClicks = 0;

    private int leftClickChainCount = 0;
    private float lastLeftClickTime = 0f;

    private FishSpawner fishSpawner;
    private GameManager gameManager;
    private InputAction leftClickAction;
    private InputAction rightClickAction;

	private void Start()
	{
        gameManager = GameManager.instance;
        fishSpawner = gameManager.FishSpawner;

        PlayerInput playerInput = gameManager.inputProvider.GetPlayerInput();

        leftClickAction = playerInput.actions["LeftClick"];
        rightClickAction = playerInput.actions["RightClick"];

        leftClickAction.performed += HandleLeftClick;
        rightClickAction.performed += HandleRightClick;

        leftClickAction.Enable();
        rightClickAction.Enable();

        gameManager.RegisterSpeakerTransform(speakerTransform);
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
            //Spawn fish - WIP
            fishSpawner.SpawnFish();

            //Moved XP to on fish reach destination 
            //gameManager.RollForXP();
        }
    }

    void OnRightClick()
    {
        gameManager.UseUpgrade();
    }

    public int GetTotalLeftClicks() => totalLeftClicks;
    public int GetTotalRightClicks() => totalRightClicks;
}
