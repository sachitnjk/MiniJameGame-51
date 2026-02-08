using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationState : UIState
{
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private GameObject currentPanel;
	[SerializeField] private TextMeshProUGUI messageTextBok;
	[SerializeField] private float duration = 2f;
	[SerializeField] private float fadeTime = 0.3f;

	public override void OnEnter()
	{
		currentPanel.SetActive(true);
		StartCoroutine(PlayRoutine());
	}

	public override void OnExit()
	{
		StopAllCoroutines();
		Destroy(this.gameObject);
	}

	private IEnumerator PlayRoutine()
	{
		canvasGroup.alpha = 1f;

		yield return new WaitForSeconds(duration);

		float t = 0f;
		while (t < fadeTime)
		{
			t += Time.deltaTime;
			canvasGroup.alpha = 1f - (t / fadeTime);
			yield return null;
		}

		UIManager.instance.UI_StateMachine.Pop();
	}

	public void SetMessage(string message)
	{
		messageTextBok.text = message;
	}

}
