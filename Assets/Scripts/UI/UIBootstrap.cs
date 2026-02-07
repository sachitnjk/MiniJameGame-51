using UnityEngine;

public class UIBootstrap : MonoBehaviour
{
	[SerializeField] private MainMenuState mainMenuState;

	private void Start()
	{
		UIManager.instance.UI_StateMachine.Push(mainMenuState);
	}
}
