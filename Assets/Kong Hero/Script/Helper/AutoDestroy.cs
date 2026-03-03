using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {
	public float destroyAfterTime = 3f;

	void Awake(){
		Destroy (gameObject, destroyAfterTime);
	}
}
