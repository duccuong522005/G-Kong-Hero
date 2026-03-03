using UnityEngine;
using System.Collections;

public interface ICanTakeDamage {

	void TakeDamage (float damage, Vector2 force, GameObject instigator);
}
