using UnityEngine;
using System.Collections;

public class CloseGateBoss : MonoBehaviour {
	bool close = false;
	public Transform TheGate;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (close)
			TheGate.position = Vector2.MoveTowards (TheGate.transform.position, transform.position, 0.1f);
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.GetComponent<Player> () != null)
			close = true; 
	}
}
