using UnityEngine;
using System.Collections;

public class BOSS_2 : MonoBehaviour,ICanTakeDamage {
	
	Animator anim;

	[Range(10,100)]
	public float health = 100f;
	public float damagePerHit = 10f;
	public AudioClip deadSound;

	public HealthBarEnemy HealthBar; 

	[Header("Attack")]
	public GameObject Stone;
	public Transform attackPoint;
	public float MinAttackTime = 2f;
	public float MaxAttackTime = 4f;


	bool isDead = false;
	Rigidbody2D rig;

	// Use this for initialization
	void Start () {
		rig = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();

		if (HealthBar != null) {
			HealthBar.maxHealth = health;
			HealthBar.currentHealth = health;
		}


	}

	//send by Detect Player trigger object 
	void Play(){
		StartCoroutine (Attack (Random.Range (MinAttackTime, MaxAttackTime)));
	}

	IEnumerator Attack(float delay){
		anim.SetTrigger ("Attack");
		yield return new WaitForSeconds (delay);

		if (GameManager.Instance != null && GameManager.Instance.State == GameManager.GameState.Playing) {
			StartCoroutine (Attack (Random.Range (MinAttackTime, MaxAttackTime)));
		}
	}

	//Called by animation event trigger
	public void ThrowStone(){
		if (Stone == null || attackPoint == null) {
			return;
		}

		Instantiate (Stone, attackPoint.position, Quaternion.identity);
	}

	public void TakeDamage (float damage, Vector2 force, GameObject instigator)
	{
		if (isDead)
			return;

		health -= damagePerHit;
		
		isDead = health <= 0;
		if (HealthBar != null) {
			HealthBar.currentHealth = health;
		}
		if (isDead) {
			SoundManager.PlaySfx (deadSound);
			anim.SetTrigger ("Dead");
			if (HealthBar != null) {
				HealthBar.gameObject.SetActive (false);
			}
			SetCollidersEnabled(false);
			rig.isKinematic = true;

			GameManager.Instance.GameFinish ();
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
