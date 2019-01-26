using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject[] blockPrefabs;
    public GameObject currentBlock;
    public Transform placementSprite;
    public Camera cam;
    public Vector2 mouseWorldPos;

    private void Start() {
        currentBlock = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
        placementSprite.GetComponent<SpriteRenderer>().sprite = currentBlock.GetComponent<SpriteRenderer>().sprite;
    }

    public void Update() {
        mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        placementSprite.position = mouseWorldPos;
        placementSprite.position += Vector3.back * .1f;

        if (Input.GetMouseButtonDown(0)) {
            Instantiate(currentBlock, mouseWorldPos, Quaternion.identity);
            currentBlock = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
            placementSprite.GetComponent<SpriteRenderer>().sprite = currentBlock.GetComponent<SpriteRenderer>().sprite;
        }
        if (Input.GetMouseButton(1)) {
            cam.transform.position -= new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0) * Time.deltaTime * (7 + cam.orthographicSize);
        }
        cam.orthographicSize = Mathf.Max(2,cam.orthographicSize - Input.mouseScrollDelta.y * .5f);
    }

    

    



}
