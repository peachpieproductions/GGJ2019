using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Playables;

public class GameController : MonoBehaviour {

    public static GameController inst;
    public bool gameStarted;
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
    public PlayableDirector intro;
    public bool introComplete;
    public PhysicsTitle physicsTitle;
    public GameObject tutorialText;
    public Options optionsMenu;
    public Transform tutorial;
    public GameObject hudButtons;

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

    [Header("Creative Mode")]
    public bool creativeMode;
    public GameObject creativeModeMenuButton;
    public GameObject creativeModeUI;
    public GameObject blockPicker;
    public Button blockSelectionTemplate;

    float cooldown;
    bool newHighScoreAchieved;

    private void Awake() {
        physicsTitle.gameObject.SetActive(false);
        #if UNITY_ANDROID
            touch = true;
        #endif
    }

    private void Start() {
        inst = this;
        highScore = PlayerPrefs.GetInt("highscore01");
        creativeModeMenuButton.SetActive(highScore > 100000);
        screenshotIndex = PlayerPrefs.GetInt("screenshotIndex");
        if (highScore > 0) highscoreTopRightText.text = "Highscore: " + highScore.ToString("#,#");
        else highscoreTopRightText.text = "";
        highscoreNameText.text = PlayerPrefs.GetString("highscore01name");

        optionsMenu.LoadSettings();

        if (touch) {
            touchRestartButton.SetActive(true);
        }

        StartCoroutine(Sparks());

    }

    private void LateUpdate() {
        if (touch) {
            if (Input.GetMouseButtonUp(0) && touchTimer < .2f && !dragging) {
                if (!creativeMode) PlaceBlock();
                else PlaceBlockCreativeMode();
            }
        } 
        
        if (Input.GetKeyDown(KeyCode.F10)) {
            TakeScreenshot();
        }
    }

