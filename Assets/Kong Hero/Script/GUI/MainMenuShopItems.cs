using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuShopItems : MonoBehaviour {
	public int livePrice;
	public int bulletPrice;

	public AudioClip boughtSound;
	[Range(0,1)]
	public float boughtSoundVolume = 0.5f;

	public Text livePriceTxt;
	public Text bulletPriceTxt;

	public Text livesTxt;
	public Text bulletTxt;
	// Use this for initialization
	void Start () {
		livePriceTxt.text = livePrice.ToString ();
		bulletTxt.text = bulletPrice.ToString ();
	}

	void Update(){
		livesTxt.text = "Live: " + PlayerPrefs.GetInt (GlobalValue.Lives, 10);
		bulletTxt.text = "Bullet: " + PlayerPrefs.GetInt (GlobalValue.Bullets, 3);
	}
	
	public void BuyLive(){
		var coins = PlayerPrefs.GetInt (GlobalValue.Coins, 0);
		if (coins >= livePrice) {
			coins -= livePrice;
			PlayerPrefs.SetInt (GlobalValue.Coins, coins);
			var lives = PlayerPrefs.GetInt (GlobalValue.Lives, 10);
			lives++;
			PlayerPrefs.SetInt (GlobalValue.Lives, lives);

			SoundManager.PlaySfx (boughtSound, boughtSoundVolume);
		} else
			NotEnoughCoins.Instance.ShowUp ();
	}

	public void BuyBullet(){
		var coins = PlayerPrefs.GetInt (GlobalValue.Coins, 0);
		if (coins >= bulletPrice) {
			coins -= bulletPrice;
			PlayerPrefs.SetInt (GlobalValue.Coins, coins);
			var bullets = PlayerPrefs.GetInt (GlobalValue.Bullets, 0);
			bullets++;
			PlayerPrefs.SetInt (GlobalValue.Bullets, bullets);

			SoundManager.PlaySfx (boughtSound, boughtSoundVolume);
		} else
			NotEnoughCoins.Instance.ShowUp ();
	}
}
