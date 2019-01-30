using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunismCrate : MonoBehaviour {


    public AudioClip sndSmash;
    public AudioClip sndHit;



    private void OnMouseDown() {
        Destroy(gameObject);
        AudioManager.PlayOneShot(sndSmash);
        for(var i = 0; i < 4; i++) {
            var index = Random.Range(0, GameController.inst.blockPrefabs.Length);
            var inst = Instantiate(GameController.inst.blockPrefabs[index], 
                transform.position + Vector3.left * (-1 + i * 1.3f) + Vector3.back * .2f, Quaternion.Euler(0,0,90));
            if (Random.value > .5f && (index == 3 || index == 5 || index == 11 || index == 14 || index == 16)) inst.transform.localScale = new Vector3(-1, 1, 1);
            inst.GetComponent<Rigidbody2D>().velocity += (Vector2) Vector3.up * Random.Range(-2f, 6f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        AudioManager.PlayOneShot(sndHit);
    }


}
