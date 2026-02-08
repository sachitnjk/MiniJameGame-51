using UnityEngine;

public class MenuFish : MonoBehaviour
{
	private Vector3 direction;
	private float speed;

	public void Init(Vector3 dir, float moveSpeed)
	{
		direction = dir.normalized;
		speed = moveSpeed;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0f, 0f, angle);
	}

	private void Update()
	{
		transform.position += direction * speed * Time.deltaTime;

		Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
		if (viewPos.x < -0.2f || viewPos.x > 1.2f ||
			viewPos.y < -0.2f || viewPos.y > 1.2f)
		{
			Destroy(gameObject);
		}
	}
}
