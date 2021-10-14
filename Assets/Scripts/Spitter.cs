using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : UnitController {

	const float CHECK_ALLY_INTERVAL = 1f;
	const float BUFF_ALLY_INTERVAL = 1f; //Also doubles for checking if front ally is in range.
	const float BUFF_ALLY_RANGE = 4f;
	const float PACKAGE_ICON_HEIGHT = 1.5f;

	public GameObject HealthPackagePrefab;
	public GameObject AttackSpeedPackagePrefab;

	public Projectile ProjectilePrefab;
	public Vector2 ProjectileSpawnOffset;


	[Space]

	public int HealAmount = 0;
	[Range(0f, 1f)] public float AttackSpeedBuff = 0.25f;


	UnitController allyTarget;
	Transform healthPackage;
	Transform buffPackage;


	float checkAllyCounter = BUFF_ALLY_INTERVAL/2;
	float healAllyCounter = 0;
	float buffAllyCounter = 0;
	float healPackageCounter = 0;
	float buffPackageCounter = 0;


	//----------------------------------------------------------------------------------------------------------------------------------<


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


	//----------------------------------------------------------------------------------------------------------------------------------<


	public override void Update() {
		if (Game.Current.Freeze) return;
		base.Update();


		//Heal/Buff front ally evolution
		if (Modifiers.HealFrontAlly || Modifiers.BuffFrontAlly) {
			FindAlly();

			if (allyTarget != null) {
				if(Modifiers.HealFrontAlly) HealAlly();
				if (Modifiers.BuffFrontAlly) BuffAlly();
			}
			
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void FindAlly() {
		if (allyTarget == null) {
			if (healthPackage != null) {
				Destroy(healthPackage.gameObject);
				healthPackage = null;
			}

			if (buffPackage != null) {
				Destroy(buffPackage.gameObject);
				buffPackage = null;
			}


			//Find front ally
			checkAllyCounter += Time.deltaTime;
			if (checkAllyCounter < CHECK_ALLY_INTERVAL) return;
			checkAllyCounter = 0;

			Player player = GetUnitOwner();

			UnitController ally = player.Base.GetUnit(0, GetPath());

			if (ally != null) {
				if (ally == this) return;

				float d = Mathf.Abs(ally.transform.localPosition.x - transform.localPosition.x);

				if (d <= BUFF_ALLY_RANGE) {
					allyTarget = ally;
				}
			}
		} else {
			float d = Mathf.Abs(allyTarget.transform.localPosition.x - transform.localPosition.x);

			if (d > BUFF_ALLY_RANGE) {
				allyTarget = null;

				if (healthPackage != null) {
					Destroy(healthPackage.gameObject);
					healthPackage = null;
				}

				if (buffPackage != null) {
					Destroy(buffPackage.gameObject);
					buffPackage = null;
				}
			}
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void HealAlly() {
		if (healthPackage != null) {
			healPackageCounter += Time.deltaTime;

			if (healPackageCounter >= 1) {
				healPackageCounter = 0;

				allyTarget.Heal(HealAmount);

				Destroy(healthPackage.gameObject);
				healthPackage = null;
				return;
			}

			Vector2 p = Bezier(transform.localPosition, allyTarget.transform.localPosition, healPackageCounter);
			healthPackage.transform.localPosition = p;

			return;
		}

		healAllyCounter += Time.deltaTime;
		if (healAllyCounter < BUFF_ALLY_INTERVAL) return;
		healAllyCounter = 0;

		//wait to create another package
		healthPackage = Instantiate(HealthPackagePrefab).transform;
		healthPackage.localPosition = transform.localPosition;

		healthPackage.name = "Health";
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void BuffAlly() {
		if(buffPackage != null) {
			buffPackageCounter += Time.deltaTime;

			if(buffPackageCounter >= 1) {
				buffPackageCounter = 0;

				allyTarget.SetAttackSpeedBuff(1 + AttackSpeedBuff);

				Destroy(buffPackage.gameObject);
				buffPackage = null;
				return;
			}

			Vector2 p = Bezier(transform.localPosition, allyTarget.transform.localPosition, buffPackageCounter);
			buffPackage.transform.localPosition = p;

			return;
		}

		buffAllyCounter += Time.deltaTime;
		if (buffAllyCounter < BUFF_ALLY_INTERVAL) return;
		buffAllyCounter = 0;

		//wait to create another package
		buffPackage = Instantiate(AttackSpeedPackagePrefab).transform;
		buffPackage.localPosition = transform.localPosition;

		buffPackage.name = "Attack Speed";
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	static Vector2 Bezier(Vector2 p1, Vector2 p2, float t, float h = PACKAGE_ICON_HEIGHT) {
		Vector2 w = new Vector2(Mathf.Lerp(p1.x, p2.x, 0.5f), ((p1.y + p2.y) / 2f) + h);

		Vector2 l1 = Vector2.Lerp(p1, w, t);
		Vector2 l2 = Vector2.Lerp(w, p2, t);

		Vector2 l3 = Vector2.Lerp(l1, l2, t);

		return l3;
	}
}
