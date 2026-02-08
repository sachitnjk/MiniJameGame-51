using UnityEngine;

public class BubbleFloat : MonoBehaviour
{
	private Vector3 drift;
	private float bubbleLifeTime;

	public void Init()
	{
		drift = new Vector3(
			Random.Range(-0.3f, 0.3f),
			Random.Range(0.8f, 1.4f),
			0f);

		bubbleLifeTime = Random.Range(0.8f, 1.3f);

		float scale = Random.Range(0.6f, 1.2f);
		transform.localScale = Vector3.one * scale;

		Destroy(gameObject, bubbleLifeTime);
	}

	private void Update()
	{
		transform.position += drift * Time.deltaTime;
	}
}
