using UnityEngine;
using System.Collections;

public class FireBallAI : MonoBehaviour,ICanTakeDamage, IPlayerRespawnListener {
	public bool isKillByProjectile = true;

	float oldY;

	// Use this for initialization
	void Start () {
		oldY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		var currentY = transform.position.y;

		if (currentY > oldY) {
			transform.localScale = new Vector3 (1, 1, 1);
		} else {
			transform.localScale = new Vector3 (1, -1, 1);
		}

		oldY = currentY;
	}

	#region ICanTakeDamage implementation

	public void TakeDamage (float damage, Vector2 force, GameObject instigator)
	{
		if (!isKillByProjectile || instigator == null) {
			return;
		}
		
		var projectile =(Projectile) instigator.GetComponent (typeof(Projectile));

		if (projectile != null) {
			gameObject.SetActive (false);
		}
	}

	#endregion

	#region IPlayerRespawnListener implementation

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		oldY = transform.position.y;
		gameObject.SetActive (true);
	}

	#endregion
}
