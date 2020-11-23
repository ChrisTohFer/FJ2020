using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public float timeStart;
    public PlayerInput playerRef;
    public Rigidbody2D playerRb;
    public Text textBox;
    public Text highScore;
    public bool timerActive = false;
    public GameObject completeStageUI;
    public bool LevelComplete = false;
    public float LoadSceneTimeSeconds = 10f;

    // Start is called before the first frame update
    void Start()
    {
        textBox.text = timeStart.ToString("F2");
        highScore.text = PlayerPrefs.GetFloat("HighScore", 0).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive == false)
        {
            timeStart += Time.deltaTime;
            textBox.text = timeStart.ToString("F2");
            
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

