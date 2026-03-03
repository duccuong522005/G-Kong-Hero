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

		if (GameManager.Instance.State == GameManager.GameState.Playing)
			StartCoroutine (Attack (Random.Range (MinAttackTime, MaxAttackTime)));
	}

	//Called by animation event trigger
	public void ThrowStone(){
		Instantiate (Stone, attackPoint.position, Quaternion.identity);
	}

	public void TakeDamage (float damage, Vector2 force, GameObject instigator)
	{
		if (isDead)
			return;

		health -= damagePerHit;
		
		isDead = health <= 0 ? true : false;
		if (HealthBar != null)
			HealthBar.currentHealth = health;
		if (isDead) {
			SoundManager.PlaySfx (deadSound);
			anim.SetTrigger ("Dead");
			HealthBar.gameObject.SetActive (false);
			var boxCo = GetComponents<BoxCollider2D> ();
			foreach (var box in boxCo) {
				box.enabled = false;
			}
			var CirCo = GetComponents<CircleCollider2D> ();
			foreach (var cir in CirCo) {
				cir.enabled = false;
			}
			rig.isKinematic = true;

			GameManager.Instance.GameFinish ();
		}
	}
}
