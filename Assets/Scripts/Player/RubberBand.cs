﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubberBand : MonoBehaviour
{
    //Public Properties
    public float naturalLength = 1f;
    public float thinningPower = 1f;
    public float slingTime = 0.2f;
    public Transform pin;
    public Rigidbody2D rigidBody;
    public Vector3 slingDestination
    {
        get { return m_slingDestination; }
        set { m_slingDestination = value; }
    }

    //
    Vector3 m_slingDestination;
    bool m_slinging = false;

    public bool Pinned
    {
        get { return !rigidBody.simulated && !m_slinging; }
    }

    public bool Slinging
    {
        get { return m_slinging; }
    }

    public Vector2 StretchVector
    {
        get { return pin.position - transform.position; }
    }

    private void Update()
    {
        var difference = pin.position - transform.position;
        var angle = -Vector2.SignedAngle(difference, Vector2.right);

        transform.eulerAngles = new Vector3(0f, 0f, angle);
        transform.localScale = new Vector3(difference.magnitude, 1f / Mathf.Pow(Mathf.Max(difference.magnitude, 1f), thinningPower));
    }

    public void PinToLocation(Vector3 pos)
    {
        rigidBody.simulated = false;
        StartCoroutine(BandSling(slingTime, pos));
    }
    public void PinToLocationInstant(Vector3 pos)
    {
        rigidBody.simulated = false;
        pin.position = pos;
        StopCoroutine("BandSling");
        m_slinging = false;
    }
    public void Unpin()
    {
        rigidBody.simulated = true;
        StopCoroutine("BandSling");
        m_slinging = false;
    }

    //Coroutines

    IEnumerator BandSling(float duration, Vector3 destination)
    {
        if(duration == 0)
        {
            pin.position = destination;
            yield break;
        }
        slingDestination = destination;
        m_slinging = true;

       var originalPosition = pin.transform.position;
        var difference = destination - originalPosition;
        for (float time = Time.fixedDeltaTime; time < duration; time += Time.fixedDeltaTime)
        {
            pin.position = originalPosition + (difference * time / duration);

            yield return new WaitForFixedUpdate();
        }
        pin.position = destination;

        m_slinging = false;
    }
    
}
