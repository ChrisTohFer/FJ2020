using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    bool exiting = false;
    private void Update()
    {
        if(!exiting && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            exiting = true;
            ScreenBlind.Enter();
            Invoke("LoadMenu", 1f);
        }
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
