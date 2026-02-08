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
		EnterFullScreen();

		PlayButtonSound();

		SceneManager.LoadScene("Playground");
	}

	public void OnSettingsPressed()
	{
		PlayButtonSound();

		SettingsState settings = (FindFirstObjectByType<SettingsState>(FindObjectsInactive.Include));

		UIManager.instance.UI_StateMachine.Push(settings);
	}

	public void OnCreditsPressed()
	{
		PlayButtonSound();

		CreditsState credits = (FindFirstObjectByType<CreditsState>(FindObjectsInactive.Include));

		UIManager.instance.UI_StateMachine.Push(credits);
	}

	public void OnExitPressed()
	{
		PlayButtonSound();

		Application.Quit();
	}

	public void EnterFullScreen()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
        Screen.fullScreen = true;
#endif
	}
}
