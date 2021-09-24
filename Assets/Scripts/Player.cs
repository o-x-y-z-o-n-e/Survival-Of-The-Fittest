using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Core class for holding Player/AI data. References to units, bases, points, etc...
/// </summary>
public class Player : MonoBehaviour {


	public Base Base;

	// Total amount of DNA the player has gained
	public int DNA;


	//----------------------------------------------------------------------------------------------------------------------------------<


	bool isAI; public bool IsAI => isAI;


	//----------------------------------------------------------------------------------------------------------------------------------<

	private bool unitsAreMoving;



	private void Start()
	{
		unitsAreMoving = true;
	}

    public bool GetUnitsAreMoving()
	{
		return unitsAreMoving;
	}

	public void SetUnitsAreMoving(bool a)
	{
		unitsAreMoving = a;
	}

}
