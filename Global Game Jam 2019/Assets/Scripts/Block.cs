using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public bool placing;
    public bool placed;

    public AudioClip[] sndBlockHits; 


    private void OnMouseUp() {
        if (!placed && GameController.inst.currentBlock == null) {
            GameController.inst.SetCurrentBlock(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        AudioManager.PlayOneShot(sndBlockHits[Random.Range(0, sndBlockHits.Length)]);
        if (placed) {
            if (collision.transform.CompareTag("Ground")) {
                GameController.inst.GameOver();
            }
        }
    }


}
