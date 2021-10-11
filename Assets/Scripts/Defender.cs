using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : UnitController {

	const float KAMIKAZE_PERCENT = 0.3f;

	public override void OnDeath() {
		base.OnDeath();

		UnitModifiers mod = GetUnitOwner().GetModifierReference(Type);

		if(mod.Kamikaze) {
			float a = GetWidth() / 2 * Direction;

			RaycastHit2D hit;
			Vector3 origin = transform.localPosition + (Vector3.right * a);
			hit = Physics2D.Raycast(origin, Vector3.right * Direction, GetAttackRange(), GetEnemyMask());

			if(hit.collider != null) {
				Damageable enemy = hit.collider.GetComponent<Damageable>();

				int damage = (int)(GetMaxHealth() * KAMIKAZE_PERCENT);

				if (enemy != null) enemy.TakeDamage(damage, GetUnitOwner());
			}
		}
	}

}
