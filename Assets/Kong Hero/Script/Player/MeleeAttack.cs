using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour {
	[Tooltip("What layers should be hit")]
	public LayerMask CollisionMask;
	[Tooltip("Hit more than one enemy at the same time")]
	public bool multiDamage = false;
	[Tooltip("Give damage to the enemy or object")]
	public float damageToGive;
	[Tooltip("Apply the force to enemy if they are hit, only for Rigidbody object")]
	public Vector2 pushObject;
	public Transform MeleePoint;
	public float areaSize;

	public float attackRate = 0.2f;
	[Tooltip("Check target in range after a delay time, useful to sync the right attack time of the animation")]
	public float attackAfterTime = 0.15f;

	float nextAttack = 0;


	public bool Attack(){
		if (Time.time > nextAttack) {
			nextAttack = Time.time + attackRate;
			StartCoroutine (CheckTargetCo (attackAfterTime));
			return true;
		} else
			return false;
	}

	IEnumerator CheckTargetCo(float delay){
		yield return new WaitForSeconds (delay);

		var hits = Physics2D.CircleCastAll (MeleePoint.position, areaSize, Vector2.zero,0,CollisionMask);

		if (hits == null)
			yield break;

		foreach (var hit in hits) {
			var damage = (ICanTakeDamage) hit.collider.gameObject.GetComponent (typeof(ICanTakeDamage));
			if (damage == null)
				continue;

			damage.TakeDamage (damageToGive,pushObject, gameObject);
			if (!multiDamage)
				break;

		}
	}

	void OnDrawGizmos(){
		if (MeleePoint == null)
			return;
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (MeleePoint.position, areaSize);
	}
}
