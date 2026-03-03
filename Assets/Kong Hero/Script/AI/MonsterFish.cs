using UnityEngine;
using System.Collections;

public class MonsterFish : MonoBehaviour, ICanTakeDamage, IPlayerRespawnListener {
	public Vector2 Attackdirection;
	public float AttackForce = 750;

	public float delayAttack = 0.35f;
	public AudioClip soundAttack;
	public AudioClip soundDead;
	public GameObject deadFx;
	public int scoreRewarded = 200;

	Vector3 oldPosition;
	float rotation;
	Rigidbody2D rig;

	void Start(){
		rig = GetComponent<Rigidbody2D> ();
		oldPosition = transform.position;
		rotation = -Vector2.Angle (Attackdirection, Vector2.left);
	}

	public void Attack(){
		transform.Rotate (Vector3.forward, rotation);
		StartCoroutine (WaitAndAttack (delayAttack));
	}

	IEnumerator WaitAndAttack(float time){
		yield return new WaitForSeconds (time);
		SoundManager.PlaySfx (soundAttack);
		rig.isKinematic = false;
//		transform.Rotate (Vector3.forward, rotation);
		rig.AddRelativeForce(new Vector2(-AttackForce,0));
//		GetComponent<Rigidbody2D> ().velocity = forceAttack * forceMul;
	}

	public void Dead(){
		SoundManager.PlaySfx(soundDead);
		GameManager.Instance.AddPoint(scoreRewarded);
		if (deadFx != null)
			Instantiate (deadFx, transform.position, Quaternion.identity);

		rig.linearVelocity = Vector2.zero;

		//turn off all colliders if the enemy have
		var boxCo = GetComponents<BoxCollider2D> ();
		foreach (var box in boxCo) {
			box.enabled = false;
		}
		var CirCo = GetComponents<CircleCollider2D> ();
		foreach (var cir in CirCo) {
			cir.enabled = false;
		}
	}

//	void OnTriggerEnter2D(Collider2D other){
//		if (isAttack) {
//			if (other.CompareTag ("Player")) {
//				Dead ();
//				//Push player up
//				other.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
//				other.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, 300f));
//			}
//		}
//	}
//
//
//	void OnCollisionEnter2D(Collision2D other){
//		if (isAttack) {
//			if (other.gameObject.CompareTag ("Player")) {
//				LevelManager.Instance.KillPlayer ();
//			}
//		}
//	}


	public void TakeDamage (float damage, Vector2 force, GameObject instigator)
	{
		Dead ();
	}

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		transform.position = oldPosition;
		transform.rotation = Quaternion.Euler (0, 0, 0);
		gameObject.SetActive (true);

		//turn on all colliders if the enemy have
		var boxCo = GetComponents<BoxCollider2D> ();
		foreach (var box in boxCo) {
			box.enabled = true;
		}
		var CirCo = GetComponents<CircleCollider2D> ();
		foreach (var cir in CirCo) {
			cir.enabled = true;
		}
	}

	void OnDrawGizmosSelected(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay (transform.position, Attackdirection);
	}

}