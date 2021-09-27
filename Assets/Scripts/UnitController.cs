using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitController : MonoBehaviour {

	const float ATTACK_EXTEND = 0.2f;
	const float FRIENDLY_OVERLAP = 0.0f;

	public UnitType Type;

    [SerializeField] private int direction;
    [SerializeField] private int baseHealth;
    [SerializeField] private int damage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackInterval;
	[SerializeField] private float attackIntervalDeviation;
	[SerializeField] private int giveDNA; // how much DNA the unit will give when killed

    [SerializeField] private Player unitOwner; //this will not be a serialized field once unit owners are assigned at time of prefab instantiation.

	new BoxCollider2D collider;
    Text healthText;
	UnitModifiers modifiers;

	bool stopMoving;
	float attackCounter;
	int health;

	UnitController frontUnit;


	//----------------------------------------------------------------------------------------------------------------------------------<


	// Start is called before the first frame update
	void Awake() {
		collider = GetComponent<BoxCollider2D>();
        healthText = GetComponentInChildren<Text>();
		modifiers = unitOwner.GetModifierReference(Type);

		health = (int)(baseHealth * modifiers.Health);
		healthText.text = health.ToString();
    }

    //----------------------------------------------------------------------------------------------------------------------------------<


    // Update is called once per frame
    void Update() {
		if (Game.Current.IsFinished) return;

		CheckCollision();
		CheckMove();
		CheckAttack();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void CheckCollision() {
		RaycastHit2D hit;
		Vector3 origin = transform.localPosition + (Vector3.right * collider.size.x * direction);
		hit = Physics2D.Raycast(origin, Vector3.right * direction, ATTACK_EXTEND);

		if (hit.collider != null) {
			if (frontUnit == null) {
				UnitController unit = hit.collider.GetComponent<UnitController>();
				if (unit != null) {
					frontUnit = unit;
					attackCounter = GetNextAttackSpeed();
				}
			} else {
				if (frontUnit.gameObject.GetInstanceID() != hit.collider.gameObject.GetInstanceID()) {
					UnitController unit = hit.collider.GetComponent<UnitController>();
					if (unit != null) {
						frontUnit = unit;
						attackCounter = GetNextAttackSpeed();
					}
				}
			}
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void CheckMove() {
		if (frontUnit != null) {
			if (frontUnit.GetPlayerID() != unitOwner.PlayerID) stopMoving = true;
			else {
				float p = (frontUnit.GetWidth() + GetWidth()) / 2;
				stopMoving = (Mathf.Abs(frontUnit.transform.localPosition.x - transform.localPosition.x) <= p - FRIENDLY_OVERLAP);
			}
		} else stopMoving = false;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void CheckAttack() {
		if (frontUnit == null) return;

		if(frontUnit.GetPlayerID() != unitOwner.PlayerID) {
			attackCounter -= Time.deltaTime;

			if(attackCounter <= 0) {
				attackCounter = GetNextAttackSpeed();

				int d = (int)(damage * modifiers.Damage);
				frontUnit.TakeDamage(d, this);
				SoundManagerScript.PlayUnitSound(Type + "_Attack");
			}
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void FixedUpdate() {
		if (Game.Current.IsFinished) return;

		if (!stopMoving) transform.Translate(moveSpeed * modifiers.MoveSpeed * direction * Time.fixedDeltaTime, 0, 0);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	//Causes sprite to shake - Used for recording purposes
	IEnumerator Shake()
    {
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


	public void TakeDamage(int damage, UnitController sender) {
		if (health <= 0) return;

		health -= damage;

		
		if (health <= 0) {
			sender.GiveDNA(giveDNA);
			Die();
			return;
		} else {
			StartCoroutine(Shake());
		}

		healthText.text = health.ToString();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void GiveDNA(int amount) {

	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Die() {
		health = 0;

		healthText.gameObject.SetActive(false);

		Destroy(gameObject);
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
	public int GetPlayerID() => unitOwner.PlayerID;
	public float GetWidth() => collider.size.x;
	float GetNextAttackSpeed() => UnityEngine.Random.Range((attackInterval / modifiers.AttackSpeed) - attackIntervalDeviation, (attackInterval / modifiers.AttackSpeed) + attackIntervalDeviation);


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
	All,
	Worker,
	Soldier,
	Spitter,
	Defender
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