using System.Collections;
using UnityEngine;
using System;

public class MoleBehaviour : MonoBehaviour {

	public float regularSpeed;
	public float attackSpeedMultiplier;
	public float attackDistance;
	[Range(0, 1)] public float moleAgility;
	public int attackDamage;
	public float delay;
	public Vector2 attackSizeBox = new Vector2(0.5f, 0.36f);
	public Vector2 attackOffsetBox = new Vector2(0f, 0.30f);
	public BoxCollider2D physicsCollider;
	public BoxCollider2D triggerCollider;

	private SpriteRenderer sr;
	private Rigidbody2D rb;
	private Animator anim;
	private Transform transf;
	private Transform player;
	private LayerMask groundLayerIndex;
	private LayerMask damageableObjLayer;
	private float speed;
	private double pDistance;
	private bool attacking;
	private bool telegraphing;


	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		transf = GetComponent<Transform>();
		player = GameObject.Find("Player").transform;
		groundLayerIndex = LayerMask.NameToLayer("Foreground");
		damageableObjLayer = LayerMask.NameToLayer("DamageableObjects");
		sr = GetComponent<SpriteRenderer>();

		speed = regularSpeed;
	}

	// Update is called once per frame
	void Update() {

		pDistance = playerDistance();

		if (pDistance < attackDistance && !attacking && !telegraphing) {
			telegraphing = true;
			StartCoroutine("delayedAttack");
		}

		if (attacking && UnityEngine.Random.value < moleAgility) {
			GoTowardsPlayer();
		}

		if ((reachBorder() || reachWall()) && !attacking) {
			speed = -speed;
		}

		if (!telegraphing)
			sr.flipX = speed < 0;

		//Animação
		anim.SetBool("Run", attacking);

	}
	
	public void SetHorizontalHitboxes() {
		physicsCollider.size = attackSizeBox;
		physicsCollider.offset = attackOffsetBox;
	}

	private void FixedUpdate() {
		rb.velocity = new Vector2(speed, rb.velocity.y);
	}

	private double playerDistance() {
		float xDiff = transf.position.x - player.position.x,
			  yDiff = transf.position.y - player.position.y;
		return Math.Sqrt(xDiff*xDiff + yDiff*yDiff);
	}

	private bool reachBorder() {
		Vector2 leftBottom = physicsCollider.bounds.min;
		Vector2 rightBottom = new Vector2(physicsCollider.bounds.max.x, physicsCollider.bounds.min.y);
		if (speed < 0)
			return !Physics2D.Raycast(leftBottom, Vector2.down, 0.2f, (1 << groundLayerIndex)|(1 << damageableObjLayer));
		return !Physics2D.Raycast(rightBottom, Vector2.down, 0.2f, (1 << groundLayerIndex) | (1 << damageableObjLayer));
	}

	private bool reachWall() {
		Vector2 rightOrigin = new Vector2(physicsCollider.bounds.max.x, physicsCollider.bounds.center.y);
		Vector2 leftOrigin = new Vector2(physicsCollider.bounds.min.x, physicsCollider.bounds.center.y);
		if (speed < 0)
			return Physics2D.Raycast(leftOrigin, Vector2.left, 0.2f, (1 << groundLayerIndex) | (1 << damageableObjLayer));
		return Physics2D.Raycast(rightOrigin, Vector2.right, 0.2f, (1 << groundLayerIndex) | (1 << damageableObjLayer));
	}

	private void GoTowardsPlayer() {
		float diff = player.position.x - transf.position.x;
		if (diff / speed < 0) speed = -speed;
	}

	private IEnumerator delayedAttack() {
		speed = 0f;
		sr.flipX = player.position.x < transf.position.x;
		anim.speed = 0f;
		yield return new WaitForSeconds(delay);
		SetHorizontalHitboxes();
		speed = regularSpeed * attackSpeedMultiplier;
		anim.speed = 1f;
		telegraphing = false;
		attacking = true;
		GoTowardsPlayer();
	}

}
