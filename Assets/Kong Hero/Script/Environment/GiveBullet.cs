using UnityEngine;
using System.Collections;

public class GiveBullet : MonoBehaviour,IPlayerRespawnListener {
	public int bulletToAdd = 1;
	public int pointToAdd = 5;
	public GameObject Effect;
	public bool isRespawnCheckPoint = true;
	public AudioClip sound;
	[Range(0,1)]
	public float soundVolume = 0.5f;


	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.GetComponent<Player> () == null)
			return;

		SoundManager.PlaySfx (sound, soundVolume);

		GameManager.Instance.AddBullet (bulletToAdd);
		GameManager.Instance.AddPoint (pointToAdd);

		if (Effect != null)
			Instantiate (Effect, transform.position, transform.rotation);

		gameObject.SetActive (false);
	}

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		if (isRespawnCheckPoint)
			gameObject.SetActive (true);
	}
}
