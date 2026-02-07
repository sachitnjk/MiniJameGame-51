using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	[field: SerializeField] public UIStateMachine UI_StateMachine { get; private set; }

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
