using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_StartScreen : MonoBehaviour {

	public Text levelName;
	public Text lives;

	// Use this for initialization
	void Start () {
		levelName.text = LevelManager.Instance.LevelName;
		lives.text = "x" + GameManager.Instance.SavedLives;
	}
}
