using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float timeStart;
    public Text textBox;
    public Text highScore;
    public bool LevelComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        textBox.text = timeStart.ToString("F2");
        highScore.text = PlayerPrefs.GetFloat("HighScore", 0).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        timeStart += Time.deltaTime;
        textBox.text = timeStart.ToString("F2");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            // set game state to done 
            LevelComplete = true;

            // store our time 
            if (timeStart < PlayerPrefs.GetFloat("HighScore", 0))
            {
                PlayerPrefs.SetFloat("HighScore", timeStart);
            }
            
            // pause the timer

            // Load the score screen


        }
    }


}

