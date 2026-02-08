using Unity.VisualScripting;
using UnityEngine;

public class FishBase : MonoBehaviour
{
    private Vector3 spawnScale;
    private Vector3 targetScale;
    private GameObject fishVisualPrefab;

    private GameObject instantiatedFishVisual;
    private Transform targetTransform;

	[SerializeField] private GameObject BubbleVFX;
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
            if (!isTrash)
            {
                GameManager.instance.AddFishXP();
            }
            else
            {
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

        if (fishVisualPrefab != null)
        {
            instantiatedFishVisual = Instantiate(fishVisualPrefab, transform);

            // Sprite flip
            SpriteRenderer spriteRenderer = instantiatedFishVisual.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // flipY since flipping of x is handled by the rotation logic below
                spriteRenderer.flipY = transform.position.x > target.position.x;
            }

            // Fish visual object rotate towards target
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            instantiatedFishVisual.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        targetTransform = target;
    }

	private void SpawnBubbleBurst()
	{
		if (BubbleVFX == null) return;

		int bubbleCount = Random.Range(4, 8);
		float radius = 0.4f;

		for (int i = 0; i < bubbleCount; i++)
		{
			float angle = Random.Range(0f, Mathf.PI * 2f);
			float r = Random.Range(0.1f, radius);

			Vector3 pos = transform.position + new Vector3(
				Mathf.Cos(angle) * r,
				Mathf.Sin(angle) * r,
				0f);

			GameObject bubble = Instantiate(BubbleVFX, pos, Quaternion.identity);

			// small random scale for variety
			float s = Random.Range(0.8f, 1.3f);
			bubble.transform.localScale = Vector3.one * s;
		}
	}

    private void DespawnCurrentFish()
    {
        //trigger vfx;
        SpawnBubbleBurst();

        Destroy(this.gameObject);
    }

    private void OnMouseDown()
    {
        if (!isTrash) return;

        GameManager.instance.ProcessTrashClick();
        DespawnCurrentFish();
    }

}
