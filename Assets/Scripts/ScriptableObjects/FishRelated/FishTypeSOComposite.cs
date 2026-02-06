using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishType", menuName = "Game/Fish_Type_Composite")]
public class FishTypeSOComposite : ScriptableObject
{
	public List<FishTypeSO> fishTypeSOList = new List<FishTypeSO>();
	public List<FishVisualSO> fishVisualSOList = new List<FishVisualSO>();
}
