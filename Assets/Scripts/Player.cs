using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPersistable<Player.Data> {

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;

    private float speed = 3f;
    private Vector2 velocity;

    [Serializable]
    public class Data {
        public OverworldData overworldData;
        public List<BattleData> battleDataList;
    }

    public Data data;

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

        if (velocity.x < 0f && !spriteRenderer.flipX) {
            spriteRenderer.flipX = true;
        }
        else if (velocity.x > 0f && spriteRenderer.flipX) {
            spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    private void HandleMovement() {
        velocity.x *= speed * Time.fixedDeltaTime;
        velocity.y *= speed * Time.fixedDeltaTime;

        rb2d.MovePosition(rb2d.position + velocity);
    }

    public void SaveData() {
        data.overworldData.Position = transform.position;
    }

    public void LoadData(Data data) {
        if (data != null) {
            this.data = data;
            RestoreOverworldState();
        }
    }

    private void RestoreOverworldState() {
        transform.position = data.overworldData.Position;
    }
}
