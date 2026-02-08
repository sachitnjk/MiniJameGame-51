using UnityEngine;

public class FishSpawner : MonoBehaviour
{
	[SerializeField] private FishTypeSOComposite fishTypeComposite;
	[SerializeField] private FishTypeSOComposite trashTypeComposite;
	[SerializeField] private GameObject blankFishPrefab;
	[SerializeField] private GameObject trashFishPrefab;

	private Transform targetTransform;

	public void SpawnFish(bool isTrash)
	{
		if (targetTransform == null)
		{
			targetTransform = GameManager.instance.registeredMainSpeakerTransform;
		}

		// Select prefab and composite based on whether it's trash
		GameObject prefabToSpawn = isTrash ? trashFishPrefab : blankFishPrefab;
		FishTypeSOComposite compositeToUse = isTrash ? trashTypeComposite : fishTypeComposite;

		// Randomly select type and visual from the composite
		int typeIndex = Random.Range(0, compositeToUse.fishTypeSOList.Count);
		int visualIndex = Random.Range(0, compositeToUse.fishVisualSOList.Count);

		FishTypeSO fishType = compositeToUse.fishTypeSOList[typeIndex];
		FishVisualSO fishVisual = compositeToUse.fishVisualSOList[visualIndex];

		Vector2 spawnPos = GetSpawnPosition();
		GameObject spawnedFish = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

		if (spawnedFish != null)
		{
			FishBase fishBase = spawnedFish.GetComponent<FishBase>();
			fishBase.Initialize(fishType, fishVisual, targetTransform);
			fishBase.isTrash = isTrash; // Set trash flag
		}
	}

	private Vector2 GetSpawnPosition()
	{
		Camera mainCam = Camera.main;
		float z = Mathf.Abs(mainCam.transform.position.z);
		Vector3 bottomLeft = mainCam.ViewportToWorldPoint(new Vector3(0, 0, z));
		Vector3 topRight = mainCam.ViewportToWorldPoint(new Vector3(1, 1, z));

		// How far outside the screen the fish would spawn
		float spawnPadding = 1.5f;

		// 0-L, 1-R, 2-D
		int side = Random.Range(0, 3);
		Vector2 spawnPos = Vector2.zero;

		switch (side)
		{
			case 0:
				spawnPos.x = bottomLeft.x - spawnPadding;
				spawnPos.y = Random.Range(bottomLeft.y, topRight.y);
				break;
			case 1:
				spawnPos.x = topRight.x + spawnPadding;
				spawnPos.y = Random.Range(bottomLeft.y, topRight.y);
				break;
			case 2:
				spawnPos.y = bottomLeft.y - spawnPadding;
				spawnPos.x = Random.Range(bottomLeft.x, topRight.x);
				break;
		}

		return spawnPos;
	}
}