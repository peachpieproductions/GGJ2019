using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTitle : MonoBehaviour {


    List<Rigidbody2D> letters = new List<Rigidbody2D>();

    private void Start() {

        foreach(Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>()) {
            letters.Add(rb);
        }
        StartCoroutine(DropLetters());

    }

    IEnumerator DropLetters() {

        while (letters.Count > 0) {
            AudioManager.PlayOneShot(GameController.inst.sndPlaceBlock, .25f);
            letters[0].isKinematic = false;
            letters[0].velocity = Vector2.down * Random.Range(1, 15);
            letters[0].angularVelocity = Random.Range(-30, 30);
            letters.RemoveAt(0);

            yield return new WaitForSeconds(.2f);
        }

    }





}
