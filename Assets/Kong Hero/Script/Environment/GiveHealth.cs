using UnityEngine;
using System.Collections;

public class GiveHealth : MonoBehaviour,IPlayerRespawnListener
{
	public int healthToGive;
	public GameObject Effect;
	public bool isRespawnCheckPoint = true;
	public AudioClip soundEffect;
	[Range(0,1)]
	public float soundEffectVolume = 0.5f;

	// Update is called once per frame
	void OnTriggerEnter2D(Collider2D other){
		var Player = other.gameObject.GetComponent<Player> ();
		if (Player == null)
			return;

		Player.GiveHealth (healthToGive, gameObject);
		if (Effect != null)
			Instantiate (Effect, transform.position, Quaternion.identity);

		SoundManager.PlaySfx (soundEffect, soundEffectVolume);

		gameObject.SetActive (false);
	}

	#region IPlayerRespawnListener implementation

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		if (isRespawnCheckPoint)
			gameObject.SetActive (true);
	}

	#endregion
}

