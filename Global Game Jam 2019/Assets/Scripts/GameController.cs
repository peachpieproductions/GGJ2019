using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour {

    public static GameController inst;
    public bool gameOver;
    public GameObject[] blockPrefabs;
    public GameObject currentBlock;
    public GameObject communistCrate;
    public float currentRot;
    public Transform placementSprite;
    public Camera cam;
    public Vector2 mouseWorldPos;
    public GameObject gameOverText;
    public TextMeshProUGUI scoreText;
    public int score;
    public int highScore;

    private void Start() {
        inst = this;
        //currentBlock = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
        //placementSprite.GetComponent<SpriteRenderer>().sprite = currentBlock.GetComponent<SpriteRenderer>().sprite;
        StartCoroutine(GameLoop());
    }

    public void Update() {
        mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        placementSprite.position = mouseWorldPos;
        placementSprite.position += Vector3.back * .1f;

        if (Input.GetKeyDown(KeyCode.R)) {
            currentRot += 45;
            if (currentRot > 360) currentRot -= 360;
            placementSprite.eulerAngles = new Vector3(0, 0, currentRot);
        }

        if (Input.GetMouseButtonDown(0)) {
            if (currentBlock) {
                //Instantiate(currentBlock, mouseWorldPos, placementSprite.rotation);
                currentBlock.SetActive(true);
                currentBlock.GetComponent<Block>().placed = true;
                currentBlock.transform.rotation = placementSprite.rotation;
                currentBlock.transform.position = placementSprite.position;
                score += Mathf.RoundToInt(currentBlock.transform.position.y + 5) * 100;
                scoreText.text = score.ToString("#,#");
                //currentBlock = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                //placementSprite.GetComponent<SpriteRenderer>().sprite = currentBlock.GetComponent<SpriteRenderer>().sprite;
                currentRot = 0;
                placementSprite.eulerAngles = new Vector3(0, 0, currentRot);
                placementSprite.gameObject.SetActive(false);
                currentBlock = null;
                
            }
        }
        if (Input.GetMouseButton(1)) {
            cam.transform.position -= new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0) * Time.deltaTime * (7 + cam.orthographicSize);
        }
        cam.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * 10f;
        cam.orthographicSize = Mathf.Max(2,cam.orthographicSize - Input.mouseScrollDelta.y * .5f);
    }

    public void GameOver() {
        if (!gameOver) {
            gameOver = true;
            gameOverText.SetActive(true);
        }
    }

    public void SetCurrentBlock(GameObject block) {
        currentBlock = block;
        placementSprite.GetComponent<SpriteRenderer>().sprite = currentBlock.GetComponent<SpriteRenderer>().sprite;
        placementSprite.gameObject.SetActive(true);
    }

    IEnumerator GameLoop() {

        while(!gameOver) {

            yield return new WaitForSeconds(2f);

            var inst = Instantiate(communistCrate);
            inst.SetActive(true);

            bool finishedPlacingBlocks = true;

            do {
                finishedPlacingBlocks = true;
                foreach (Block b in FindObjectsOfType<Block>()) {
                    if (!b.placed) finishedPlacingBlocks = false;
                }
                if (FindObjectOfType<CommunismCrate>() != null) finishedPlacingBlocks = false;
                if (currentBlock) finishedPlacingBlocks = false; 
                yield return new WaitForSeconds(1f);
            } while (!finishedPlacingBlocks);


            yield return new WaitForSeconds(3f);
        }

    }
    

    



}
