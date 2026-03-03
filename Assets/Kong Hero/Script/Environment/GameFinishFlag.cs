using UnityEngine;
using System.Collections;

public class GameFinishFlag : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.GetComponent<Player> () == null)
			return;

		GameManager.Instance.GameFinish ();
	}
}
