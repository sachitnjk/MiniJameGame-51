using UnityEngine;

public class NotificationManager : MonoBehaviour
{
	[SerializeField] private NotificationState notificationBannerPrefab;
	[SerializeField] private Transform notificationParent;

	public void Show(string message)
	{
		NotificationState notif = Instantiate(notificationBannerPrefab, notificationParent);

		SoundManager.instance.PlayPanelSlideSFX();

		notif.SetMessage(message);

		UIManager.instance.UI_StateMachine.Push(notif);
	}
}
