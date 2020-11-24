using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager gminstance;
    public int collectableScore;
    public Text collectableText;
    public float timeStart;
    public PlayerInput playerRef;
    public Rigidbody2D playerRb;
    public Text currentTime;
    public Text highScore;
    public bool timerActive = false;
    public GameObject completeStageUI;
    public bool LevelComplete = false;
    public float LoadSceneTimeSeconds = 10f;

    // Start is called before the first frame update
    void Start()
    {
        if (gminstance == null)
        {
            gminstance = this;
        }

        if (highScore == null||currentTime == null)
        {
            print("EMPTY OBJECT MUST BE THE TUTORIAL LEVEL");
        }
        else
        {
            highScore.text = PlayerPrefs.GetFloat("HighScore", 0).ToString();
            currentTime.text = timeStart.ToString("F2");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive == false && currentTime != null)
        {
            timeStart += Time.deltaTime;
            currentTime.text = timeStart.ToString("F2");
            
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            // set game state to done 
            LevelComplete = true;
            LevelCompleted();

        }
    }

    public void ChangeScore(int followerValue)
    {
        collectableScore += followerValue;
        collectableText.text = "X" + collectableScore.ToString();
    }

    public void LevelCompleted()
    {
        if(LevelComplete == true)
        {
            // store our time 
            
            if (timeStart < PlayerPrefs.GetFloat("HighScore", 0))
            {
                PlayerPrefs.SetFloat("HighScore", timeStart);
                
            }
            else
            {
                // pause the timer
                timerActive = !timerActive;

                // disable player control 
                playerRb.velocity = Vector3.zero;
                playerRef = Movement.playerTransform.GetComponent<PlayerInput>();
                playerRef.enabled = false;

                // Load the score screen
                completeStageUI.SetActive(true);

                // load next scene after short delay
                //Invoke("LoadNextLevel", LoadSceneTimeSeconds);
            }



        }
    }

    public void LoadNextLevel()
    {
        // load next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

