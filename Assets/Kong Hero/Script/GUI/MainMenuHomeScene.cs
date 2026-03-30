using UnityEngine;
using System.Collections;

public class MainMenuHomeScene : MonoBehaviour {
	public static MainMenuHomeScene Instance;

	public GameObject StartMenu;
	public GameObject WorldsChoose;
	public GameObject LoadingScreen;
	public GameObject LevelsChoose;
	public GameObject CharacterChoose;
	public GameObject AchievementPanel;
	public GameObject LeaderboardPanel;
	public GameObject[] WorldLevel;

	SoundManager soundManager;

	void Awake(){
		Instance = this;
		soundManager = FindObjectOfType<SoundManager> ();
	}

	// Use this for initialization
	void Start () {
		StartMenu.SetActive (true);
		WorldsChoose.SetActive (false);
		LevelsChoose.SetActive (false);
		LoadingScreen.SetActive (false);
		CharacterChoose.SetActive (false);
	}
	
	public void OpenWorld(int world){
		WorldsChoose.SetActive (false);
		LevelsChoose.SetActive (true);

		if (WorldLevel == null || WorldLevel.Length < world) {
			Debug.LogError("WorldLevel array is not assigned or does not have enough elements for world " + world);
			return;
		}

		for (int i = 0; i < WorldLevel.Length; i++) {
			if (i == (world - 1)) {
				WorldLevel [i].SetActive (true);
			} else
				WorldLevel [i].SetActive (false);
		}

		SoundManager.PlaySfx (soundManager.soundClick);
	}

	public void OpenWorldChoose(){
		StartMenu.SetActive (false);
		WorldsChoose.SetActive (true);
		LevelsChoose.SetActive (false);

		SoundManager.PlaySfx (soundManager.soundClick);
	}

	public void OpenStartMenu(){
		StartMenu.SetActive (true);
		WorldsChoose.SetActive (false);
		CharacterChoose.SetActive (false);

		SoundManager.PlaySfx (soundManager.soundClick);
	}

	public void OpenCharacterChoose(){
		StartMenu.SetActive (false);
		CharacterChoose.SetActive (true);

		SoundManager.PlaySfx (soundManager.soundClick);
	}

	public void OpenAchievement(){
		// Nếu đang mở world/character hay loading thì tắt hết
		StartMenu.SetActive (false);
		WorldsChoose.SetActive (false);
		LevelsChoose.SetActive (false);
		CharacterChoose.SetActive (false);
		LeaderboardPanel?.SetActive(false);

		if (AchievementPanel != null)
			AchievementPanel.SetActive (true);

		// Reload achievement state từ PlayerPrefs khi mở panel
		if (AchievementManager.Instance != null) {
			AchievementManager.Instance.LoadAchievementState();
			AchievementManager.Instance.RefreshUI();
		}

		SoundManager.PlaySfx (soundManager.soundClick);
	}

	public void CloseAchievement(){
		if (AchievementPanel != null)
			AchievementPanel.SetActive (false);

		OpenStartMenu();
		SoundManager.PlaySfx (soundManager.soundClick);
	}

	public void OpenLeaderboard(){
		StartMenu.SetActive (false);
		WorldsChoose.SetActive (false);
		LevelsChoose.SetActive (false);
		CharacterChoose.SetActive (false);
		AchievementPanel?.SetActive(false);

		if (LeaderboardPanel != null)
			LeaderboardPanel.SetActive(true);

		if (LeaderboardManager.Instance != null) {
			LeaderboardManager.Instance.ApplyPendingPlayerScore();
			LeaderboardManager.Instance.LoadLeaderboard();
			LeaderboardManager.Instance.RefreshUI();
		}

		SoundManager.PlaySfx (soundManager.soundClick);
	}

	public void CloseLeaderboard(){
		if (LeaderboardPanel != null)
			LeaderboardPanel.SetActive(false);

		OpenStartMenu();
		SoundManager.PlaySfx (soundManager.soundClick);
	}
}
