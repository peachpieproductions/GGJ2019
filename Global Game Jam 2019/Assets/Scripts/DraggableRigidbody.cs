using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableRigidbody : MonoBehaviour {


    Rigidbody2D rb;
    bool dragging;
    public AudioClip sndBlockImpact;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnMouseDown() {
        dragging = true;
    }

    private void FixedUpdate() {
        if (dragging) rb.velocity = (GameController.inst.mouseWorldPos - (Vector2)transform.position) * 15f;
    }

    private void Update() {
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        AudioManager.PlayOneShot(sndBlockImpact, Mathf.Max(.4f, rb.velocity.magnitude * 3));
        GameController.inst.SparkEffect(collision.contacts[0].point);
        if (collision.contacts.Length > 1) GameController.inst.SparkEffect(collision.contacts[1].point);
    }








}
