using UnityEngine;

public class SettingsState : UIState
{
	[SerializeField] private GameObject currentPanel;

	public override void OnEnter()
	{
		currentPanel.SetActive(true);
	}

	public override void OnExit()
	{
		currentPanel.SetActive(false);
	}

	public void OnBackPressed()
	{
		PlayButtonSound();

		UIManager.instance.UI_StateMachine.Pop();
	}
}
