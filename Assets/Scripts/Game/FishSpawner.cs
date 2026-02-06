using UnityEngine;

public class FishSpawner : MonoBehaviour
{
	[SerializeField] private FishTypeSOComposite fishTypeComposite;
	[SerializeField] private GameObject blankFishPrefab;
	[SerializeField] private Transform targetTransform;

	public void SpawnFish()
	{
		int typeIndex = Random.Range(0, fishTypeComposite.fishTypeSOList.Count);
		int visualIndex = Random.Range(0, fishTypeComposite.fishVisualSOList.Count);

		FishTypeSO currentFishTypeToSpawn = fishTypeComposite.fishTypeSOList[typeIndex];
		FishVisualSO currentFishVisualToSpawn = fishTypeComposite.fishVisualSOList[visualIndex];

		GameObject spawnedFish = Instantiate(blankFishPrefab);
		if(spawnedFish != null)
		{
			FishBase fishBase = spawnedFish.GetComponent<FishBase>();
			fishBase.Initialize(currentFishTypeToSpawn, currentFishVisualToSpawn, targetTransform);
		}
		
	}
}
