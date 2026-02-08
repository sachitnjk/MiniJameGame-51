using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	[field: SerializeField] public UIStateMachine UI_StateMachine { get; private set; }
	[field: SerializeField] public NotificationManager UI_NotificationManager { get; private set; }
	[field: SerializeField] public PauseState UI_PauseMenu { get; private set; }

	private void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(instance);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
