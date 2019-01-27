using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyMember : MonoBehaviour {

    public bool dead;
    public Sprite deadSprite;




    private void OnCollisionEnter2D(Collision2D collision) {
        if (!dead) {
            if (collision.transform.CompareTag("Block")) {
                dead = true;
                GetComponent<SpriteRenderer>().sprite = deadSprite;
                GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-4, 4), Random.Range(0, 5));
                GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-50, 50);
                GameController.inst.GameOver();
            }
        }
    }




}
