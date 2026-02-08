using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform speakerTransform;

    private int totalLeftClicks = 0;
    private int totalRightClicks = 0;

    private GameManager gameManager;
    private InputAction leftClickAction;
    private InputAction rightClickAction;

    private void Start()
    {
        gameManager = GameManager.instance;

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
        SoundManager.instance?.PlayClickSFX();
        
        // Process click through GameManager's spawn system
        gameManager.ProcessClick();

        Debug.Log($"Click #{totalLeftClicks} processed");
    }

    void HandleRightClick(InputAction.CallbackContext ctx)
    {
        //Sound for upgrade purchase

        totalRightClicks++;

        // Toggle upgrade menu
        if (UpgradeSelectionManager.instance != null)
        {
            UpgradeSelectionManager.instance.ToggleMenu();
        }

        Debug.Log($"Right-click #{totalRightClicks} - toggling upgrade menu");
    }

    public int GetTotalLeftClicks() => totalLeftClicks;
    public int GetTotalRightClicks() => totalRightClicks;
}