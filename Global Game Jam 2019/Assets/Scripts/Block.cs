using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public bool placing;
    public bool placed;


    private void OnMouseUp() {
        if (!placed) {
            GameController.inst.SetCurrentBlock(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (placed) {
            if (collision.transform.CompareTag("Ground")) {
                GameController.inst.GameOver();
            }
        }
    }


}
