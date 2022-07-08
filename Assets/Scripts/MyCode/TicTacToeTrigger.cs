using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeTrigger : MonoBehaviour
{
	[SerializeField] private bool debug;

	[SerializeField] public int index;

    [SerializeField] public bool canClick = true;

	public delegate void TriggerCLicked(GameObject gameobject);
	public static event TriggerCLicked onTriggerCLicked;


    public void TriggerHit()
	{
		// Checks to see if this trigger is clickable, then sends an event
		if (canClick)
		{
			onTriggerCLicked?.Invoke(gameObject);
			if (debug) Debug.Log("Trigger Hit " + index);
		}
	}
}
