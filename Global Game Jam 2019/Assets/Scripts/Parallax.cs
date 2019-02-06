using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

    
    public float offsetMult = 1;
    public float animateOffsetSpeed;
    public Transform cam;

    Vector3 startPos;
    float xOffset;
    Material mainMat;

    private void Awake() {
        startPos = transform.position;
        mainMat = GetComponent<MeshRenderer>().material;
    }


    private void Update() {
        transform.position = new Vector3(cam.position.x * offsetMult, startPos.y, transform.position.z);
        if (animateOffsetSpeed != 0) {
            xOffset += animateOffsetSpeed * Time.deltaTime * .1f;
            mainMat.SetTextureOffset("_MainTex", new Vector2 (xOffset, 0));
        }
    }






}
