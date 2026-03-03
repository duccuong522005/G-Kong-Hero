using UnityEngine;
using System.Collections;

public class FireEvil : MonoBehaviour, IPlayerRespawnListener {
	public float speed = 5;
	float _direction;
	Vector3 old_position;

	// Use this for initialization
	void Start () {
		_direction = transform.localScale.x > 0 ? -1 : 1;
		old_position = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (speed * Time.deltaTime * _direction, 0, 0);
	}

	void OnBecameInvisible(){
		gameObject.SetActive (false);
	}

	#region IPlayerRespawnListener implementation
	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		transform.position = old_position;
		gameObject.SetActive (true);
	}
	#endregion
}
