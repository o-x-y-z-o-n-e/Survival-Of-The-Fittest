using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float MoveDistance = 5f;
	public float DamageDistance = 0.5f;
	public float MoveSpeed = 3;

	int collideCount = 1;
	int damage;
	bool critical;
	int direction = 1;
	LayerMask attackMask;

	SpriteRenderer sprite;
	Player sender;
	UnitType ownerType;

	HashSet<int> collided = new HashSet<int>();
	int collideCounter = 0;

	float startX;

	void Awake() {
		sprite = GetComponentInChildren<SpriteRenderer>();
		startX = transform.localPosition.x;
	}

	void Update() {
		CheckCollision();

		if(Mathf.Abs(transform.localPosition.x - startX) >= MoveDistance) Destroy(gameObject);
	}


	void FixedUpdate() {
		transform.Translate(Vector3.right * direction * MoveSpeed * Time.fixedDeltaTime);
	}


	public void SetDirection(int dir) {
		if (dir == 0) dir = 1;
		else dir = Math.Sign(dir);

		direction = dir;
		sprite.flipX = dir == -1;

		if(direction == 1)		attackMask = LayerMask.GetMask("Right Unit", "Right Base");
		else					attackMask = LayerMask.GetMask("Left Unit", "Left Base");
	}


	public void SetCollideNumber(int num) => collideCount = num;
	public void SetSender(Player sender) => this.sender = sender;
	public void SetDamage(int damage, bool critical) {
		this.damage = damage;
		this.critical = critical;
	}
	public void SetUnitSenderType(UnitType type) => ownerType = type;
	


	void CheckCollision() {
		RaycastHit2D hit;
		Vector3 origin = transform.localPosition;
		hit = Physics2D.Raycast(origin, Vector3.right * direction, DamageDistance, attackMask);

		if (hit.collider != null) {
			if (!collided.Contains(hit.collider.GetInstanceID())) {
				collided.Add(hit.collider.GetInstanceID());

				Damageable enemy = hit.collider.GetComponent<Damageable>();
				if (enemy != null) {
					if(enemy.TakeDamage(damage, sender)) {
						UnitModifiers mod = sender.GetModifierReference(ownerType);
						sender.ChangeDNA(mod.ExtraDNAHarvest);
					}
				}


				collideCounter++;
				if(collideCounter >= collideCount) {
					SoundManagerScript.PlaySound("Projectile_Hit");
					Destroy(gameObject);
				}
			}
		}

	}

}
