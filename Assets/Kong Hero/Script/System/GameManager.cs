using UnityEngine;
using System.Collections;

public class GameManager: MonoBehaviour {
	public static GameManager Instance{ get; private set;}

	public enum GameState{Menu,Playing, Dead, Finish};
	public GameState State{ get; set; }

	public enum ZeroLivesAction{ResetHighestWorlds, ResetAllWorlds};
	public ZeroLivesAction zeroLiveAction;

	public GameObject FadeInEffect;
	[Header("Floating Text")]
	public GameObject FloatingText;
	private MenuManager menuManager;

	[Header("Default Value")]
	public int defaultLive = 10;
	public int defaultBullet = 3;

	public Player Player{ get; private set;}
	SoundManager soundManager;

	[HideInInspector]
	public bool isNoLives = false;

	void Awake(){
		Instance = this;
		State = GameState.Menu;
		Player = FindObjectOfType<Player> ();

		if (CharacterHolder.Instance != null && CharacterHolder.Instance.CharacterPicked != null) {
			Instantiate (CharacterHolder.Instance.CharacterPicked, Player.transform.position, Player.transform.rotation);
			Destroy (Player.gameObject);

			Player = FindObjectOfType<Player> ();
		}
	}


	public int Point{ get; set; }
	int savePointCheckPoint;

	public int Coin{ get; set; }
	int saveCoinCheckPoint;

	public int Bullet{ get; set; }
	int saveBulletCheckPoint;

	public void AddPoint(int addpoint){
		Point += addpoint;
	}

	public void AddCoin(int addcoin){
		Coin += addcoin;
	}

	public void AddBullet(int addbullet){
		Bullet += addbullet;
	}

	public int SavedLives{ 
		get { return PlayerPrefs.GetInt (GlobalValue.Lives, defaultLive); } 
		set { PlayerPrefs.SetInt (GlobalValue.Lives, value); } 
	}
	public int SavedCoins{ 
		get { return PlayerPrefs.GetInt (GlobalValue.Coins, 0); } 
		set { PlayerPrefs.SetInt (GlobalValue.Coins, value); } 
	}
	public int SavedPoints { 
		get { return PlayerPrefs.GetInt (GlobalValue.Points, 0); } 
		set { PlayerPrefs.SetInt (GlobalValue.Points, value); } 
	}
	public int SavedBullets { 
		get { return PlayerPrefs.GetInt (GlobalValue.Bullets, defaultBullet); } 
		set { PlayerPrefs.SetInt (GlobalValue.Bullets, value); } 
	}


	void Start(){
		menuManager = FindObjectOfType<MenuManager> ();

		soundManager = FindObjectOfType<SoundManager> ();

		Coin = SavedCoins;
		Bullet = SavedBullets;
		Point = SavedPoints;
	}


	//called by LevelManager
	public void SaveCheckPoint(){
		savePointCheckPoint = Point;
		saveCoinCheckPoint = Coin;
		saveBulletCheckPoint = Bullet;
	}

	private void ResetCheckPoint(){
		if (savePointCheckPoint != 0) {
			Point = savePointCheckPoint;
			Coin = saveCoinCheckPoint;
			Bullet = saveBulletCheckPoint;
			State = GameState.Playing;
		} else {
			Coin = SavedCoins;
			Bullet = SavedBullets;
			Point = SavedPoints;
		}
	}

	public void ShowFloatingText(string text, Vector2 positon, Color color){
		GameObject floatingText = Instantiate (FloatingText) as GameObject;
		var _position = Camera.main.WorldToScreenPoint (positon);

		floatingText.transform.SetParent (menuManager.transform,false);
		floatingText.transform.position = _position;
			
		var _FloatingText = floatingText.GetComponent<FloatingText> ();
		_FloatingText.SetText (text, color);
	}

	public void StartGame(){
		State = GameState.Playing;
		LevelManager.Instance.StartGame ();

		if (ServiceManager.Instance != null)
			ServiceManager.Instance.HideAds ();
	}

	public void GameFinish(){
		State = GameState.Finish;
		Player.GameFinish ();
		MenuManager.Instance.Gamefinish ();
		SoundManager.PlaySfx (soundManager.soundGamefinish, 0.5f);

		//save coins and points
		SavedCoins = Coin;
		SavedPoints = Point;
		SavedBullets = Bullet;

		//unlock new world if this level is the last one
		if (LevelManager.Instance.isLastLevelOfWorld) {
			PlayerPrefs.SetInt (GlobalValue.WorldReached, GlobalValue.worldPlaying + 1);
			Debug.Log ("Completed the last level, if this is not the final level, please uncheck isLastLevelOfWorld in LevelManager script");
			return;
		}

		//check to unlock new level
		var levelreached = PlayerPrefs.GetInt (GlobalValue.worldPlaying.ToString (), 1);
		if (GlobalValue.levelPlaying == levelreached) {
			PlayerPrefs.SetInt (GlobalValue.worldPlaying.ToString (), levelreached + 1);
			Debug.Log ("Unlock new level");
		}

		if (ServiceManager.Instance != null)
			ServiceManager.Instance.ShowAds ();
	}

	public void GameOver(){
		State = GameState.Dead;
		SavedLives--;
		MenuManager.Instance.GameOver ();
		SoundManager.PlaySfx (soundManager.soundGameover, 0.5f);

		if (SavedLives <= 0) {
			//reset all levels and worlds

			isNoLives = true;

			if (zeroLiveAction == ZeroLivesAction.ResetAllWorlds) {
				PlayerPrefs.DeleteAll ();

			} else {
				var highestWorld = PlayerPrefs.GetInt (GlobalValue.WorldReached, 1);
				//Reset the highest world
				PlayerPrefs.SetInt (highestWorld.ToString (), 1);
				SavedBullets = defaultBullet;
				SavedLives = defaultLive;
				SavedPoints = 0;
			}
		}

		if (ServiceManager.Instance != null)
			ServiceManager.Instance.ShowAds ();
	}

	//called by MenuManager
	public void GotoCheckPoint(){
		State = GameState.Playing;

		ResetCheckPoint ();
		LevelManager.Instance.GotoCheckPoint ();

		Instantiate (FadeInEffect);

		if (ServiceManager.Instance != null)
			ServiceManager.Instance.HideAds ();
	}
}
