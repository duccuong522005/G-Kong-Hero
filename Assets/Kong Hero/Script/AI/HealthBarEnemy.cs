using UnityEngine;
using System.Collections;

public class HealthBarEnemy : MonoBehaviour {
	public Transform forceGroundSprite;
	public float maxHealth = 1;
	public float currentHealth = 1;

	// Update is called once per frame
	void Update () {
		var healthPercent = (float) currentHealth / maxHealth;
		forceGroundSprite.localScale = new Vector3 (healthPercent, 1, 1);
		//		forceGroundRenderer.color = Color.Lerp (minColor, maxColor, healthPercent);
	}
}
