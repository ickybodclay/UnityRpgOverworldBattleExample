using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;

    private float speed = 3f;
    private Vector2 velocity;

	private void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	private void Update () {
        HandleInput();
	}

    private void HandleInput() {
        velocity.x = Input.GetAxis("Horizontal");
        velocity.y = Input.GetAxis("Vertical");
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    private void HandleMovement() {
        velocity.x *= speed * Time.fixedDeltaTime;
        velocity.y *= speed * Time.fixedDeltaTime;

        rb2d.MovePosition(rb2d.position + velocity);
    }
}
