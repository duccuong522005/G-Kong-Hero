using UnityEngine;
using System.Collections;

public class GiveDamageToPlayer : MonoBehaviour {
	[Header("Option")]
	[Tooltip("destroy this object when hit player?")]
	public bool isDestroyWhenHitPlayer = false;
	public GameObject DestroyFx;

	[Header("Make Damage")]
	public int DamageToPlayer;
	[Tooltip("delay a moment before give next damage to Player")]
	public float rateDamage = 0.2f;
	public Vector2 pushPlayer = new Vector2 (0, 10);
	float nextDamage;

	[Tooltip("Give damage to this object when Player jump on his head")]
	public bool canBeKillOnHead = false;
	public float damageOnHead;

	void OnTriggerStay2D(Collider2D other){
		var player = other.GetComponent<Player> ();
		if (player == null) {
			return;
		}

		if (!player.isPlaying) {
			return;
		}

		if (Time.time < nextDamage + rateDamage) {
			return;
		}

		nextDamage = Time.time;

		if (canBeKillOnHead && player.transform.position.y > transform.position.y) {

			player.SetForce(pushPlayer);
			var canTakeDamage = (ICanTakeDamage) GetComponent (typeof(ICanTakeDamage));
			if (canTakeDamage != null) {
				canTakeDamage.TakeDamage (damageOnHead, Vector2.zero, gameObject);
			}
			
			return;
		}



		//Push player back
//		var facingDirectionX = Mathf.Sign (Player.transform.localScale.x);
//		var facingDirectionY = Mathf.Sign (Player.velocity.y);
		if (DamageToPlayer == 0) {
			return;
		}

		var facingDirectionX = Mathf.Sign (player.transform.position.x - transform.position.x);
		var facingDirectionY = Mathf.Sign (player.velocity.y);

		player.SetForce(new Vector2 (Mathf.Clamp (Mathf.Abs(player.velocity.x), 10, 15) * facingDirectionX,
			Mathf.Clamp (Mathf.Abs(player.velocity.y), 5, 15) * facingDirectionY * -1));

		player.TakeDamage (DamageToPlayer, Vector2.zero, gameObject);

		if (isDestroyWhenHitPlayer) {
			if (DestroyFx != null) {
				Instantiate (DestroyFx, transform.position, Quaternion.identity);
			}

			Destroy (gameObject);
		}
	}
}
