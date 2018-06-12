using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private float speed = 3f;

	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
        HandleInput();
	}

    private void HandleInput() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += h * speed * Time.deltaTime;
        pos.y += v * speed * Time.deltaTime;
        transform.position = pos;
    }
}
