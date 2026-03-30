using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_Level : MonoBehaviour {
	private const string DefaultScenePlaceholder = "Level Name";

	public int worldNumber = 0;
	public int levelNumber = 0;

	public string loadscene = DefaultScenePlaceholder;

	public Text TextLevel;
	public GameObject Locked;
	private Button levelButton;

	void Start () {
		levelButton = GetComponent<Button> ();
		var levelReached = PlayerPrefs.GetInt (worldNumber.ToString (), 1);
		if (levelNumber <= levelReached && worldNumber <= PlayerPrefs.GetInt (GlobalValue.WorldReached, 1)) {
			TextLevel.gameObject.SetActive (true);
			TextLevel.text = levelNumber.ToString ();
			Locked.SetActive (false);
		} else {
			TextLevel.gameObject.SetActive (false);
			Locked.SetActive (true);
			if (levelButton != null) {
				levelButton.interactable = false;
			}
		}
	}

	public void LoadScene(){
		GlobalValue.worldPlaying = worldNumber;
		GlobalValue.levelPlaying = levelNumber;

		LoadingSreen.Show ();
		int buildIndex = ResolveBuildIndex();
		AsyncOperation operation = null;

		if (buildIndex >= 0) {
			operation = SceneManager.LoadSceneAsync(buildIndex);
		} else if (!string.IsNullOrWhiteSpace(loadscene) && loadscene != DefaultScenePlaceholder) {
			operation = SceneManager.LoadSceneAsync(loadscene);
		}

		if (operation == null) {
			Debug.LogError("Cannot load level scene. Check Build Settings and button config. world=" + worldNumber + ", level=" + levelNumber + ", loadscene=" + loadscene);
			LoadingSreen.Hide();
		}
	}

	int ResolveBuildIndex(){
		string candidateByNumber = "World " + worldNumber + "-" + levelNumber;

		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
			string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
			string sceneName = Path.GetFileNameWithoutExtension(scenePath);

			if (!string.IsNullOrWhiteSpace(loadscene) &&
				loadscene != DefaultScenePlaceholder &&
				string.Equals(sceneName, loadscene, System.StringComparison.OrdinalIgnoreCase)) {
				return i;
			}

			if (string.Equals(sceneName, candidateByNumber, System.StringComparison.OrdinalIgnoreCase)) {
				return i;
			}
		}

		return -1;
	}
}
