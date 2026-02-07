using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : UIState
{
	[SerializeField] private GameObject panel;

	public override void OnEnter()
	{
		panel.SetActive(true);
	}

	public override void OnExit()
	{
		panel.SetActive(false);
	}

	public void OnPlayPressed()
	{
		SceneManager.LoadScene("Playground");
	}

	public void OnSettingsPressed()
	{
		SettingsState settings = (FindFirstObjectByType<SettingsState>(FindObjectsInactive.Include));

		UIManager.instance.UI_StateMachine.Push(settings);
	}

	public void OnCreditsPressed()
	{
		CreditsState credits = (FindFirstObjectByType<CreditsState>(FindObjectsInactive.Include));

		UIManager.instance.UI_StateMachine.Push(credits);
	}

	public void OnExitPressed()
	{
		Application.Quit();
	}
}
