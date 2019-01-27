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
                for (var i = 0; i < 5; i++) {
                    var index = Random.Range(0, GameController.inst.gibs.Length);
                    var inst = Instantiate(GameController.inst.gibs[index],
                        transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                    inst.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
                    inst.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-50, 50);
                    if (index != 5) inst.transform.localScale *= Random.Range(1f, 2f);
                }
                GetComponent<BoxCollider2D>().size *= .5f;
                GetComponent<SpriteRenderer>().sprite = deadSprite;
                GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-4, 4), Random.Range(0, 5));
                GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-50, 50);
                GameController.inst.GameOver();
            }
        }
    }




}
