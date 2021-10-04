using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable {

	bool IsDead();
	Transform GetTransform();
	float GetWidth();
	int GetOwnerID();
	int GetInstanceID();
	void TakeDamage(int damage, Player sender);

}
