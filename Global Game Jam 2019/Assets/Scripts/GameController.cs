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
    public GameObject helperArrow;

    //Touch / Mobile
    public static bool touch;
    public static float touchTimer;
    [Header("Touch / Mobile")]
    public GameObject touchRestartButton;
    public GameObject touchRotateButton;
    public Vector2 startTouchPos;
    bool twoFingersDown;
    float prevTwoFingerDist;
    bool dragging;

    //SNDFX
    [Header("Sound Effects")]
    public AudioClip sndPlane;
    public AudioClip sndNewHighScore;
    public AudioClip sndScreenshot;
    public AudioClip sndPlaceBlock;
    public AudioClip sndCenterBlock;
    public AudioClip sndRotateBlock;


    bool newHighScoreAchieved;

    private void Awake() {
        #if UNITY_ANDROID
            touch = true;
        #endif
    }

    private void Start() {
        inst = this;
        highScore = PlayerPrefs.GetInt("highscore01");
        screenshotIndex = PlayerPrefs.GetInt("screenshotIndex");
        if (highScore > 0) highscoreTopRightText.text = "Highscore: " + highScore.ToString("#,#");
        else highscoreTopRightText.text = "";
        highscoreNameText.text = PlayerPrefs.GetString("highscore01name");

        if (touch) {
            touchRestartButton.SetActive(true);
        }

        StartCoroutine(GameLoop());
        StartCoroutine(Sparks());
        StartCoroutine(HelperArrow());
    }

    private void LateUpdate() {
        if (touch) {
            if (Input.GetMouseButtonUp(0) && touchTimer < .2f && !dragging) {
                PlaceBlock();
            }
        } 
    }

    public void Update() {

        //Set mouse world position
        mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));

        //Set placement sprite
        if (!touch) {
            placementSprite.position = mouseWorldPos;
            //placementSprite.position += Vector3.back * .1f;
        } else {
            if (currentBlock && Input.touchCount == 1) {
                placementSprite.position += (Vector3) Input.GetTouch(0).deltaPosition * Time.deltaTime;
            }
        }


        //Game Over
        if (gameOver) {

            //Restart Game
            if (Input.GetKeyDown(KeyCode.R) && !inputField.isFocused) {
                RestartGame();
            }
            
        }

        //Erase Files
        if (Input.GetKey(KeyCode.RightControl)) {
            if (Input.GetKey(KeyCode.RightShift)) {
                if (Input.GetKeyDown(KeyCode.Delete)) {
                    PlayerPrefs.DeleteKey("highscore01");
                    PlayerPrefs.DeleteKey("highscore01name");
                    PlayerPrefs.DeleteKey("screenshotIndex");
                    AudioManager.PlayOneShot(sndNewHighScore);
                }
            }
        }

        //Place Current Block
        if (!touch) {
            if (Input.GetMouseButtonUp(0)) {
                PlaceBlock();
            }
        }

        //Rotate Current Block
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(2)) {
            if (Input.GetKey(KeyCode.LeftShift)) RotateBlock(false);
            else RotateBlock(true);
        }


        //Camera Control
        if (touch) {

            if (Input.touchCount > 0) {
                touchTimer += Time.deltaTime;

                if (Input.touches[0].deltaPosition.magnitude > .4f) dragging = true;

                //Camera Zoom (touch)
                float fingerDistDelta = 0f;
                if (Input.touchCount > 1) {
                    if (!twoFingersDown) {
                        twoFingersDown = true;
                        prevTwoFingerDist = (Input.touches[0].position - Input.touches[1].position).magnitude;
                    }
                    fingerDistDelta = prevTwoFingerDist - (Input.touches[0].position - Input.touches[1].position).magnitude;
                    if (Mathf.Abs(fingerDistDelta) > 2) cam.orthographicSize = cam.orthographicSize + fingerDistDelta * Time.deltaTime * .5f;
                    prevTwoFingerDist = (Input.touches[0].position - Input.touches[1].position).magnitude;
                } else {
                    twoFingersDown = false;
                }

                //Camera Movement (touch)
                if (Input.touchCount > 1 || Input.touchCount == 1 && currentBlock == null) {
                    if (Input.GetTouch(0).deltaPosition.magnitude > 2f) {
                        cam.transform.position -= (Vector3)Input.GetTouch(0).deltaPosition * Time.deltaTime * .8f * Mathf.Max(1, (cam.orthographicSize - 6) * .2f);
                    }
                }
                
            }
            if (Input.GetMouseButtonDown(0)) { //Touch Down
                touchTimer = 0;
                dragging = false;
            }
            if (Input.GetMouseButtonUp(0)) { //Touch Up
                
            }
        } else {
            //Camera Movement
            if (Input.GetMouseButton(1)) {
                cam.transform.position -= new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0) * Time.deltaTime * (7 + cam.orthographicSize);
            }
            if (!inputField.isFocused) cam.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * 10f;

            //Camera Zoom
            cam.orthographicSize = cam.orthographicSize - Input.mouseScrollDelta.y * .5f;
        }

        //Camera Bounds
        cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, -20, 20), Mathf.Max(-8, cam.transform.position.y), cam.transform.position.z);
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 2f, 40f);

        //Sin wave for helper arrow
        if (helperArrow.activeSelf) helperArrow.transform.GetChild(0).localPosition = new Vector3(0, Mathf.Sin(Time.time * 5) * .75f, 0);
    }

    public void PlaceBlock() {
        if (currentBlock) {
            if (placementSprite.position.y > 0 || placementSprite.position.x > -6) { //cant drop block in crate area
                AudioManager.PlayOneShot(sndPlaceBlock, .35f);
                currentBlock.SetActive(true);
                currentBlock.GetComponent<Block>().placed = true;
                currentBlock.layer = 9 >> 0;
                currentBlock.transform.rotation = placementSprite.rotation;
                currentBlock.transform.position = placementSprite.position;
                currentRot = 0;
                placementSprite.eulerAngles = new Vector3(0, 0, currentRot);
                placementSprite.gameObject.SetActive(false);
                currentBlock = null;
                if (touch) {
                    touchRotateButton.SetActive(false);
                }
            }
        }
    }

    public void RotateBlock(bool left = true) {
        if (currentBlock) {
            if (left) AudioManager.PlayOneShot(sndRotateBlock,.7f,false,1.2f);
            else AudioManager.PlayOneShot(sndRotateBlock, .7f,false);
            touchTimer++;
            if (left) {
                currentRot += 45;
                if (currentRot > 360) currentRot -= 360;
            } else {
                currentRot -= 45;
                if (currentRot < 0) currentRot += 360;
            }
            placementSprite.eulerAngles = new Vector3(0, 0, currentRot);
        }
    }

    public void CenterBlockToScreen() {
        if (currentBlock) {
            AudioManager.PlayOneShot(sndCenterBlock, .7f);
            touchTimer++;
            placementSprite.position = new Vector3(cam.transform.position.x, cam.transform.position.y, placementSprite.position.z);
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddPoints(float blockHeight) {
        if (!gameOver) {
            if (!scoreUnderline.activeSelf) scoreUnderline.SetActive(true);
            int newPoints = Mathf.RoundToInt(blockHeight + 5) * 100;
            score += newPoints;
            if (corNewPoints != null) StopCoroutine(corNewPoints);
            corNewPoints = StartCoroutine(NewPoints(newPoints));
            if (score > highScore && !newHighScoreAchieved && highScore > 0) { StartCoroutine(NewHighScore()); }
            scoreText.text = score.ToString("#,#");
        }
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
        helperArrow.SetActive(false);
        currentBlock = block;
        placementSprite.GetComponent<SpriteRenderer>().sprite = currentBlock.GetComponent<SpriteRenderer>().sprite;
        placementSprite.transform.localScale = currentBlock.transform.localScale;
        placementSprite.gameObject.SetActive(true);
        if (touch) {
            touchTimer++;
            touchRotateButton.SetActive(true);
            placementSprite.transform.position = new Vector3 (currentBlock.transform.position.x, -1f, currentBlock.transform.position.z);
        }
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

    bool waiting;
    IEnumerator GameLoop() {

        while(!gameOver) {

            AudioManager.PlayOneShot(sndPlane, .8f, false);

            yield return new WaitForSeconds(2f);

            var inst = Instantiate(communistCrate);
            inst.SetActive(true);

            bool finishedPlacingBlocks = true;
            waiting = false;

            do {
                finishedPlacingBlocks = true;
                foreach (Block b in FindObjectsOfType<Block>()) {
                    if (!b.placed) { finishedPlacingBlocks = false; if (currentBlock == null) waiting = true; break; }
                }
                if (FindObjectOfType<CommunismCrate>() != null) finishedPlacingBlocks = false;
                if (currentBlock) finishedPlacingBlocks = false; 
                yield return new WaitForSeconds(1f);
            } while (!finishedPlacingBlocks);


            yield return new WaitForSeconds(3f);
        }

    }

    IEnumerator HelperArrow() {
        float timer = 12f;
        bool arrowActive = false;
        while (true) {
            if (currentBlock == null && waiting && !gameOver) {
                timer -= 2f;
                if (timer <= 0) {
                    if (!arrowActive) {
                        helperArrow.SetActive(arrowActive = true);
                        foreach (Block b in FindObjectsOfType<Block>()) {
                            if (!b.placed) { helperArrow.transform.position = b.transform.position + Vector3.up * 3f; break; }
                        }
                        AudioManager.PlayOneShot(sndCenterBlock, .6f, false);
                    }
                }
            } else {
                timer = 12f;
                if (arrowActive) {
                    helperArrow.SetActive(arrowActive = false);
                }
            }
            yield return new WaitForSeconds(2f);
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
        highscoreTopRightText.text = "Highscore: " + score.ToString("#,#");
        newHighScoreEndScreen.SetActive(false);
    }

    



}
