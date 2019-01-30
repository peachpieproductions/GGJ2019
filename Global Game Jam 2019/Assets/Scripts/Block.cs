using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public bool placing;
    public bool placed;
    public bool scored;
    Rigidbody2D rb;

    public AudioClip[] sndBlockHits;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnMouseUp() {
        if (!placed && GameController.inst.currentBlock == null) {
            GameController.inst.SetCurrentBlock(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        AudioManager.PlayOneShot(sndBlockHits[Random.Range(0, sndBlockHits.Length)],Mathf.Max(.4f, rb.velocity.magnitude * 3));
        if (placed) {
            if (!scored) {
                scored = true;
                GameController.inst.AddPoints(transform.position.y);
            }
            if (!collision.transform.CompareTag("Family")) {
                GameController.inst.SparkEffect(collision.contacts[0].point);
                if (collision.contacts.Length > 1) GameController.inst.SparkEffect(collision.contacts[1].point);
            }

            if (collision.transform.CompareTag("Ground")) {
                GameController.inst.GameOver();
            }
        } else {
            if (collision.transform.CompareTag("Block")) {
                Vector2 dist = transform.position - collision.transform.position;
                if (dist.magnitude < 1.5f) rb.velocity += dist * .3f;
            }
        }
    }


}
