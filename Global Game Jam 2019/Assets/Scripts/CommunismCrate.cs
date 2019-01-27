using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunismCrate : MonoBehaviour {






    private void OnMouseDown() {
        Destroy(gameObject);
        for(var i = 0; i < 4; i++) {
            Instantiate(GameController.inst.blockPrefabs[Random.Range(0, GameController.inst.blockPrefabs.Length)], 
                transform.position + Vector3.left * (i + 1), Quaternion.Euler(0,0,90));
        }
    }


}
