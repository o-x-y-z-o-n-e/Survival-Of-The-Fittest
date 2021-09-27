using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The home base for each player. This is where ants will spawn. Goal is to destroy other player's Base.
/// </summary>
public class Base : MonoBehaviour {


	//----------------------------------------------------------------------------------------------------------------------------------<


	public Transform Spawnpoint1;
	public Transform Spawnpoint2;


	//----------------------------------------------------------------------------------------------------------------------------------<


	[Space]
	public float Health = 100;


	//----------------------------------------------------------------------------------------------------------------------------------<


	bool isDestroyed; public bool IsDestroyed => isDestroyed;


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Takes away specified amount of health. If the Base's health reaches zero, then is will be destroyed and the game will end.
	/// </summary>
	/// <param name="amount">Health points to deduct.</param>
	public void TakeDamage(int amount) {
		if (isDestroyed) return;

		Health -= amount;

		if (Health <= 0) Destroy();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Plays the destruction animation and ends the game.
	/// </summary>
	public void Destroy() {
		if (isDestroyed) return;

		Health = 0;
		isDestroyed = true;

		Game.Current.End();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Creates a unit instance at specified position.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="path"></param>
	public void SpawnUnit(UnitType type, int path) {
		if (Game.Current.IsFinished) return;

	}
}
