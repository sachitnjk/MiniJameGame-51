using UnityEngine;

public class FishBase : MonoBehaviour
{
	private Vector3 spawnScale;
	private Vector3 targetScale;
	private GameObject fishVisualPrefab;

	private GameObject instantiatedFishVisual;
	private Transform targetTransform;

	[SerializeField] private float moveSpeed = 2f;

	private void Update()
	{
		Vector2 currentPos = transform.position;
		Vector2 targetPos = targetTransform.position;

		Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);
		transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

		float distanceToTarget = Vector2.Distance(newPos, targetPos);
		if (distanceToTarget <= GameManager.instance.consumeRadius)
		{
			GameManager.instance.RollForXP();

			DespawnCurrentFish();
		}
	}

	public void Initialize(FishTypeSO fishType, FishVisualSO visualSO, Transform target)
	{
		spawnScale = fishType.initialFishSize;
		targetScale = fishType.targetFishSize;
		fishVisualPrefab = visualSO.fishVisual;

		if(fishVisualPrefab != null)
		{
			instantiatedFishVisual = Instantiate(fishVisualPrefab, transform);

			bool isOnRightSide = transform.position.x > target.position.x;
			if(isOnRightSide)
			{
				SpriteRenderer spriteRenderer = instantiatedFishVisual.GetComponentInChildren<SpriteRenderer>();
				if(spriteRenderer != null)
				{
					spriteRenderer.flipX = true;
				}
			}
		}

		targetTransform = target;
	}

	private void DespawnCurrentFish()
	{
		//trigger vfx;

		Destroy(this.gameObject);
	}
}
