using UnityEngine;

public class BubbleVFX : MonoBehaviour
{
	private Vector3 drift;

	private void Awake()
	{
		drift = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.3f, 0.8f), 0f);
	}

	private void Update()
	{
		transform.position += drift * Time.deltaTime;
	}

	private void OnEnable()
	{
		Destroy(gameObject, 1f);
	}
}
