using UnityEngine;
using System.Collections;

public class MonsterIV : MonoBehaviour, ICanTakeDamage, IPlayerRespawnListener {
	public AudioClip soundDead;
	public GameObject deadFx;
	public int scoreRewarded = 200;
	public Transform linePoint;

	private LineRenderer line;
	Vector3 oldPosition;
	SpringJoint2D springJoint;
	Rigidbody2D rig;

	void Start(){
		line = GetComponent<LineRenderer> ();
		oldPosition = transform.position;
		springJoint = GetComponent<SpringJoint2D> ();
		rig = GetComponent<Rigidbody2D> ();
	}
	void Update(){
		line.SetPosition (0, linePoint.position);
		line.SetPosition (1, transform.position);
	}

	public void Dead(){
		SoundManager.PlaySfx(soundDead);
		GameManager.Instance.AddPoint(scoreRewarded);

		if (deadFx != null)
		Instantiate (deadFx, transform.position, Quaternion.identity);

		//turn off all colliders if the enemy have
		var boxCo = GetComponents<BoxCollider2D> ();
		foreach (var box in boxCo) {
			box.enabled = false;
		}
		var CirCo = GetComponents<CircleCollider2D> ();
		foreach (var cir in CirCo) {
			cir.enabled = false;
		}

		springJoint.enabled = false;
		line.enabled = false;
		rig.linearVelocity = Vector2.zero;
		rig.AddForce (new Vector2 (0, 300f));
	}

//	void OnTriggerEnter2D(Collider2D other){
//		if (other.CompareTag ("Player")) {
//			Dead ();
//			//Push player up
//			other.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
//			other.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, 300f));
//		}
//	}
//
//
//	void OnCollisionEnter2D(Collision2D other){
//		if (other.gameObject.CompareTag ("Player")) {
//			LevelManager.Instance.KillPlayer ();
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

		rig.isKinematic = true;
		springJoint.enabled = true;
		line.enabled = true;
	}
}