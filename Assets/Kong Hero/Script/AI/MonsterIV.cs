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
		if (line == null || linePoint == null) {
			return;
		}

		line.SetPosition (0, linePoint.position);
		line.SetPosition (1, transform.position);
	}

	public void Dead(){
		SoundManager.PlaySfx(soundDead);
		GameManager.Instance.AddPoint(scoreRewarded);

		if (deadFx != null) {
			Instantiate (deadFx, transform.position, Quaternion.identity);
		}

		SetCollidersEnabled(false);

		if (springJoint != null) {
			springJoint.enabled = false;
		}
		if (line != null) {
			line.enabled = false;
		}
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

		SetCollidersEnabled(true);

		rig.isKinematic = true;
		if (springJoint != null) {
			springJoint.enabled = true;
		}
		if (line != null) {
			line.enabled = true;
		}
	}

	private void SetCollidersEnabled(bool enabled) {
		var boxColliders = GetComponents<BoxCollider2D> ();
		foreach (var box in boxColliders) {
			box.enabled = enabled;
		}

		var circleColliders = GetComponents<CircleCollider2D> ();
		foreach (var circle in circleColliders) {
			circle.enabled = enabled;
		}
	}
}