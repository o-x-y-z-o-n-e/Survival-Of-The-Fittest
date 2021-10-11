using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : UnitController {

	const float HEAL_ALLY_INTERVAL = 1f; //Also doubles for checking if front ally is in range.
	const float HEAL_ALLY_RANGE = 4f;
	const float HEAL_ICON_HEIGHT = 1.5f;

	public GameObject HealthPackagePrefab;

	public Projectile ProjectilePrefab;
	public Vector2 ProjectileSpawnOffset;


	[Space]

	public int HealAmount = 0;


	UnitController allyTarget;
	Transform healthPackage;


	float checkAllyCounter = 0;
	float packageCounter = 0;


	public override void Attack() {
		//base.Attack();


		SoundManagerScript.PlayUnitSound(Type + "_Attack");

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



	public override void Update() {
		if (Game.Current.Freeze) return;
		base.Update();


		//Heal front ally evolution
		if(Modifiers.HealFrontAlly) HealAlly();
	}



	void HealAlly() {
		if (allyTarget != null) {
			float d = Mathf.Abs(allyTarget.transform.localPosition.x - transform.localPosition.x);

			if (d > HEAL_ALLY_RANGE) {
				allyTarget = null;

				if (healthPackage != null) {
					Destroy(healthPackage.gameObject);
					healthPackage = null;
				}

				return;
			}


			if (healthPackage != null) {
				Vector2 p1 = transform.localPosition;
				Vector2 p2 = allyTarget.transform.localPosition;

				Vector2 w = new Vector2(Mathf.Lerp(p1.x, p2.x, 0.5f), ((p1.y + p2.y) / 2f) + HEAL_ICON_HEIGHT);

				packageCounter += Time.deltaTime;

				if (packageCounter >= 1) {
					packageCounter = 0;

					allyTarget.Heal(HealAmount);

					Destroy(healthPackage.gameObject);
					healthPackage = null;
					return;
				}

				float t = packageCounter;

				Vector2 l1 = Vector2.Lerp(p1, w, t);
				Vector2 l2 = Vector2.Lerp(w, p2, t);

				Vector2 l3 = Vector2.Lerp(l1, l2, t);


				healthPackage.transform.localPosition = l3;

				return;
			}

			checkAllyCounter += Time.deltaTime;
			if (checkAllyCounter < HEAL_ALLY_INTERVAL) return;
			checkAllyCounter = 0;

			//wait to create another package
			healthPackage = Instantiate(HealthPackagePrefab).transform;
			healthPackage.localPosition = transform.localPosition;

			healthPackage.name = "Health";


		} else {
			if (healthPackage != null) {
				Destroy(healthPackage.gameObject);
				healthPackage = null;
			}


			//Find front ally
			checkAllyCounter += Time.deltaTime;
			if (checkAllyCounter < HEAL_ALLY_INTERVAL) return;
			checkAllyCounter = 0;

			Player player = GetUnitOwner();

			UnitController ally = player.Base.GetUnit(0, GetPath());

			if (ally != null) {
				if (ally == this) return;

				float d = Mathf.Abs(ally.transform.localPosition.x - transform.localPosition.x);

				if (d <= HEAL_ALLY_RANGE) {
					allyTarget = ally;
				}
			}
		}
	}

}
