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
    public GameObject scoreUnderline;
    public TextMeshProUGUI highscoreTopRightText;
    public int score;
    public int highScore;
    public GameObject newHighScoreEndScreen;
    public InputField inputField;
    public Text inputFieldText;
    public TextMeshProUGUI highscoreNameText;
    public TextMeshProUGUI newPointsText;
    public GameObject invisibleWall;
    public ParticleSystem sparksPs;
    public List<Vector2> sparksQueue;
    public int screenshotIndex;

    [Header("Sound Effects")]
    public AudioClip sndPlane;
    public AudioClip sndNewHighScore;
    public AudioClip sndScreenshot;


    bool newHighScoreAchieved;

    private void Start() {
        inst = this;
        highScore = PlayerPrefs.GetInt("highscore01");
        screenshotIndex = PlayerPrefs.GetInt("screenshotIndex");
        if (highScore > 0) highscoreTopRightText.text = "Highscore: " + highScore.ToString("#,#");
        else highscoreTopRightText.text = "";
        highscoreNameText.text = PlayerPrefs.GetString("highscore01name");
        StartCoroutine(GameLoop());
        StartCoroutine(Sparks());
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
                    currentBlock.layer = 9 >> 0;
                    currentBlock.transform.rotation = placementSprite.rotation;
                    currentBlock.transform.position = placementSprite.position;
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

    public void AddPoints(float blockHeight) {
        if (!scoreUnderline.activeSelf) scoreUnderline.SetActive(true);
        int newPoints = Mathf.RoundToInt(blockHeight + 5) * 100;
        if (!gameOver) score += newPoints;
        if (corNewPoints != null) StopCoroutine(corNewPoints);
        corNewPoints = StartCoroutine(NewPoints(newPoints));
        if (score > highScore && !newHighScoreAchieved && highScore > 0) { StartCoroutine(NewHighScore()); }
        scoreText.text = score.ToString("#,#");
    }

    public void GameOver() {
        if (!gameOver) {
            gameOver = true;
            gameOverText.SetActive(true);
            invisibleWall.SetActive(false);

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

    public void SparkEffect(Vector2 pos) {
        sparksQueue.Add(pos);
    }

    public void TakeScreenshot() {
        StartCoroutine(Screenshot());
    }
    IEnumerator Screenshot() {
        AudioManager.PlayOneShot(sndScreenshot,.8f,false);
        scoreText.transform.parent.gameObject.SetActive(false);
        yield return null;
        ScreenCapture.CaptureScreenshot("Screenshot_" + screenshotIndex + ".png");
        yield return null;
        scoreText.transform.parent.gameObject.SetActive(true);
        PlayerPrefs.SetInt("screenshotIndex",++screenshotIndex);
        Debug.Log(Application.persistentDataPath);
    }

    IEnumerator Sparks() {
        while (true) {
            if (sparksQueue.Count > 0) {
                sparksPs.transform.position = (Vector3)sparksQueue[0] + Vector3.back * .5f;
                sparksPs.Play();
                sparksQueue.RemoveAt(0);
                if (sparksQueue.Count > 40) sparksQueue.Clear();
            }
            yield return new WaitForSeconds(.01f);
        }
    }

    IEnumerator GameLoop() {

        while(!gameOver) {

            AudioManager.PlayOneShot(sndPlane, .8f, false);

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
        AudioManager.PlayOneShot(sndNewHighScore, 1, false);
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
