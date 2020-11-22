using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicTrigger : MonoBehaviour
{
    public UnityEvent triggered;
    public string requiredTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(requiredTag == "" || collision.gameObject.tag == requiredTag)
        {
            triggered.Invoke();
        }
    }
}
