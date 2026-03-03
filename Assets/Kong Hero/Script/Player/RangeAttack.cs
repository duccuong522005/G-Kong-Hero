using UnityEngine;
using System.Collections;

public class RangeAttack : MonoBehaviour {
	
	public Transform FirePoint;
	public Projectile Projectile;
	[Tooltip("fire projectile after this delay, useful to sync with the animation of firing action")]
	public float fireDelay;
	public float fireRate;
	public bool inverseDirection = false;

	float nextFire = 0;

	public bool Fire(){
		if (GameManager.Instance.Bullet>0 && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			GameManager.Instance.Bullet--;
			StartCoroutine (DelayAttack (fireDelay));
			return true;
		} else
			return false;
	}

	IEnumerator DelayAttack(float time){
		yield return new WaitForSeconds (time);

		var direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

		if (inverseDirection)
			direction *= -1;

		var projectile = (Projectile) Instantiate (Projectile, FirePoint.position, Quaternion.identity);

		projectile.Initialize (gameObject, direction, Vector2.zero);
	}
}
