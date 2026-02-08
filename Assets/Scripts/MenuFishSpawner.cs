using UnityEngine;

public class MenuFishSpawner : MonoBehaviour
{
	[SerializeField] private GameObject[] fishPrefabs;
	[SerializeField] private float spawnInterval = 1.5f;
	[SerializeField] private float minSpeed = 0.5f;
	[SerializeField] private float maxSpeed = 1.5f;

	private float timer;

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= spawnInterval)
		{
			SpawnFish();
			timer = 0f;
		}
	}

	void SpawnFish()
	{
		GameObject prefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];

		Vector3 spawnPos;
		Vector3 dir;

		int side = Random.Range(0, 4);

		switch (side)
		{
			case 0: // left
				spawnPos = ViewportToWorld(-0.1f, Random.value);
				dir = Vector3.right;
				break;

			case 1: // right
				spawnPos = ViewportToWorld(1.1f, Random.value);
				dir = Vector3.left;
				break;

			case 2: // bottom
				spawnPos = ViewportToWorld(Random.value, -0.1f);
				dir = Vector3.up;
				break;

			default: // top
				spawnPos = ViewportToWorld(Random.value, 1.1f);
				dir = Vector3.down;
				break;
		}

		GameObject fish = Instantiate(prefab, spawnPos, Quaternion.identity);
		fish.GetComponent<MenuFish>().Init(dir, Random.Range(minSpeed, maxSpeed));
	}

	Vector3 ViewportToWorld(float x, float y)
	{
		Vector3 pos = Camera.main.ViewportToWorldPoint(
			new Vector3(x, y, -Camera.main.transform.position.z));
		pos.z = 0f;
		return pos;
	}
}
