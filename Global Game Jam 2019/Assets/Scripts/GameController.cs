using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController inst;
    public bool gameOver;
    public GameObject[] blockPrefabs;
    public GameObject currentBlock;
    public GameObject communistCrate;
    public GameObject[] gibs;
    public float currentRot;
    public Transform placementSprite;
    public Camera cam;
    public Vector2 mouseWorldPos;
    public GameObject gameOverText;
    public GameObject newHighScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreTopRightText;
    public int score;
    public int highScore;
    public GameObject newHighScoreEndScreen;
    public InputField inputField;
    public Text inputFieldText;
    public TextMeshProUGUI highscoreNameText;
    public TextMeshProUGUI newPointsText;


    bool newHighScoreAchieved;

    private void Start() {
        inst = this;
        highScore = PlayerPrefs.GetInt("highscore01");
        if (highScore > 0) highscoreTopRightText.text = "Highscore: " + highScore.ToString("#,#");
        else highscoreTopRightText.text = "";
        highscoreNameText.text = PlayerPrefs.GetString("highscore01name");
        StartCoroutine(GameLoop());
    }

    public void Update() {
        mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        placementSprite.position = mouseWorldPos;
        placementSprite.position += Vector3.back * .1f;

        if (gameOver) {
            if (Input.GetKeyDown(KeyCode.R) && !inputField.isFocused) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(2)) {
            if (currentBlock) {
                currentRot += 45;
                if (currentRot > 360) currentRot -= 360;
                placementSprite.eulerAngles = new Vector3(0, 0, currentRot);
            }
        }

        //Erase Files
        if (Input.GetKey(KeyCode.RightControl)) {
            if (Input.GetKey(KeyCode.RightShift)) {
                if (Input.GetKeyDown(KeyCode.Delete)) {
                    PlayerPrefs.DeleteKey("highscore01");
                    PlayerPrefs.DeleteKey("highscore01name");
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            if (currentBlock) {
                if (mouseWorldPos.y > 0 || mouseWorldPos.x > -5) { //cant drop block in crate area
                    currentBlock.SetActive(true);
                    currentBlock.GetComponent<Block>().placed = true;
                    currentBlock.transform.rotation = placementSprite.rotation;
                    currentBlock.transform.position = placementSprite.position;
                    int newPoints = Mathf.RoundToInt(currentBlock.transform.position.y + 5) * 100;
                    if (!gameOver) score += newPoints;
                    if (corNewPoints != null) StopCoroutine(corNewPoints);
                    corNewPoints = StartCoroutine(NewPoints(newPoints));
                    if (score > highScore && !newHighScoreAchieved && highScore > 0) { StartCoroutine(NewHighScore()); }
                    scoreText.text = score.ToString("#,#");
                    currentRot = 0;
                    placementSprite.eulerAngles = new Vector3(0, 0, currentRot);
                    placementSprite.gameObject.SetActive(false);
                    currentBlock = null;
                }
            }
        }
        if (Input.GetMouseButton(1)) {
            cam.transform.position -= new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0) * Time.deltaTime * (7 + cam.orthographicSize);
        }
        if (!inputField.isFocused) cam.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * 10f;
        cam.orthographicSize = Mathf.Max(2,cam.orthographicSize - Input.mouseScrollDelta.y * .5f);
    }

    public void GameOver() {
        if (!gameOver) {
            gameOver = true;
            gameOverText.SetActive(true);

            if (score > highScore) {
                highScore = score;
                PlayerPrefs.SetInt("highscore01", highScore);
                newHighScoreEndScreen.SetActive(true);
            }
        }
    }

    public void SetCurrentBlock(GameObject block) {
        currentBlock = block;
        placementSprite.GetComponent<SpriteRenderer>().sprite = currentBlock.GetComponent<SpriteRenderer>().sprite;
        placementSprite.transform.localScale = currentBlock.transform.localScale;
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

    IEnumerator NewHighScore() {
        newHighScoreAchieved = true;
        newHighScoreText.SetActive(true);
        yield return new WaitForSeconds(5f);
        newHighScoreText.SetActive(false);
    }

    Coroutine corNewPoints;
    IEnumerator NewPoints(int amount) {
        newPointsText.text = "+" + amount.ToString("#,#");
        yield return new WaitForSeconds(3f);
        newPointsText.text = "";
    }

    public void SaveNewHighScoreName() {
        if (inputFieldText.text != "") {
            PlayerPrefs.SetString("highscore01name", inputFieldText.text);
            highscoreNameText.text = inputFieldText.text;
        }
    }

    



}
