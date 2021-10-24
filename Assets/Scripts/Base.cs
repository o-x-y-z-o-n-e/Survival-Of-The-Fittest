using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The home base for each player. This is where ants will spawn. Goal is to destroy other player's Base.
/// </summary>
public class Base : MonoBehaviour, Damageable {


	//----------------------------------------------------------------------------------------------------------------------------------<

	public Player Player;
	public Transform UnitContainer;
	public Transform Spawnpoint1;
	public Transform Spawnpoint2;
	public Animator anim;

	private Image healthBar;


	//----------------------------------------------------------------------------------------------------------------------------------<


	[Space]

	public int MaxHealth = 100;
	public int Health = 100;
	public float ReflectedDamage = 0;


	//----------------------------------------------------------------------------------------------------------------------------------<


	bool isDestroyed; public bool IsDestroyed => isDestroyed;


	LinkedList<UnitController> topUnits = new LinkedList<UnitController>();
	LinkedList<UnitController> bottomUnits = new LinkedList<UnitController>();


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Awake() {
		Health = MaxHealth;
		healthBar = GetComponentsInChildren<Image>()[1];
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Adds a percentage of the MaxHealth back into the health of the base. Range: [0, 1] = 0% to 100%
	/// </summary>
	/// <param name="percent">Range: [0, 1] = 0% to 100%</param>
	/// <returns></returns>
	public void Repair(float percent) => Repair(Mathf.RoundToInt(MaxHealth * percent));


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Adds health back onto base.
	/// </summary>
	/// <param name="amount"></param>
	public void Repair(int amount) {
		if (isDestroyed) return;

		Health += Mathf.Abs(amount);

		if (Health >= MaxHealth) Health = MaxHealth;

		UpdateHealthBar();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Takes away specified amount of health. If the Base's health reaches zero, then is will be destroyed and the game will end.
	/// </summary>
	/// <param name="amount">Health points to deduct.</param>
	public bool TakeDamage(int amount, Player sender) {
		if (isDestroyed) return false;

		Health -= Mathf.Abs(amount);

		/*
		healthBar.transform.localScale = new Vector3(
			((float)Health / 100) * healthBar.transform.localScale.x,
			healthBar.transform.localScale.y,
			healthBar.transform.localScale.z);
		*/

		UpdateHealthBar();

		if (Health <= 0) {
			Destroy();
			return true;
		}

		return false;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void UpdateHealthBar() => healthBar.rectTransform.sizeDelta = new Vector2(((float)Health / MaxHealth) * 300, 15);


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Plays the destruction animation and ends the game.
	/// </summary>
	public void Destroy() {
		if (isDestroyed) return;

		anim.SetBool("hiveDestroy", true);
		SoundManagerScript.PlaySound("Hive_Destruction");
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
	public void SpawnUnit(UnitType type, Path path) {
		if (Game.Current.Freeze) return;

		int cost = UnitController.GetUnitBaseCost(type);

		if (cost > Player.DNA) return;

		Player.ChangeDNA(-cost);

		UnitController prefab = Resources.Load<UnitController>(UnitController.GetUnitPrefabPath(type));
		if (prefab == null) return;


		Vector3 corePosition = path == Path.Surface ? Spawnpoint1.position : Spawnpoint2.position;
		int direction = Game.Current.Player1 == Player ? 1 : -1;
		const float CHECK_DISTANCE = 20f;

		RaycastHit2D hit;
		hit = Physics2D.Raycast(corePosition - (Vector3.right * direction * CHECK_DISTANCE), Vector2.right * direction, CHECK_DISTANCE);

		if(hit.collider != null) {
			corePosition = hit.point - (Vector2.right * direction * (prefab.GetComponent<BoxCollider2D>().size.x/2f));
		}


		UnitController instance = Instantiate(prefab, corePosition, Quaternion.identity, UnitContainer);
		instance.name = instance.Type.ToString();
		instance.SetPlayer(Player);

		
		instance.SetDirection(direction);

		instance.SetSprite(Player.Evolutions.GetEvolutionLevel());

		if (path == Path.Surface) topUnits.AddLast(instance);
		else bottomUnits.AddLast(instance);
	}




	public int GetOwnerID() => Player.PlayerID;
	public new int GetInstanceID() => gameObject.GetInstanceID();
	public float GetWidth() => 0;
	public Transform GetTransform() => transform;
	public bool IsDead() => Health <= 0;

	public void RemoveUnit(UnitController unit) {
		if (!topUnits.Remove(unit)) bottomUnits.Remove(unit);
	}

	public UnitController GetUnit(int index, Path path) {
		LinkedList<UnitController> units = (path == Path.Surface ? topUnits : bottomUnits);

		if (units.Count == 0) return null;

		int i = 0;
		LinkedListNode<UnitController> node = units.First;
		while (true) {
			if (i == index) return node.Value;
			else if (node.Next != null) node = node.Next;
			else return null;
		}
	}
}
