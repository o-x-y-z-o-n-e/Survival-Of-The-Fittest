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

	public int MaxHealth = 100;
	public float Health = 100;


	//----------------------------------------------------------------------------------------------------------------------------------<


	bool isDestroyed; public bool IsDestroyed => isDestroyed;


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Adds a percentage of the MaxHealth back into the health of the base. Range: [0, 1] = 0% to 100%
	/// </summary>
	/// <param name="percent">Range: [0, 1] = 0% to 100%</param>
	/// <returns></returns>
	public float Repair(float percent) => Repair(MaxHealth * percent);


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Adds health back onto base.
	/// </summary>
	/// <param name="amount"></param>
	public void Repair(int amount) {
		if (isDestroyed) return;

		Health += Mathf.Abs(amount);

		if (Health >= MaxHealth) Health = MaxHealth;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Takes away specified amount of health. If the Base's health reaches zero, then is will be destroyed and the game will end.
	/// </summary>
	/// <param name="amount">Health points to deduct.</param>
	public void TakeDamage(int amount) {
		if (isDestroyed) return;

		Health -= Mathf.Abs(amount);

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
		if (Game.Current.Freeze) return;

	}
}
