using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	int damage;
	float moveSpeed = 3;
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
		transform.Translate(Vector3.right * direction * moveSpeed * Time.fixedDeltaTime);
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

	}

}
