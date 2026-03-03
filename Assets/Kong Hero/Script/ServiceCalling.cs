using UnityEngine;
using System.Collections;

public class ServiceCalling : MonoBehaviour {

	public void FaceBookInvite(){
		ServiceManager.Instance.InviteFriend ();
	}

//	public void RemoveAds(){
//		ServiceManager.Instance.RemoveAds ();
//	}

	public void WatchVideoAds(){
		ServiceManager.Instance.ShowRewardAds ();
	}

//	public void BuyItem1(){
//		ServiceManager.Instance.BuyItem1 ();
//	}
//
//	public void BuyItem2(){
//		ServiceManager.Instance.BuyItem2 ();
//	}
}
