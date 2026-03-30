using UnityEngine;

public class LoadingSreen : MonoBehaviour {
	public static LoadingSreen Instance;

	void Awake () {
		Instance = this;
		gameObject.SetActive (false);
	}

	public static void Show () {
		if (Instance != null)
			Instance.gameObject.SetActive (true);
	}

	public static void Hide () {
		if (Instance != null)
			Instance.gameObject.SetActive (false);
	}
}
