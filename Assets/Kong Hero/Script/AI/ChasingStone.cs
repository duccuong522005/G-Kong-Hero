using UnityEngine;
using System.Collections;

public class ChasingStone : MonoBehaviour {
	Player player;
	public float speedMin = 2f;
	public float speedMax = 3f;
	float speed;
	public Vector2 direction;

	public GameObject ExplosionFx;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<Player> ();
		direction = (player.transform.position - transform.position).normalized;
		speed = Random.Range (speedMin, speedMax);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (speed * direction*Time.deltaTime);
	}
}
