using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_Gameover : MonoBehaviour {
	public Text liveText;
	public GameObject Next;
	public GameObject StartOver;
	public GameObject Buttons;
	int lives;

	void Awake(){
		if (LevelManager.Instance.isLastLevelOfWorld)
			Next.SetActive (false);		//dont show the next button when this is the final level



	}

	void OnEnable () {

		Buttons.SetActive (false);

		if (!GameManager.Instance.isNoLives)
			lives = GameManager.Instance.SavedLives;
		else
			lives = 0;



		var levelReached = PlayerPrefs.GetInt (GlobalValue.worldPlaying.ToString (), 1);

		if (GlobalValue.levelPlaying < levelReached)
			Next.SetActive (true);
		else
			Next.SetActive (false);
		
		liveText.text = (lives + 1).ToString ("00");
		StartCoroutine (SubtractLiveCo (1));
	}

	IEnumerator SubtractLiveCo(float time){
		
		
		yield return new WaitForSeconds (time);

		liveText.text = lives.ToString("00");
		liveText.gameObject.GetComponent<Animator> ().SetTrigger ("live");

		if (lives <= 0) {
			StartOver.SetActive (true);
			Buttons.SetActive (false);
		} else {
			StartOver.SetActive (false);
			Buttons.SetActive (true);
		}
		


	}
}
