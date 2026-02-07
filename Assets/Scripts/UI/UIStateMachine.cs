using System.Collections.Generic;
using UnityEngine;

public class UIStateMachine : MonoBehaviour
{
	private Stack<UIState> stateStack = new Stack<UIState> ();

	public void Push(UIState state)
	{
		if(stateStack.Count > 0)
		{
			stateStack.Peek().OnExit();
		}

		stateStack.Push(state);
		state.OnEnter();
	}

	public void Pop()
	{
		if(stateStack.Count == 0) return;

		stateStack.Pop().OnExit();

		if(stateStack.Count > 0)
		{
			stateStack.Peek().OnEnter();
		}
	}
}
