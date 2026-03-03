using UnityEngine;	
using System.Collections;	
	
public class BOSS_3 : MonoBehaviour,ICanTakeDamage {	
	Animator anim;

	[Range(10,100)]
	public float health = 100f;


	public float damagePerHit = 10f;

	public AudioClip attackSound;
	public AudioClip deadSound;

	public HealthBarEnemy HealthBar; 

	public float speedFly  = 3;
	public float speedAttack ;

	public float attackMin = 5;
	public float attackMax = 10;

	public Transform[] PointBackUps;
	Transform pointBackUp;
	Transform currentPatrolPoint;

	Player player;

	bool attack;
	bool backup;
	bool patrolRight;

	float oldPos, currentPos;

	bool isDead = false;
	Rigidbody2D rig;


	[Header("damage")]
	public int DamageToPlayer;
	[Tooltip("delay a moment before give next damage to Player")]
	public float rateDamage = 0.2f;
	public Vector2 pushPlayer = new Vector2 (0, 10);
	float nextDamage;

	[Tooltip("Give damage to this object when Player jump on his head")]
	public bool canBeKillOnHead = false;
			
	// Use this for initialization	
	void Start () {		
		player = FindObjectOfType<Player> ();
		currentPatrolPoint = PointBackUps [Random.Range (0, PointBackUps.Length)];

		rig = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();

		if (HealthBar != null) {
			HealthBar.maxHealth = health;
			HealthBar.currentHealth = health;
		}
		oldPos = currentPos = transform.position.x;
	}

	public void Play(){
		StartCoroutine (AttackCo ());
	}
		
	// Update is called once per frame	
	void Update () {	
		if (isDead)
			return;
		
		if (attack) {
			transform.position = Vector2.MoveTowards (transform.position, player.transform.position, speedAttack * Time.deltaTime);
			if (Vector2.Distance (transform.position, player.transform.position) < 0.1f) {
				BackUp ();
			}
		} else if (backup) {
			transform.position = Vector2.MoveTowards (transform.position, pointBackUp.position, speedFly * Time.deltaTime * 2);
			if (Vector2.Distance (transform.position, pointBackUp.position) < 0.1f) {
				backup = false;
				StartCoroutine (AttackCo ());
			}
		} else {
			transform.position = Vector2.MoveTowards (transform.position, currentPatrolPoint.position, speedFly * Time.deltaTime/2);
			if (Vector2.Distance (transform.position, currentPatrolPoint.position) < 0.1f) {
				currentPatrolPoint = PointBackUps [Random.Range (0, PointBackUps.Length)];
			}
		}

		currentPos = transform.position.x;

		if (currentPos < oldPos) 
			transform.localScale = new Vector3 (1, 1, 1);
		else if(currentPos > oldPos)
			transform.localScale = new Vector3 (-1, 1, 1);

		oldPos = currentPos;
	}	

	void BackUp(){
		attack = false;
		backup = true;
		pointBackUp = PointBackUps [Random.Range (0, PointBackUps.Length)];
	}

	IEnumerator AttackCo(){
		var delay = Random.Range (attackMin, attackMax);

		yield return new WaitForSeconds (delay);

		attack = true;
		SoundManager.PlaySfx (attackSound);
	}

	public void TakeDamage (float damage, Vector2 force, GameObject instigator)
	{
		if (isDead)
			return;

		health -= damagePerHit;
//		if (health < 50)
//			randomMax = coolDownState2;

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
			rig.isKinematic = false;
			rig.AddForce (new Vector2 (0, 200));

			GameManager.Instance.GameFinish ();
		}

		BackUp ();
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
}	
