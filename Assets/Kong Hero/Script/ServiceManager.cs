using UnityEngine;
using System.Collections;

public class ServiceManager : MonoBehaviour {
	public static ServiceManager Instance;

	private GameObject _AdsController, _UnityAds, _IAP, _Facebook;

	void Awake(){
		if (ServiceManager.Instance != null)
			Destroy (gameObject);	//just allow one adscontroller on scene over gameplay, even when you restart this level
		else {
			Instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
//		if (gameObject.transform.childCount == 0) {
//			Debug.LogError ("There are no any service object, please read the Tutorial file to setup the service");
//			return;
//		}

//		if (GameObject.Find ("AdsController") != null)
			_AdsController = GameObject.Find ("AdsController");
//		if (GameObject.Find ("UnityAds") != null)
			_UnityAds = GameObject.Find ("UnityAds");
//		
//		
////		_IAP = transform.Find ("IAP").gameObject;
		_Facebook = GameObject.Find ("Facebook");

	}

	public void ShowAds(){
		if (_AdsController != null) {
//			if (!GlobalValue.RemoveAds)
				_AdsController.SendMessage ("ShowAdsX");
		}
		else
			Debug.Log ("There is no AdsController object in the scene, please read the Tutorial file for more information");
	}

	public void HideAds(){
		if (_AdsController != null)
			_AdsController.SendMessage ("HideAdsX");
		else
			Debug.Log ("There is no AdsController object in the scene, please read the Tutorial file for more information");
	}

	public void ShowRewardAds(){
		if (_UnityAds != null)
			_UnityAds.SendMessage ("ShowRewardVideo");
		else
			Debug.Log ("There is no UnityAds object in the scene, please read the Tutorial file for more information");
	}

//	public void BuyItem1(){
//		Debug.LogWarning ("Only work on real device and published on store");
//		if (_IAP != null)
//			_IAP.SendMessage ("BuyItem1");
//		else
//			Debug.LogWarning ("There is no IAP object in the scene, please read the Tutorial file for more information");
//	}
//
//	public void BuyItem2(){
//		Debug.LogWarning ("Only work on real device and published on store");
//		if (_IAP != null)
//			_IAP.SendMessage ("BuyItem2");
//		else
//			Debug.LogWarning ("There is no IAP object in the scene, please read the Tutorial file for more information");
//	}
//
//	public void RemoveAds(){
//		Debug.LogWarning ("Only work on real device and published on store");
//		if (_IAP != null)
//			_IAP.SendMessage ("BuyRemoveAds");
//		else
//			Debug.LogWarning ("There is no IAP object in the scene, please read the Tutorial file for more information");
//	}

	public void InviteFriend(){
		if (_Facebook != null)
			_Facebook.SendMessage ("InviteFriends");
		else
			Debug.Log ("There is no Facebook object in the scene, please read the Tutorial file for more information");
	}
	

}
