using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform speakerTransform;
    [SerializeField] private GameObject clickBubblePrefab;

    private int totalLeftClicks = 0;
    private int totalRightClicks = 0;

    private GameManager gameManager;
    private InputAction leftClickAction;
    private InputAction rightClickAction;
    private InputAction pauseAction;

    private bool isPaused = false;

    private void Start()
    {
        gameManager = GameManager.instance;

        PlayerInput playerInput = gameManager.inputProvider.GetPlayerInput();

        leftClickAction = playerInput.actions["LeftClick"];
        rightClickAction = playerInput.actions["RightClick"];
        pauseAction = playerInput.actions["Pause"];

        leftClickAction.performed += HandleLeftClick;
        rightClickAction.performed += HandleRightClick;
        pauseAction.performed += HandlePause;

        leftClickAction.Enable();
        rightClickAction.Enable();

        gameManager.RegisterSpeakerTransform(speakerTransform);
    }

    private void OnDisable()
    {
        leftClickAction.performed -= HandleLeftClick;
        rightClickAction.performed -= HandleRightClick;
        pauseAction.performed -= HandlePause;
    }

    void HandleLeftClick(InputAction.CallbackContext ctx)
    {
        totalLeftClicks++;

        SpawnClickBubbles(ctx);
        SoundManager.instance?.PlayClickSFX();
        
        // Process click through GameManager's spawn system
        if (!ClickedOnTrash())
            gameManager.ProcessClick();

        Debug.Log($"Click #{totalLeftClicks} processed");
    }

    private void SpawnClickBubbles(InputAction.CallbackContext ctx)
    {
		Vector2 screenPos = Mouse.current.position.ReadValue();

		Vector3 screenPos3D = new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos3D);
		worldPos.z = 0f;

		int count = Random.Range(2, 4);

		for (int i = 0; i < count; i++)
		{
			Vector3 offset = new Vector3(
				Random.Range(-0.15f, 0.15f),
				Random.Range(-0.15f, 0.15f),
				0f);

			GameObject instantiatedBubble = Instantiate(clickBubblePrefab, worldPos, Quaternion.identity);
			instantiatedBubble.GetComponent<BubbleFloat>().Init();
		}
	}

    bool ClickedOnTrash()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            FishBase fish = hit.collider.GetComponent<FishBase>();
            if (fish != null && fish.isTrash)
            {
                return true;
            }
        }

        return false;
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

    void HandlePause(InputAction.CallbackContext ctx)
    {
        if(isPaused)
        {
            isPaused = false;

            UIManager.instance.UI_StateMachine.Pop();
        }
        else
        {
            isPaused = true;

            UIManager.instance.UI_StateMachine.Push(UIManager.instance.UI_PauseMenu);
        }
    }


	public int GetTotalLeftClicks() => totalLeftClicks;
    public int GetTotalRightClicks() => totalRightClicks;
}