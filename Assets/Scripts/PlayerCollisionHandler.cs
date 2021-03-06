using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour {

	public int damage;
	public Vector2 defaultKnockback;

	private Damageable damageScript;
	private SpriteRenderer sr;

	void Start() {
		damageScript = GetComponent<Damageable>();
		sr = GetComponent<SpriteRenderer>();
	}

	private void OnTriggerEnter2D(Collider2D collider) {
		switch (collider.gameObject.tag) {
			// OBSOLETO: Os projéteis chamam TakeDamage por conta própria
			// case "Projectile":
				// float xDirection = sr.flipX ? 1f : -1f;
				// damageScript.TakeDamage(damage, new Vector2(xDirection * defaultKnockback.x, defaultKnockback.y), collider);
				// break;
			case "Enemy":
				float horDist = transform.position.x - collider.gameObject.transform.position.x;
				Vector2 knockback = new Vector2(horDist > 0 ? defaultKnockback.x : -defaultKnockback.x, defaultKnockback.y);
				damageScript.TakeDamage(damage, knockback, collider.gameObject.GetComponent<BoxCollider2D>());
				break;
			case "Spikes":
				damageScript.TakeDamage(999, defaultKnockback, collider);
				break;
		}
	}
}
