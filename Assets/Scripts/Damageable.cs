using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damagable {

	Transform GetTransform();
	float GetWidth();
	int GetOwnerID();
	int GetInstanceID();
	void TakeDamage(int damage, UnitController sender);

}
