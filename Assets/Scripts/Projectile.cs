using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float DamageDistance = 0.5f;
	public float MoveSpeed = 3;
	
	int damage;
	int direction = 1;
	int attackMask;

	SpriteRenderer sprite;
	Player sender;

	void Awake() {
		sprite = GetComponentInChildren<SpriteRenderer>();
	}

	void Update() {
		CheckCollision();
	}


	void FixedUpdate() {
		transform.Translate(Vector3.right * direction * MoveSpeed * Time.fixedDeltaTime);
	}


	public void SetDirection(int dir) {
		if (dir == 0) dir = 1;
		else dir = Math.Sign(dir);

		direction = dir;
		sprite.flipX = dir == -1;

		attackMask = LayerMask.GetMask(direction == 1 ? "Right Unit" : "Left Unit");
	}


	public void SetSender(Player sender) => this.sender = sender;
	public void SetDamage(int damage) => this.damage = damage;


	void CheckCollision() {
		RaycastHit2D hit;
		Vector3 origin = transform.localPosition;
		hit = Physics2D.Raycast(origin, Vector3.right * direction, DamageDistance, attackMask);

		if(hit.collider != null) {
			Damageable enemy = hit.collider.GetComponent<Damageable>();
			if (enemy != null) {
				enemy.TakeDamage(damage, sender);
				Destroy(gameObject);
			}
		}
	}

}
