using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBlind : MonoBehaviour
{
    static ScreenBlind singleton;

    public float distance = 15f;
    public float duration = 1f;
    public SpriteRenderer[] renderers;

    private void Awake()
    {
        singleton = this;
        foreach(var v in renderers)
        {
            v.enabled = true;
        }
    }

    private void Start()
    {
        Exit();
    }

    public static void Enter()
    {
        singleton.StopAllCoroutines();
        singleton.StartCoroutine(singleton.Transition(Vector3.up * singleton.distance, Vector3.zero, singleton.duration));
    }

    public static void Exit()
    {
        singleton.StopAllCoroutines();
        singleton.StartCoroutine(singleton.Transition(Vector3.zero, Vector3.down * singleton.distance, singleton.duration));
    }

    //Coroutines

    IEnumerator Transition(Vector3 startpos, Vector3 endpos, float duration)
    {
        for(float time = Time.fixedDeltaTime; time < duration; time += Time.fixedDeltaTime)
        {
            transform.localPosition = Vector3.Lerp(startpos, endpos, time / duration) + Vector3.forward * 10f;
            yield return new WaitForFixedUpdate();
        }
        transform.localPosition = endpos + Vector3.forward * 10f;
    }

}
