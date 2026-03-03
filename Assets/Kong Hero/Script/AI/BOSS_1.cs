using UnityEngine;
using System.Collections;

public class BOSS_1 : MonoBehaviour, ICanTakeDamage {

	public float speed = 1f;

	Rigidbody2D rig;
	Animator anim;

	Player player;

	[Range(10,100)]
	public float health = 100f;

	public float damagePerHit = 10f;

	public AudioClip attackSound;
	public AudioClip deadSound;

	public HealthBarEnemy HealthBar; 

	public float distanceDetectPlayer = 10f;
	public LayerMask playerLayer;

	public Transform centerPoint;

	public float rangeAttack = 0.77f;
	[Range(1,5)]
	public float attackZone = 2f;
	[Range(10,100)]
	public float givePlayerDamage= 30f;

	public float delayHit = 0.1f;
	public float coolDown = 2f;

	[Header("damage")]
	public int DamageToPlayer;
	[Tooltip("delay a moment before give next damage to Player")]
	public float rateDamage = 0.2f;
	public Vector2 pushPlayer = new Vector2 (0, 10);
	float nextDamage;

	[Tooltip("Give damage to this object when Player jump on his head")]
	public bool canBeKillOnHead = false;

	RaycastHit2D hit;
	bool moving;
	float x;
	bool isDead = false;

	// Use this for initialization
	void Start () {
		rig = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		player = FindObjectOfType<Player> ();


		x = transform.localScale.x;

		if (HealthBar != null) {
			HealthBar.maxHealth = health;
			HealthBar.currentHealth = health;
		}
	}

	public void Play(){
		moving = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (isDead || player.isFinish)
			return;
		
		if (moving) {
			hit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y + 0.5f), Vector2.left, distanceDetectPlayer, playerLayer);
			if (hit) {
				transform.Translate (new Vector3 (-speed * Time.deltaTime, 0));
				transform.localScale = new Vector3 (x, transform.localScale.y, transform.localScale.z);
			} else {
				hit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y + 0.5f), Vector2.right, distanceDetectPlayer, playerLayer);
				if (hit) {
					transform.localScale = new Vector3 (-x, transform.localScale.y, transform.localScale.z);
					transform.Translate (new Vector3 (speed * Time.deltaTime, 0));
				}
			}
		}
		
		if (Physics2D.CircleCast(centerPoint.position,attackZone,Vector2.zero,0,playerLayer) && moving) {
			StartCoroutine (Attack ());
			StartCoroutine (IdleDelay (1, 3));
		}

		if (moving && hit)
			anim.SetBool ("walk", true);
		else
			anim.SetBool ("walk", false);
		
	}

	IEnumerator IdleDelay(float min, float max){
		moving = false;
		var delay = Random.Range (min, max);
		yield return new WaitForSeconds (delay);
		moving = true;
	}

	IEnumerator Attack(){
		anim.SetTrigger ("attack");
		yield return new WaitForSeconds (delayHit);
		//check if player still in range and give damage
		if (Physics2D.CircleCast(centerPoint.position,attackZone,Vector2.zero,0,playerLayer))
			player.TakeDamage (givePlayerDamage, new Vector2 (0, 3), gameObject);

	}

	public void TakeDamage (float damage, Vector2 force, GameObject instigator)
	{
		if (isDead)
			return;
		
		anim.SetTrigger ("hit");
		health -= damagePerHit;
		isDead = health <= 0 ? true : false;
		if (HealthBar != null)
			HealthBar.currentHealth = health;
		if (isDead) {
			SoundManager.PlaySfx (deadSound);
			anim.SetTrigger ("die");
			anim.SetBool ("isDead", true);
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

	void OnTriggerStay2D(Collider2D other){
		var Player = other.GetComponent<Player> ();
		if (Player == null)
			return;

		if (!Player.isPlaying)
			return;

		if (Time.time < nextDamage + rateDamage)
			return;

		nextDamage = Time.time;

		if (canBeKillOnHead && Player.transform.position.y > transform.position.y) {

			Player.SetForce (new Vector2 (transform.localScale.x > 0 ? -pushPlayer.x : pushPlayer.x, pushPlayer.y));
			var canTakeDamage = (ICanTakeDamage) GetComponent (typeof(ICanTakeDamage));
			if (canTakeDamage != null)
				canTakeDamage.TakeDamage (damagePerHit, Vector2.zero, gameObject);

			return;
		}



		//Push player back
		//		var facingDirectionX = Mathf.Sign (Player.transform.localScale.x);
		//		var facingDirectionY = Mathf.Sign (Player.velocity.y);


		var facingDirectionX = Mathf.Sign (Player.transform.position.x - transform.position.x);
		var facingDirectionY = Mathf.Sign (Player.velocity.y);

		Player.SetForce(new Vector2 (Mathf.Clamp (Mathf.Abs(Player.velocity.x), 10, 15) * facingDirectionX,
			Mathf.Clamp (Mathf.Abs(Player.velocity.y), 5, 15) * facingDirectionY * -1));

		if (DamageToPlayer == 0)
			return;
		Player.TakeDamage (DamageToPlayer, Vector2.zero, gameObject);
	}

	void OnDrawGizmosSelected(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay (new Vector2 (transform.position.x, transform.position.y +  0.5f), Vector2.left*distanceDetectPlayer);
		Gizmos.DrawRay (new Vector2 (transform.position.x, transform.position.y +  0.5f), Vector2.right*distanceDetectPlayer);
		Gizmos.DrawWireSphere (centerPoint.position, attackZone);
	}
}
