using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour
{
	[SerializeField] private PlayerInput playerInputs;

	public PlayerInput GetPlayerInput()
	{
		return playerInputs;
	}
}
