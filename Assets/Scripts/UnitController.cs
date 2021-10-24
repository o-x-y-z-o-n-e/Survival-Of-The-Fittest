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

	public const string WORKER_SPRITE_PATH = "Sprites/Units/Worker";
	public const string SOLDIER_SPRITE_PATH = "Sprites/Units/Soldier";
	public const string SPITTER_SPRITE_PATH = "Sprites/Units/Spitter";
	public const string DEFENDER_SPRITE_PATH = "Sprites/Units/Defender";


	public const int	WORKER_DNA_COST			= 25;
	public const int	SOLDIER_DNA_COST		= 120;
	public const int	SPITTER_DNA_COST		= 100;
	public const int	DEFENDER_DNA_COST		= 150;


	const float			FRIENDLY_OVERLAP		= 0.0f;
	const float			ALLY_CHECK_DIST			= 0.1f;

	const float BLOODLUST_THRESHOLD = 0.15f; //What percent of total health does a Unit have to reach below for bloodlust to activate.
	const float BLOODLUST_DAMAGE_MULTIPLIER = 2f;
	const float BLOODLUST_MOVE_MULTIPLIER = 1f;


	public const float CRITICAL_DAMAGE_MULTIPLIER = 3f;


	const float ATTACK_LUNGE_TIME = 0.25f;
	const float ATTACK_LUNGE_WIDTH = 0.25f;
	const float ATTACK_LUNGE_HEIGHT = 0.125f;

	const float PUNCH_ICON_TIME = 0.35f;
	const float PUNCH_RANGE = 0.5f;


	public UnitType Type;


	[SerializeField] private float attackRange = 0.2f;
    [SerializeField] private int direction; public int Direction => direction;
    [SerializeField] private int baseHealth;
    [SerializeField] private int damage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackInterval;
	[SerializeField] private float attackIntervalDeviation;
	[SerializeField] private int giveDNA; // how much DNA the unit will give when killed
	[SerializeField] private Sprite[] sprites;
	[SerializeField] private GameObject[] statusEffect;
	[SerializeField] private Transform punchIcon;
	[SerializeField] private SpriteRenderer spriteBody;


	private Player unitOwner; //this will not be a serialized field once unit owners are assigned at time of prefab instantiation.

	SpriteRenderer punchIconSprite;
	SpriteRenderer[] spriteRenderers;
	Animator animator;
	new BoxCollider2D collider;
	UnitModifiers modifiers; public UnitModifiers Modifiers => modifiers;
	Image healthBar;

	float attackSpeedBuff = 1f;
	bool stopMoving;
	float attackCounter;
	int health;
	bool bloodlust;
	bool stunNextAttack = false;
	bool isStunned = false;

	int enemyMask;
	int allyMask;

	Path path; //0 = Surface, 1 = Tunnel


	UnitController nextAlly;
	Damageable nextEnemy;


	Vector2 hitOffset = Vector2.zero;
	float hitRotate;
	float bloodlustRotate;
	float attackLungeCounter;
	float attackLungeHeightMulitplier = 1;

	float punchIconCounter = 0;


	//----------------------------------------------------------------------------------------------------------------------------------<


	// Start is called before the first frame update
	void Awake() {
		animator = GetComponentInChildren<Animator>();
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		collider = GetComponent<BoxCollider2D>();
		healthBar = GetComponentsInChildren<Image>()[1];
		punchIconSprite = punchIcon.GetComponentInChildren<SpriteRenderer>();

		if (unitOwner != null) SetPlayer(unitOwner);
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	void OnDestroy() {
		unitOwner.Base.RemoveUnit(this);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void SetPlayer(Player player) {
		unitOwner = player;
		modifiers = unitOwner.GetModifierReference(Type);

		health = (int)(baseHealth * modifiers.Health);

		if (modifiers.StunNextAttackAcquired == true) stunNextAttack = true;

		foreach (SpriteRenderer sprite in spriteRenderers)
			sprite.color = player.Color;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	// Update is called once per frame
	public virtual void Update() {
		if (Game.Current.Freeze) return;

		CheckCollision();
		CheckMove();
		CheckAttack();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void CheckCollision() {
		//Check for next enemy
		RaycastHit2D hit;
		Vector3 origin = transform.localPosition + (Vector3.right * (collider.size.x / 2f) * direction);
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
		origin += (Vector3.right * 0.05f * direction);
		hit = Physics2D.Raycast(origin, Vector3.right * direction, ALLY_CHECK_DIST, allyMask);

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
				attackCounter -= Time.deltaTime * attackSpeedBuff;
				
				if (attackCounter <= 0 && !isStunned)
				{
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
			float velocity = moveSpeed * modifiers.MoveSpeed * direction * Time.fixedDeltaTime;

			//Current BLOODLUST_MOVE_MULTIPLIER is set to '1' added this in case we wanted it.
			if (bloodlust) velocity *= BLOODLUST_MOVE_MULTIPLIER;

			transform.Translate(velocity, 0, 0);
		}

		MoveSprite();

		animator.SetBool("isWalking", !stopMoving);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	//Causes sprite to shake - Used for recording purposes
	IEnumerator Shake() {
        hitOffset = new Vector3(0, 0.1f);
		hitRotate = 10;
        yield return new WaitForSeconds(0.01f);
		hitOffset = new Vector3(0, -0.1f);
		hitRotate = -10;
		yield return new WaitForSeconds(0.01f);
		hitOffset = new Vector3(0, 0.1f);
		hitRotate = 10;
		yield return new WaitForSeconds(0.01f);
		hitOffset = new Vector3(0, -0.1f);
		hitRotate = -10;
		yield return new WaitForSeconds(0.01f);
		hitOffset = new Vector3(0, 0.1f);
		hitRotate = 10;
		yield return new WaitForSeconds(0.01f);
		hitOffset = new Vector3(0, -0.1f);
		hitRotate = -10;
		yield return new WaitForSeconds(0.01f);

		hitOffset = Vector2.zero;
		hitRotate = 0;
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void Heal(int amount) {
		health += Mathf.Abs(amount);

		if (health > baseHealth) health = baseHealth;

		UpdateHealthBar();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<
	

	public bool TakeDamage(int damage, Player sender) {
		if (health <= 0) return false;

		float blockRange = UnityEngine.Random.Range(0f, 1f);
		bool blockEnemy = false;
		if (modifiers.BlockEnemy)
			blockEnemy = blockRange > 1 - modifiers.BlockEnemyChance;

		if (blockEnemy)
		{
			StartCoroutine(CritDefendVisual());
			return false;
		}

		int d = (int)(damage * (1f - modifiers.Armor));
		health -= d;


		

		if (modifiers.Bloodlust) {
			float t = health / baseHealth;

			//Activate bloodlust if helath is below threshold
			if (t <= BLOODLUST_THRESHOLD)
			{
				statusEffect[0].SetActive(true);
				bloodlust = true;
			}
		}

		
		if (health <= 0) {
			sender.ChangeDNA(giveDNA);
			Die();
			return true;
		} else {
			StartCoroutine(Shake());
		}

		UpdateHealthBar();

		return false;
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Die() {
		health = 0;

		OnDeath();

		Destroy(gameObject);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void UpdateHealthBar() {
		healthBar.rectTransform.sizeDelta = new Vector2(((float)health / baseHealth)*100, 10);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public virtual void Attack() {
		int d = GetNextDamage();

		/*Activate attack lunge movement*/ {
			attackLungeCounter = ATTACK_LUNGE_TIME;
			//This might seem a bit hacky. Basically, the bounds for lunge height is perfect, but I wanted variation around that value.
			//with rand(-1, 1) too many values where close to 0. I changed the chances gravitate towards +- 1 by having a linear range of [-2, 2] and clamping to [-1, 1]
			attackLungeHeightMulitplier = Mathf.Clamp(UnityEngine.Random.Range(-2f, 2f), -1f, 1f);
		}

		if (TryCritical()) {
			d = (int)(d * CRITICAL_DAMAGE_MULTIPLIER);

			punchIconCounter = PUNCH_ICON_TIME;
			punchIcon.transform.localPosition = Vector3.zero;
			punchIcon.transform.gameObject.SetActive(true);

			StartCoroutine(CritHitVisual());
		}

		if (nextEnemy.TakeDamage(damage, unitOwner)) {
			unitOwner.ChangeDNA(modifiers.ExtraDNAHarvest);
		}

		if(nextEnemy.GetType() == typeof(Base))
		{
			Base nextBase = (Base)nextEnemy;
			TakeDamage((int)(baseHealth * nextBase.ReflectedDamage), nextBase.Player);
		}

		if (stunNextAttack == true) {
			StartCoroutine(StunEnemy(nextEnemy));
			stunNextAttack = false;
		}
		
		SoundManagerScript.PlaySound(Type + "_Attack");
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	private IEnumerator StunEnemy(Damageable unit)
	{
		UnitController unitToStun = (UnitController)unit;

		unitToStun.SetStunned(true);
		yield return new WaitForSeconds(3);
		if (unitToStun)
		{
			unitToStun.SetStunned(false);
		}
		yield return null;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<
	
	
	private IEnumerator CritHitVisual()
	{
		statusEffect[1].SetActive(true);
		yield return new WaitForSeconds(0.7f);
		statusEffect[1].SetActive(false);

	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	private IEnumerator CritDefendVisual()
	{
		statusEffect[3].SetActive(true);
		yield return new WaitForSeconds(0.7f);
		statusEffect[3].SetActive(false);

	}


	//----------------------------------------------------------------------------------------------------------------------------------<



	private void SetStunned(bool v)
	{
		StartCoroutine(StunnedVisual());
		isStunned = v;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	private IEnumerator StunnedVisual()
	{
		statusEffect[2].SetActive(true);
		yield return new WaitForSeconds(3);
		statusEffect[2].SetActive(false);

	}


	//----------------------------------------------------------------------------------------------------------------------------------<



	public int GetEnemyMask() => enemyMask;
	public float GetAttackRange() => attackRange;
	public int GetMaxHealth() => baseHealth;
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
	float GetNextAttackSpeed() => UnityEngine.Random.Range((attackInterval / modifiers.AttackSpeed) - attackIntervalDeviation, (attackInterval / modifiers.AttackSpeed) + attackIntervalDeviation);
	public Transform GetTransform() => transform;
	public bool IsDead() => health <= 0;
	public bool TryCritical() => UnityEngine.Random.value <= modifiers.CriticalChance;
	public void SetPath(Path path) => this.path = path;
	public Path GetPath() => path;


	//----------------------------------------------------------------------------------------------------------------------------------<


	public int GetNextDamage() {
		float d = damage * modifiers.Damage;

		if (bloodlust) d *= BLOODLUST_DAMAGE_MULTIPLIER;

		return (int)d;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void SetDirection(int dir) {
		if (dir == 0) dir = 1;
		else dir = Math.Sign(dir);

		direction = dir;
		foreach (SpriteRenderer sprite in spriteRenderers)
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


	/// <summary>
	/// Sets the units' sprite to the sprite of the given evolution
	/// </summary>
	/// <param name="unitType"></param>
	/// <param name="evolution">The evolution, which the player has evolved to</param>
	public void SetSprite(int evolution)
    {
		if (evolution < sprites.Length) spriteBody.sprite = sprites[evolution];

		/*
		Sprite newSprite = null;
		switch (Type)
        {
			case UnitType.Soldier:
				if (evolution < Game.Current.SoldierSprites.Count)
				{
					newSprite = Game.Current.SoldierSprites[evolution];
				}
				break;
			case UnitType.Spitter:
				if (evolution < Game.Current.SpitterSprites.Count)
				{
					Debug.Log("Spitter sprite changed!");
					newSprite = Game.Current.SpitterSprites[evolution];
				}
				break;
			case UnitType.Defender:
				if (evolution < Game.Current.DefenderSprites.Count)
				{
					newSprite = Game.Current.DefenderSprites[evolution];
				}
				break;
		}

		Debug.Log(newSprite);

		if (newSprite)
		{
			spriteBody.sprite = newSprite;
		}
		*/
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// This is for moving/rotating the unit sprite. Used to illustrate effects like bloodlust.
	/// </summary>
	void MoveSprite() {
		Vector2 offset = Vector2.zero;
		float rotation = 0;

		//Visualize bloodlust by 'shaking' back and forth.
		if (bloodlust) {
			bloodlustRotate += Time.fixedDeltaTime * 25f;
			float r = Mathf.Sin(bloodlustRotate) * 7f;

			rotation += r;
		}


		if(attackLungeCounter > 0) {
			attackLungeCounter -= Time.fixedDeltaTime;

			if(attackLungeCounter <= 0) {
				attackLungeCounter = 0;
			}

			float t = 1f - (attackLungeCounter / ATTACK_LUNGE_TIME);

			float x = Mathf.Sin(t * Mathf.PI) * ATTACK_LUNGE_WIDTH * direction;
			float y = Mathf.Sin(Mathf.Clamp(t, 0, 0.5f) * 2 * Mathf.PI) * ATTACK_LUNGE_HEIGHT * attackLungeHeightMulitplier;

			offset += new Vector2(x, y);
		}


		if(punchIconCounter > 0) {
			punchIconCounter -= Time.fixedDeltaTime;

			if(punchIconCounter <= 0) {
				punchIconCounter = 0;

				punchIcon.gameObject.SetActive(false);
			}

			float t = 1f - (punchIconCounter / PUNCH_ICON_TIME);
			float x = 1f - Mathf.Pow(1f-t, 3);
			float a = 1f - Mathf.Pow(t, 9);

			punchIcon.transform.localScale = new Vector3(direction, 1, 1);
			punchIcon.transform.localPosition = Vector3.right * x * PUNCH_RANGE * direction;
			punchIconSprite.color = new Color(1, 1, 1, a);
		}


		offset += hitOffset;
		rotation += hitRotate;


		animator.transform.localPosition = offset;
		animator.transform.localRotation = Quaternion.Euler(0, 0, rotation);
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


	public static string GetUnitSpritePath(UnitType type)
	{
		switch (type)
		{
			case UnitType.Worker: return WORKER_SPRITE_PATH;
			case UnitType.Soldier: return SOLDIER_SPRITE_PATH;
			case UnitType.Spitter: return SPITTER_SPRITE_PATH;
			case UnitType.Defender: return DEFENDER_SPRITE_PATH;
		}
		return "";
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public virtual void OnDeath() {
		SoundManagerScript.PlaySound(Type + "_Death");
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void SetAttackSpeedBuff(float multiplier) => attackSpeedBuff = multiplier;
	

}


//----------------------------------------------------------------------------------------------------------------------------------<


public enum UnitType {
	Worker = 0,
	Soldier = 1,
	Spitter = 2,
	Defender = 3
}


//----------------------------------------------------------------------------------------------------------------------------------<


[System.Serializable]
public class UnitModifiers {

	[Header("General")]
	public float MoveSpeed = 1;
	public float Damage = 1;
	public float Health = 1;
	[Range(0, 1)] public float Armor = 0;
	[Range(0, 1)] public float CriticalChance = 0.08f;
	public float AttackSpeed = 1;
	public int ExtraDNAHarvest = 0;
	

	[Space]

	[Header("For Spitters Only")]
	public int RangedPassCount = 1;
	public bool HealFrontAlly;
	public bool BuffFrontAlly;

	[Space]

	[Header("For Soldiers Only")]
	public bool Bloodlust;
	public bool StunNextAttackAcquired = false;

	[Space]

	[Header("For Defenders Only")]
	public bool Kamikaze;
	public bool BlockEnemy;
	public float BlockEnemyChance = 0f;

}