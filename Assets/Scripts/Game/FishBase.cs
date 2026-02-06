using UnityEngine;

public class FishBase : MonoBehaviour
{
	private Vector3 spawnScale;
	private Vector3 targetScale;
	private GameObject fishVisualPrefab;

	private GameObject instantiatedFishVisual;
	private Transform targetTransform;

	private Vector3 lerpStart;
	private Vector3 lerpEnd;
	private float lerpTime;
	private float lerpDuration = 20f;

	private void Update()
	{
		if(lerpTime < lerpDuration)
		{
			lerpTime += Time.deltaTime;
			float t = lerpTime/lerpDuration;

			transform.position = Vector3.Lerp(lerpStart, lerpEnd, t);
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
		}

		targetTransform = target;
		CalculateRandomTrajectory();
	}

	private void CalculateRandomTrajectory()
	{
		Vector3 currentPos = this.gameObject.transform.position;
		Vector3 targetPos = targetTransform.position;

		Vector3 direction = (targetPos - currentPos).normalized;
		float distanceToTarget = Vector3.Distance(currentPos, targetPos);
		float travelDistance = distanceToTarget * Random.Range(0.5f, 0.8f);

		lerpStart = currentPos;
		lerpEnd = currentPos + direction * travelDistance;

		Vector3 sideOffset = Random.insideUnitSphere * 2f;
		lerpEnd += sideOffset;

		lerpTime = 0f;
	}
}
