using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour {
	public float Speed;

	public LayerMask LayerCollision;

	public GameObject Owner{ get; private set; }
	public Vector2 Direction{ get; private set; }
	public Vector2 InitialVelocity{ get; private set; }

	// Use this for initialization
	public void Initialize (GameObject owner, Vector2 direction, Vector2 initialVelocity) {
		transform.right = direction;	//turn the X asix to the direction
		Owner = owner;
		Direction = direction;
		InitialVelocity = initialVelocity;
		OnInitialized ();
	}

	public virtual void OnInitialized(){
	}

	public virtual void OnTriggerEnter2D(Collider2D other){
		if ((LayerCollision.value & (1 << other.gameObject.layer)) == 0) {
			OnNotCollideWith (other);
			return;
		}

		var isOwner = Owner == other.gameObject;
		if (isOwner) {
			OnCollideOwner ();
			return;
		}

		var takeDamage = (ICanTakeDamage)other.gameObject.GetComponent (typeof(ICanTakeDamage));
		if (takeDamage != null) {

			if (other.gameObject.GetComponent (typeof(Projectile)) != null)
				OnCollideOther (other);
			else
				OnCollideTakeDamage (other, takeDamage);
			return;
		}

		OnCollideOther (other);
	}

	protected virtual void OnNotCollideWith(Collider2D other){
	}

	protected virtual void OnCollideOwner (){}

	protected virtual void OnCollideTakeDamage(Collider2D other, ICanTakeDamage takedamage){}

	protected virtual void OnCollideOther(Collider2D other){}
}
