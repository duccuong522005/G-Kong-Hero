using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_GUI : MonoBehaviour {

	public Text scoreText;
	public Text liveText;
	public Text bulletText;
	public Text coinText;
	public Text timerText;
	
	// Update is called once per frame
	void Update () {
		scoreText.text = GameManager.Instance.Point.ToString ("0000000");
		coinText.text = GameManager.Instance.Coin.ToString ("00");
		timerText.text = LevelManager.Instance.currentTimer.ToString ("000");
		bulletText.text = GameManager.Instance.Bullet.ToString ();
		liveText.text = "x" + GameManager.Instance.SavedLives.ToString ();
	}
}
