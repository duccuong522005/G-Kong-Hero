using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu_StartMenu : MonoBehaviour {

	public string facebookLink;
	public string twitterLink;
	public string moreGameLink;

	public Image soundImage;
	public Sprite soundOn;
	public Sprite soundOff;

	SoundManager soundManager;

	// Use this for initialization
	void Start () {
		if (AudioListener.volume == 0)
			soundImage.sprite = soundOff;
		else
			soundImage.sprite = soundOn;

		soundManager = FindObjectOfType<SoundManager> ();
		
	}
	
	public void TurnSound(){
		if (AudioListener.volume == 0) {
			AudioListener.volume = 1;
			soundImage.sprite = soundOn;
		} else {
			AudioListener.volume = 0;
			soundImage.sprite = soundOff;
		}

		SoundManager.PlaySfx (soundManager.soundClick);
	}

	public void OpenTwitter(){
		Application.OpenURL (twitterLink);

		SoundManager.PlaySfx (soundManager.soundClick);
	}

	public void OpenFacebook(){
		Application.OpenURL (facebookLink);

		SoundManager.PlaySfx (soundManager.soundClick);
	}
}
