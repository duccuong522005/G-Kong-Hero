using UnityEngine;
using System.Collections;

public class DetectPlayerAndSendMessage : MonoBehaviour {
	public AudioClip detectSound;
	[Range(0,1)]
	public float detectSoundVolume = 0.5f;
	public GameObject[] Targets;
	public string message;

	void OnTriggerEnter2D(Collider2D other){
		if (other.GetComponent<Player> () != null && Targets.Length > 0) {
			foreach (var target in Targets)
				target.SendMessage (message);

			SoundManager.PlaySfx (detectSound, detectSoundVolume);
			gameObject.SetActive (false);
		}
	}
}
