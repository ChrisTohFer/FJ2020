using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    public void StageComplete()
    {
        if (ScreenBlind.Enter())
            Invoke("StageCompletePrivate", 1);
        else
            StageCompletePrivate();
        
    }
    private void StageCompletePrivate()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
