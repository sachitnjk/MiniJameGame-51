using Unity.VisualScripting;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
	[SerializeField] private FishTypeSOComposite fishTypeComposite;
	[SerializeField] private GameObject blankFishPrefab;
	
	private Transform targetTransform;

	public void SpawnFish()
	{
		if(targetTransform == null)
		{
			targetTransform = GameManager.instance.registeredMainSpeakerTransform;
		}

		int typeIndex = Random.Range(0, fishTypeComposite.fishTypeSOList.Count);
		int visualIndex = Random.Range(0, fishTypeComposite.fishVisualSOList.Count);

		FishTypeSO currentFishTypeToSpawn = fishTypeComposite.fishTypeSOList[typeIndex];
		FishVisualSO currentFishVisualToSpawn = fishTypeComposite.fishVisualSOList[visualIndex];

		Vector2 spawnPos = GetSpawnPosition();
		GameObject spawnedFish = Instantiate(blankFishPrefab, spawnPos, Quaternion.identity);
		if(spawnedFish != null)
		{
			FishBase fishBase = spawnedFish.GetComponent<FishBase>();
			fishBase.Initialize(currentFishTypeToSpawn, currentFishVisualToSpawn, targetTransform);
		}
		
	}

	private Vector2 GetSpawnPosition()
	{
		Camera mainCam = Camera.main;

		float z = Mathf.Abs(mainCam.transform.position.z);

		Vector3 bottomLeft = mainCam.ViewportToWorldPoint(new Vector3(0, 0, z));
		Vector3 topRight = mainCam.ViewportToWorldPoint(new Vector3(1, 1, z));

		//How far otuside the screen teh fish would spawn
		float spawnPadding = 1.5f;

		//0-L, 1-R, 2-D
		int side = Random.Range(0, 3);

		Vector2 spawnPos = Vector2.zero;

		switch(side)
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