    public void Update() {

        //Set mouse world position
        mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));

        if (gameStarted) {

            //cooldown
            if (cooldown > 0) cooldown -= Time.deltaTime;

            //Set placement sprite
            if (!touch) {
                placementSprite.position = mouseWorldPos;
                //placementSprite.position += Vector3.back * .1f;
            } else {
                if (currentBlock && Input.touchCount == 1) {
                    placementSprite.position += (Vector3)Input.GetTouch(0).deltaPosition * Time.deltaTime;
                }
            }


            //Game Over
            if (gameOver) {

                //Set highscore name
                if (Input.GetKeyDown(KeyCode.Return)) {
                    SaveNewHighScoreName();
                }

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
                if (Input.GetMouseButtonDown(0)) {
                    if (!creativeMode) PlaceBlock();
                    else PlaceBlockCreativeMode();
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
                if (!creativeMode || !blockPicker.activeSelf || creativeMode && mouseWorldPos.y > cam.transform.position.y - cam.orthographicSize * .6f) {
                    cam.orthographicSize = cam.orthographicSize - Input.mouseScrollDelta.y * .5f;
                }
            }

            //Camera Bounds
            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, -20, 20), Mathf.Max(-8, cam.transform.position.y), cam.transform.position.z);
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 2f, 40f);

            //Sin wave for helper arrow
            if (helperArrow.activeSelf) helperArrow.transform.GetChild(0).localPosition = new Vector3(0, Mathf.Sin(Time.time * 5) * .75f, 0);

        }
        else {
            if (!introComplete) {
                if (Input.GetMouseButtonDown(0)) {
                    introComplete = true;
                    intro.time = intro.duration;
                    
                }
            }
        }
    }

    public void StartGame(bool creativeModeOn = false) {
        creativeMode = creativeModeOn;
        gameStarted = true;
        intro.gameObject.SetActive(false);
        physicsTitle.gameObject.SetActive(false);
        if (!touch) tutorialText.SetActive(true);
        hudButtons.SetActive(true);

        if (creativeMode) {
            creativeModeUI.SetActive(true);
            StartCreativeMode();
        } else {
            highscoreTopRightText.transform.parent.gameObject.SetActive(true);
            StartCoroutine(HelperArrow());
        }

        StartCoroutine(GameLoop());
        
    }

    public void PlaceBlock() {
        if (currentBlock && cooldown <= 0) {
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
        if (!gameOver && !creativeMode) {
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
        if (!creativeMode) {
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
    }

    public void SetCurrentBlock(GameObject block) {
        if (touch) cooldown = 1f;
        else cooldown = .3f;
        helperArrow.SetActive(false);
        currentBlock = block;
        placementSprite.GetComponent<SpriteRenderer>().sprite = currentBlock.GetComponent<SpriteRenderer>().sprite;
        placementSprite.transform.localScale = currentBlock.transform.localScale;
        placementSprite.gameObject.SetActive(true);
        if (touch) {
            touchTimer++;
            touchRotateButton.SetActive(true);
            float placementHeight = -4;
            foreach(Block b in FindObjectsOfType<Block>()) {
                var ypos = b.transform.GetComponent<Collider2D>().bounds.extents.y + b.transform.GetComponent<Collider2D>().bounds.center.y;
                if (b.placed && ypos > placementHeight) {
                    placementHeight = ypos;
                }
            }
            placementSprite.transform.position = new Vector3 (0, placementHeight + 2f, currentBlock.transform.position.z);
            if (placementHeight + 2.5f > cam.transform.position.y + cam.orthographicSize)
                StartCoroutine(TweenCameraTo(new Vector2 (cam.transform.position.x, placementHeight - cam.orthographicSize * .25f)));
        }
    }

    IEnumerator TweenCameraTo(Vector2 pos) {
        bool cancel = false;
        while(!cancel) {
            if (Input.GetMouseButtonDown(0)) cancel = true;
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(pos.x, pos.y, cam.transform.position.z), Time.deltaTime * 5f);
            yield return null;
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

        if (!creativeMode) {
            while (!gameOver) {

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
        } else {
            while (true) {

                yield return null;
            }
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
        if (newHighScoreEndScreen.activeSelf) {
            if (inputFieldText.text != "") {
                PlayerPrefs.SetString("highscore01name", inputFieldText.text);
                highscoreNameText.text = inputFieldText.text;
            }
            highscoreTopRightText.text = "Highscore: " + score.ToString("#,#");
            newHighScoreEndScreen.SetActive(false);
        }
    }

    public void ClickSound(float freq = 1) {
        AudioManager.PlayOneShot(sndRotateBlock, .5f, false, freq);
    }

    public void StartTutorial() {
        tutorial.gameObject.SetActive(true);
        foreach(Transform t in tutorial) {
            t.gameObject.SetActive(true);
        }
    }

    public void StartCreativeMode() {
        foreach(GameObject go in blockPrefabs) {
            var inst = Instantiate(blockSelectionTemplate, blockSelectionTemplate.transform.parent);
            inst.transform.GetChild(0).GetComponent<Image>().sprite = go.GetComponent<SpriteRenderer>().sprite;
        }
        blockSelectionTemplate.gameObject.SetActive(false);
        invisibleWall.SetActive(false);
    }

    public void SetCurrentBlockByIndex(Transform index) {
        if (touch) cooldown = 1f;
        else cooldown = .3f;
        currentBlock = blockPrefabs[index.GetSiblingIndex() - 1];
        placementSprite.GetComponent<SpriteRenderer>().sprite = currentBlock.GetComponent<SpriteRenderer>().sprite;
        placementSprite.transform.localScale = currentBlock.transform.localScale;
        placementSprite.gameObject.SetActive(true);
        if (touch) {
            touchTimer++;
            touchRotateButton.SetActive(true);
            placementSprite.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, currentBlock.transform.position.z);
        }
    }

    public void PlaceBlockCreativeMode() {
        if (currentBlock && cooldown <= 0) {
            if (placementSprite.position.y > -4) {
                if (placementSprite.position.y > cam.transform.position.y - cam.orthographicSize * .6 && blockPicker.activeSelf ||
                    placementSprite.position.y > cam.transform.position.y - cam.orthographicSize * .87 && !blockPicker.activeSelf) {
                    AudioManager.PlayOneShot(sndPlaceBlock, .35f);
                    var newBlock = Instantiate(currentBlock, placementSprite.position, placementSprite.rotation);
                    newBlock.GetComponent<Block>().placed = true;
                    newBlock.layer = 9 >> 0;
                }
            }
        }
    }



}
