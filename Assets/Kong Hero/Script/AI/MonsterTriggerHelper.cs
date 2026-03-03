using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class MonsterTriggerHelper : MonoBehaviour,IPlayerRespawnListener {

	public GameObject[] Monsters;

	void Start(){
		StartCoroutine (DisableAllEnemiesCo (0.1f));
	}

	//when detect Player, set active all monsters in list
	void OnTriggerEnter2D(Collider2D other){
		if (other.GetComponent<Player> () == null)
			return;

		foreach (var monster in Monsters) {
			if (monster != null)
				monster.SetActive (true);
		}
	}

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		StartCoroutine (DisableAllEnemiesCo (0.1f));		//dothis to make sure disable all enemies after Respawn event
	}

	IEnumerator DisableAllEnemiesCo(float delay){
		yield return new WaitForSeconds (delay);
		foreach (var monster in Monsters) {
			if (monster != null)
				monster.SetActive (false);
		}
	}

	void OnDrawGizmosSelected(){
		foreach (var obj in Monsters) {
			Gizmos.DrawLine (transform.position, obj.transform.position);
		}
	}
}
