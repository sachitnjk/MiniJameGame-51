using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FishBase : MonoBehaviour
{
	private Vector3 spawnScale;
	private Vector3 targetScale;
	private GameObject fishVisualPrefab;

	private GameObject instantiatedFishVisual;
	private Transform targetTransform;

	[SerializeField] private float moveSpeed = 2f;

	public bool isTrash;

	private void Update()
	{
		Vector2 currentPos = transform.position;
		Vector2 targetPos = targetTransform.position;

		Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);
		transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

		float distanceToTarget = Vector2.Distance(newPos, targetPos);
		if (distanceToTarget <= GameManager.instance.consumeRadius)
		{
			if(!isTrash)
			{
				GameManager.instance.RollForXP();
			}
			else
			{
				//Still WIP
				GameManager.instance.AddToTrash();
			}

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

			//Sprite flip
			SpriteRenderer spriteRenderer = instantiatedFishVisual.GetComponentInChildren<SpriteRenderer>();
			if(spriteRenderer != null)
			{
				//flipY since flipping of x is handle by the roation logic below
				spriteRenderer.flipY = transform.position.x > target.position.x;
			}

			//Fish visual onject rotate towars target
			Vector2 direction = (target.position - transform.position).normalized;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			instantiatedFishVisual.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		}



		targetTransform = target;
	}

	private void DespawnCurrentFish()
	{
		//trigger vfx;

		SoundManager.instance?.PlayFishDeathSFX();

		Destroy(this.gameObject);
	}
}
