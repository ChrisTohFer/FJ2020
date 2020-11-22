using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneChange : MonoBehaviour
{
    public string levelOne;
    public string slingTower;

    bool changing = false;

    public void LoadLevelOne()
    {
        if (changing)
            return;
        changing = true;

        ScreenBlind.Enter();
        Invoke("LoadLevelOnePrivate", 1f);
    }
    void LoadLevelOnePrivate()
    {
        SceneManager.LoadScene(levelOne);
    }

    public void LoadSlingTower()
    {
        if (changing)
            return;
        changing = true;

        ScreenBlind.Enter();
        Invoke("LoadSlingTowerPrivate", 1f);
    }
    void LoadSlingTowerPrivate()
    {
        SceneManager.LoadScene(slingTower);
    }

    public void Exit()
    {
        if (changing)
            return;
        changing = true;

        ScreenBlind.Enter();
        Invoke("ExitPrivate", 1f);
    }
    private void ExitPrivate()
    {
        Application.Quit();
    }
}
