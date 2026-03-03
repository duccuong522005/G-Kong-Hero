using UnityEngine;
using System.Collections;

public class FishAI : MonoBehaviour, ICanTakeDamage, IPlayerRespawnListener {
	
	public GameObject DestroyEffect;
	public AudioClip deadSound;
	[Range(0,1)]
	public float deadSoundVolume = 0.5f;

	Vector3 _oldPosition;

	// Use this for initialization
	void Start () {
		_oldPosition = transform.position;
	}

	public void TakeDamage (float damage, Vector2 force, GameObject instigator)
	{
		if (DestroyEffect != null)
			Instantiate (DestroyEffect, transform.position, transform.rotation);

		SoundManager.PlaySfx (deadSound, deadSoundVolume);
		
		gameObject.SetActive (false);
	}

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		transform.position = _oldPosition;
		gameObject.SetActive (true);
	}
}
