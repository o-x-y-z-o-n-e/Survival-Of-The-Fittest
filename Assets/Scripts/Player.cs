using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Core class for holding Player/AI data. References to units, bases, points, etc...
/// </summary>
public class Player : MonoBehaviour {


	public Base Base;

	[Space]

	public int DNA; // Total amount of DNA the player has gained

	[Space]

	public Evolution Evolutions;
	public UnitModifiers WorkerModifiers;
	public UnitModifiers SoldierModifiers;
	public UnitModifiers SpitterModifiers;
	public UnitModifiers DefenderModifiers;

	[Space]

	public int PlayerID;


	//----------------------------------------------------------------------------------------------------------------------------------<


	bool isAI; public bool IsAI => isAI;


	//----------------------------------------------------------------------------------------------------------------------------------<

	private bool unitsAreMoving;



	private void Awake()
	{
		unitsAreMoving = true;
		Evolutions = new Evolution(this);
	}

	private void OnDestroy()
	{
		Evolutions = null;
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

		Game.Current.UI.UpdateDNA(DNA, PlayerID);
	}

}
