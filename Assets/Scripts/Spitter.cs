using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : UnitController {

	public Projectile ProjectilePrefab;
	public Vector2 ProjectileSpawnOffset;


	public override void Attack() {
		//base.Attack();

		Vector3 offset = ProjectileSpawnOffset;
		offset.x *= Direction;

		Projectile proj = Instantiate(ProjectilePrefab, transform.position + offset, Quaternion.identity, Game.Current.ProjectileContainer);
		proj.name = "Projectile";

		proj.SetSender(GetUnitOwner());
		proj.SetDirection(Direction);

		int damage = GetNextDamage();
		bool critical = TryCritical();
		if(critical) damage = (int)(damage * CRITICAL_DAMAGE_MULTIPLIER);
		
		proj.SetDamage(damage, critical);
	}

}
