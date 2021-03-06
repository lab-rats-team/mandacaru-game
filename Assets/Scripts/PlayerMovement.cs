using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float speed;
	public float maxVerticalSpeed = 4.5f;

	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private Animator animator;
	private float xVelocity;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	void Start() {
		GetComponent<Damageable>().hp = SaveLoader.instance.GetPlayerHp();
		transform.position = SaveLoader.instance.GetPlayerPosition();
	}

	void Update() {
		animator.SetFloat("xSpeed", xVelocity < 0 ? -xVelocity : xVelocity);
		xVelocity = Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0);
		sr.flipX = xVelocity < 0 || (sr.flipX && xVelocity == 0);

		if (Input.GetAxis("Vertical") > 0.5 && Input.GetAxis("Horizontal") == 0) {
			animator.SetBool("lookingUp", true);
		} else {
			animator.SetBool("lookingUp", false);
		}
	}

	void FixedUpdate() {
		rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);
		if (rb.velocity.y < -maxVerticalSpeed)
			rb.velocity = new Vector2(rb.velocity.x, -maxVerticalSpeed);
	}

}
