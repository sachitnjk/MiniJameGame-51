using UnityEngine;


[CreateAssetMenu(fileName = "FishType", menuName = "Game/Fish_Type")]
public class FishTypeSO : ScriptableObject
{
	public Vector3 initialFishSize;
	public Vector3 targetFishSize;
}
