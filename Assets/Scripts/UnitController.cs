using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitController : MonoBehaviour, Damageable {

	public const string WORKER_PREFAB_PATH		= "Prefabs/Worker";
	public const string SOLDIER_PREFAB_PATH		= "Prefabs/Soldier";
	public const string SPITTER_PREFAB_PATH		= "Prefabs/Spitter";
	public const string DEFENDER_PREFAB_PATH	= "Prefabs/Defender";

	public const int	WORKER_DNA_COST			= 25;
	public const int	SOLDIER_DNA_COST		= 100;
	public const int	SPITTER_DNA_COST		= 150;
	public const int	DEFENDER_DNA_COST		= 250;

	const float			FRIENDLY_OVERLAP		= 0.0f;


	public UnitType Type;


	[SerializeField] private float attackRange = 0.2f;
    [SerializeField] private int direction; public int Direction => direction;
    [SerializeField] private int baseHealth;
    [SerializeField] private int damage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackInterval;
	[SerializeField] private float attackIntervalDeviation;
	[SerializeField] private int giveDNA; // how much DNA the unit will give when killed

    [SerializeField] private Player unitOwner; //this will not be a serialized field once unit owners are assigned at time of prefab instantiation.

	SpriteRenderer sprite;
	Animator animator;
	new BoxCollider2D collider;
	UnitModifiers modifiers;
	Image healthBar;

	bool stopMoving;
	float attackCounter;
	int health;

	int enemyMask;
	int allyMask;


	//Damagable frontUnit;
	//Damagable frontBase;

	UnitController nextAlly;
	Damageable nextEnemy;


	//----------------------------------------------------------------------------------------------------------------------------------<


	// Start is called before the first frame update
	void Awake() {
		animator = GetComponentInChildren<Animator>();
		sprite = GetComponentInChildren<SpriteRenderer>();
		collider = GetComponent<BoxCollider2D>();
		healthBar = GetComponentsInChildren<Image>()[1];

		if (unitOwner != null) SetPlayer(unitOwner);
    }
	

	//----------------------------------------------------------------------------------------------------------------------------------<


	public void SetPlayer(Player player) {
		unitOwner = player;
		modifiers = unitOwner.GetModifierReference(Type);

		health = (int)(baseHealth * modifiers.Health);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	// Update is called once per frame
	void Update() {
		if (Game.Current.Freeze) return;

		CheckCollision();
		CheckMove();
		CheckAttack();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void CheckCollision() {
		//Check for next enemy
		RaycastHit2D hit;
		Vector3 origin = transform.localPosition + (Vector3.right * collider.size.x * direction);
		hit = Physics2D.Raycast(origin, Vector3.right * direction, attackRange, enemyMask);

		if (hit.collider != null) {
			Damageable enemy = null;

			if (nextEnemy == null) {
				enemy = hit.collider.GetComponent<Damageable>();
			} else {
				if (nextEnemy.IsDead()) nextEnemy = null;
				else if (nextEnemy.GetInstanceID() != hit.collider.gameObject.GetInstanceID()) {
					enemy = hit.collider.GetComponent<Damageable>();
				}
			}

			if (enemy != null) {
				nextEnemy = enemy;
				attackCounter = GetNextAttackSpeed();
			}
		} else nextEnemy = null;


		//Check for next ally
		hit = Physics2D.Raycast(origin, Vector3.right * direction, 1f, allyMask);

		if (hit.collider != null) {
			UnitController ally = null;

			if (nextAlly == null) {
				ally = hit.collider.GetComponent<UnitController>();
			} else {
				if (nextAlly.IsDead()) nextAlly = null;
				else if (nextAlly.GetInstanceID() != hit.collider.gameObject.GetInstanceID()) {
					ally = hit.collider.GetComponent<UnitController>();
				}
			}

			if (ally != null) nextAlly = ally;
		} else nextAlly = null;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void CheckMove() {
		if (nextAlly != null) {
			float p1 = nextAlly.transform.localPosition.x + ((nextAlly.GetWidth() / 2f) * -direction);
			float p2 = transform.localPosition.x + ((GetWidth() / 2f) * direction);
			stopMoving = (Mathf.Abs(p1 - p2) >= 0 + FRIENDLY_OVERLAP);
		}
		else if (nextEnemy != null) stopMoving = true;
		else stopMoving = false;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void CheckAttack() {
		if (nextEnemy != null) {
			if (nextEnemy.GetOwnerID() != unitOwner.PlayerID) {
				attackCounter -= Time.deltaTime;

				if (attackCounter <= 0) {
					attackCounter = GetNextAttackSpeed();

					Attack();
				}
			}
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void FixedUpdate() {
		if (Game.Current.Freeze) return;

		if (!stopMoving) {
			transform.Translate(moveSpeed * modifiers.MoveSpeed * direction * Time.fixedDeltaTime, 0, 0);
		}

		animator.SetBool("isWalking", !stopMoving);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	//Causes sprite to shake - Used for recording purposes
	IEnumerator Shake() {
        transform.position = transform.position + new Vector3(0, 0.1f);
        transform.Rotate(0, 0, 10);
        yield return new WaitForSeconds(0.01f);
        transform.position = transform.position + new Vector3(0, -0.1f);
        transform.Rotate(0, 0, -10);
        yield return new WaitForSeconds(0.01f);
        transform.position = transform.position + new Vector3(0, -0.1f);
        transform.Rotate(0, 0, 10);
        yield return new WaitForSeconds(0.01f);
        transform.position = transform.position + new Vector3(0, 0.1f);
        transform.Rotate(0, 0, -10);
        yield return new WaitForSeconds(0.01f);
        transform.position = transform.position + new Vector3(0, 0.1f);
        transform.Rotate(0, 0, 10);
        yield return new WaitForSeconds(0.01f);
        transform.position = transform.position + new Vector3(0, -0.1f);
        transform.Rotate(0, 0, -10);
        yield return new WaitForSeconds(0.01f);

    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void TakeDamage(int damage, Player sender) {
		if (health <= 0) return;

		health -= damage;

		
		if (health <= 0) {
			sender.ChangeDNA(giveDNA);
			Die();
			return;
		} else {
			StartCoroutine(Shake());
		}

        healthBar.transform.localScale = new Vector3(
            ((float)health / 100) * healthBar.transform.localScale.x,
            healthBar.transform.localScale.y,
            healthBar.transform.localScale.z);
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Die() {
		health = 0;

		Destroy(gameObject);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public virtual void Attack() {
		int d = GetNextDamage();
		nextEnemy.TakeDamage(d, unitOwner);
		SoundManagerScript.PlayUnitSound(Type + "_Attack");
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public int GetHealth() => health;
    public void SetHealth(int newHealthValue) => health = newHealthValue;
    public Player GetUnitOwner() => unitOwner;
    public void SetUnitOwner(Player owner) => unitOwner = owner;
    public int GetUnitDamage() => damage;
    public void SetUnitDamage(int damage) => this.damage = damage;
    public float GetUnitSpeed() => moveSpeed;
    public void SetUnitSpeed(float speed) => this.moveSpeed = speed;
	public int GetOwnerID() => unitOwner.PlayerID;
	public new int GetInstanceID() => gameObject.GetInstanceID();
	public float GetWidth() => collider.size.x;
	public int GetNextDamage() => (int)(damage * modifiers.Damage);
	float GetNextAttackSpeed() => UnityEngine.Random.Range((attackInterval / modifiers.AttackSpeed) - attackIntervalDeviation, (attackInterval / modifiers.AttackSpeed) + attackIntervalDeviation);
	public Transform GetTransform() => transform;
	public bool IsDead() => health <= 0;


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void SetDirection(int dir) {
		if (dir == 0) dir = 1;
		else dir = Math.Sign(dir);

		direction = dir;
		sprite.flipX = dir == -1;

		gameObject.layer = LayerMask.NameToLayer(direction == 1 ? "Left Unit" : "Right Unit");

		if (direction == 1) {
			enemyMask = LayerMask.GetMask("Right Unit", "Right Base");
			allyMask = LayerMask.GetMask("Left Unit");
		} else {
			enemyMask = LayerMask.GetMask("Left Unit", "Left Base");
			allyMask = LayerMask.GetMask("Right Unit");
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public static int GetUnitBaseCost(UnitType type) {
		switch(type) {
			case UnitType.Worker: return WORKER_DNA_COST;
			case UnitType.Soldier: return SOLDIER_DNA_COST;
			case UnitType.Spitter: return SPITTER_DNA_COST;
			case UnitType.Defender: return DEFENDER_DNA_COST;
		}
		return 0;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public static string GetUnitPrefabPath(UnitType type) {
		switch (type) {
			case UnitType.Worker: return WORKER_PREFAB_PATH;
			case UnitType.Soldier: return SOLDIER_PREFAB_PATH;
			case UnitType.Spitter: return SPITTER_PREFAB_PATH;
			case UnitType.Defender: return DEFENDER_PREFAB_PATH;
		}
		return "";
	}


	//----------------------------------------------------------------------------------------------------------------------------------<





	//----------------------------------------------------------------------------------------------------------------------------------<


	#region Legacy Code
	/*


	private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerUnit")
        {
            Debug.Log(gameObject.name + " Collided with " + collision.name);
            //unitOwner.SetUnitsAreMoving(false);
            //StartCoroutine("AttackEnemy", collision);
        }
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	private IEnumerator AttackEnemy(Collider2D collision)
    {
        UnitController enemyController = collision.GetComponent<UnitController>();
        float nextHitTime = Time.time;

        while (health > 0 && enemyController.GetHealth() > 0)
        {

            if (Time.time >= nextHitTime)
            {
                enemyController.SetHealth(enemyController.GetHealth() - damage);
                nextHitTime += attackInterval;
                //Shakes unit every attack
                StartCoroutine(Shake());
            }
            yield return null;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
            unitOwner.AddDNA(gameObject.GetComponent<UnitController>().giveDNA);
        }

        unitOwner.SetUnitsAreMoving(true);
        yield return null;
    }


	*/
	#endregion
}



public enum UnitType {
	Worker = 0,
	Soldier = 1,
	Spitter = 2,
	Defender = 3
}



[System.Serializable]
public class UnitModifiers {

	[Header("General")]
	public float MoveSpeed = 1;
	public float Damage = 1;
	public float Health = 1;
	public float AttackSpeed = 1;
	public float GiveDNA = 1;

	[Space]

	[Header("For Spitters Only")]
	public bool HitTwoEnemies;

}