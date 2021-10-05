using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Core class for holding Player/AI data. References to units, bases, points, etc...
/// </summary>
public class Player : MonoBehaviour {

	const float DNA_UPDATE_INTERVAL = 1f;

	public Base Base;

	[Space]

	public int DNA; // Total amount of DNA the player has gained
	public int DNAPerMinute = 600;

	[Space]

	public Evolution Evolutions;
	public UnitModifiers WorkerModifiers = new UnitModifiers();
	public UnitModifiers SoldierModifiers = new UnitModifiers();
	public UnitModifiers SpitterModifiers = new UnitModifiers();
	public UnitModifiers DefenderModifiers = new UnitModifiers();

	[Space]

	public int PlayerID;


	//----------------------------------------------------------------------------------------------------------------------------------<


	bool isAI; public bool IsAI => isAI;
	int selectedPath = 0; public int SelectedPath => selectedPath;

	float remainingDNA = 0;
	float counterDNA = 0;


	//----------------------------------------------------------------------------------------------------------------------------------<


	private void Awake() {
		Evolutions = new Evolution(this);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	private void OnDestroy() {
		Evolutions = null;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Update() {
		if (Game.Current.Freeze) return;

		GatherDNA();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void GatherDNA() {
		float rate = DNAPerMinute / 60;
		remainingDNA += Time.deltaTime * rate;

		counterDNA += Time.deltaTime;
		if(counterDNA >= DNA_UPDATE_INTERVAL) {
			counterDNA = 0;

			int whole = (int)remainingDNA;
			remainingDNA = remainingDNA % 1;

			ChangeDNA(whole);
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void ChangeDNA(int amount) {
		DNA += amount;

		Game.Current.UI.UpdateDNA(DNA, PlayerID);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public UnitModifiers GetModifierReference(UnitType type) {
		switch(type) {
			case UnitType.Worker: {
				return WorkerModifiers;
			}
			case UnitType.Soldier: {
				return SoldierModifiers;
			}
			case UnitType.Spitter: {
				return SpitterModifiers;
			}
			case UnitType.Defender: {
				return DefenderModifiers;
			}
		}
		return null;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void SelectPath(int path) {
		selectedPath = path;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void SetDNAGenerationRate(int bpm) {
		DNAPerMinute = bpm;
	}
}
