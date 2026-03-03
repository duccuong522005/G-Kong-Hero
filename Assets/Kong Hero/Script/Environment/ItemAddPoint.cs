using UnityEngine;
using System.Collections;

public class ItemAddPoint : MonoBehaviour,IPlayerRespawnListener {

	public int pointToAdd;
	public GameObject PointEffect;
	public AudioClip soundEffect;
	[Range(0,1)]
	public float soundEffectVolume = 0.5f;

	void OnTriggerEnter2D(Collider2D other){
		if (other.GetComponent<Player> () == null)
			return;

		GameManager.Instance.ShowFloatingText ("+" + pointToAdd, transform.position, Color.white);
		GameManager.Instance.AddPoint (pointToAdd);
		if (PointEffect != null)
			Instantiate (PointEffect, transform.position, Quaternion.identity);
		SoundManager.PlaySfx (soundEffect, soundEffectVolume);

		gameObject.SetActive (false);
	}

	void IPlayerRespawnListener.OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		gameObject.SetActive (true);
	}
}
