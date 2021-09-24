using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Core class for holding Player/AI data. References to units, bases, points, etc...
/// </summary>
public class Player : MonoBehaviour {


	public Base Base;

	public int DNA; // Total amount of DNA the player has gained

	public Evolution evolution;

	public int playerID;


	//----------------------------------------------------------------------------------------------------------------------------------<


	bool isAI; public bool IsAI => isAI;


	//----------------------------------------------------------------------------------------------------------------------------------<

	private bool unitsAreMoving;



	private void Start()
	{
		unitsAreMoving = true;
		evolution = new Evolution();
	}

	private void OnDestroy()
	{
		evolution = null;
	}

	public bool GetUnitsAreMoving()
	{
		return unitsAreMoving;
	}

	public void SetUnitsAreMoving(bool a)
	{
		unitsAreMoving = a;
	}


	public void AddDNA(int amount) {
		DNA += amount;

		Game.Current.UI.UpdateDNA(DNA, playerID);
	}

}
