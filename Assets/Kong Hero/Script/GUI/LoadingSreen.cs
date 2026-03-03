using UnityEngine;
using System.Collections;

public class LoadingSreen : MonoBehaviour {
	public static LoadingSreen Instance;

	// Use this for initialization
	void Awake () {
		Instance = this;
		gameObject.SetActive (false);
	}
	
	public static void Show(){
		Instance.gameObject.SetActive (true);
	}
}
