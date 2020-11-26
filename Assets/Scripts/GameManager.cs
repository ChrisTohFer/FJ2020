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

    // Start is called before the first frame update
    void Start()
    {
        if (gminstance == null)
        {
            gminstance = this;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive == false)
        {
            timeStart += Time.deltaTime;
            
            if(currentTime != null)
            {
                currentTime.text = timeStart.ToString("F2");
            }    
            
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            // set game state to done 
            LevelComplete = true;
            timerActive = true;
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
            if (timeStart < PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name+"A", 1000))
            {
                PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name + "A", timeStart);

            }
            if(highScore != null)
            {
                highScore.text = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name + "A", 0).ToString();
            }
                

            // disable player control 
            playerRb.velocity = Vector3.zero;
            playerRef = Movement.playerTransform.GetComponent<PlayerInput>();
            playerRef.enabled = false;
            

            // Load the score screen
            completeStageUI.SetActive(true);

            // load next scene after short delay
            //Invoke("LoadNextLevel", LoadSceneTimeSeconds);
            // store our time 

        }
    }

    public void LoadNextLevel()
    {
        // load next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

