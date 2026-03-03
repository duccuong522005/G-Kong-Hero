using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour,IPlayerRespawnListener {
	public enum BlockTyle{Destroyable, Rocky}
	public BlockTyle blockTyle;
	public LayerMask enemiesLayer;

	public int maxHit = 1;
	public float pushEnemyUp = 7f;
	public float sizeDetectEnemies = 0.25f;
	public int pointToAdd = 100;

	[Header("Destroyable")]
	public GameObject DestroyEffect;

	[Header("HidenTreasure")]
	public GameObject[] Treasure;
	public Transform spawnPoint;
	public Sprite imageBlockStatic;

	[Header("Sound")]
	public AudioClip soundDestroy;
	[Range(0,1)]
	public float soundDestroyVolume = 0.5f;
	public AudioClip soundSpawn;
	[Range(0,1)]
	public float soundSpawnVolume = 0.5f;

	Animator anim;
	SpriteRenderer spriteRenderer;
	Sprite oldSprite;
	int currentHitLeft;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		oldSprite = spriteRenderer.sprite;
		currentHitLeft = Mathf.Clamp (maxHit, 1, int.MaxValue);
	}
	
	void OnTriggerEnter2D(Collider2D other){
		var Player = other.gameObject.GetComponent<Player> ();
		if (Player == null)
			return;

		var hit = Physics2D.Raycast (transform.position, Vector2.down, 0.5f, 1 << LayerMask.NameToLayer ("Player"));
		if (!hit)
			return;

		if (currentHitLeft <= 0)
			return;

		Player.SetForce (new Vector2 (Player.velocity.x, 0));

		//spawn treasure

		var random = Treasure.Length > 0 ? Treasure [Random.Range (0, Treasure.Length)] : null;
		if (random != null) {
			Instantiate (random, spawnPoint.position, Quaternion.identity);
			SoundManager.PlaySfx (soundSpawn, soundSpawnVolume);
		}

		CheckEnemiesOnTop ();

		anim.SetTrigger ("hit");

		currentHitLeft--;
		if (currentHitLeft > 0) 
			return;

		if (blockTyle == BlockTyle.Destroyable) {
			if (random == null)		//only play destroy sound when there are no treasure to spawn
				SoundManager.PlaySfx (soundDestroy, soundDestroyVolume);
			
			if (DestroyEffect != null)
				Instantiate (DestroyEffect, transform.position, Quaternion.identity);

			if (pointToAdd != 0)
				GameManager.Instance.ShowFloatingText (pointToAdd.ToString (), transform.position, Color.white);
			
			gameObject.SetActive (false);
		} else if (blockTyle == BlockTyle.Rocky)
			spriteRenderer.sprite = imageBlockStatic;
		
	}

	void CheckEnemiesOnTop(){
		//check if any enemies on top? kill them
		var hits = Physics2D.CircleCastAll (spawnPoint.position, sizeDetectEnemies, Vector2.zero, 0, 1 << LayerMask.NameToLayer ("Enemies"));
		if (hits.Length > 0) {
			foreach (var hit in hits) {
				var damage = (ICanTakeDamage)hit.collider.gameObject.GetComponent (typeof(ICanTakeDamage));
				if (damage != null)
					damage.TakeDamage (10000, Vector2.up*pushEnemyUp, gameObject);	//kill it right away
			}
		}
	}

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		spriteRenderer.sprite = oldSprite;
		currentHitLeft = Mathf.Clamp (maxHit, 1, int.MaxValue);
		gameObject.SetActive (true);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (spawnPoint.position, sizeDetectEnemies);
	}
}
