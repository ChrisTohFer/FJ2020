using System.Collections;
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
    public SpriteRenderer sprite;
    public float maxStretch
    {
        set { m_maxStretch = value; }
    }
    public Vector3 slingDestination
    {
        get { return m_slingDestination; }
        set { m_slingDestination = value; }
    }

    //
    float m_maxStretch;
    Vector3 m_slingDestination;
    bool m_slinging = false;
    Color bandColor;

    public bool Pinned
    {
        get { return rigidBody.constraints == RigidbodyConstraints2D.FreezeAll && !m_slinging; }
    }

    public bool Occupied
    {
        get { return Slinging || Pinned; }
    }

    public bool Slinging
    {
        get { return m_slinging; }
    }

    public Vector2 StretchVector
    {
        get { return pin.position - transform.position; }
    }

    private void Start()
    {
        bandColor = sprite.color;
    }

    private void Update()
    {
        var difference = pin.position - transform.position;
        var angle = -Vector2.SignedAngle(difference, Vector2.right);

        transform.eulerAngles = new Vector3(0f, 0f, angle);
        transform.localScale = new Vector3(difference.magnitude, 1f / Mathf.Pow(Mathf.Max(difference.magnitude, 1f), thinningPower));
        sprite.color = Color.Lerp(bandColor, Color.white, Mathf.Pow((difference.magnitude - naturalLength) / (m_maxStretch - naturalLength),3f));
    }

    public void PinToLocation(Vector3 pos)
    {
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        StopCoroutine("BandSling");
        StartCoroutine(BandSling(slingTime, pos));
    }
    public void SlingAtLocation(Vector3 pos)
    {
        StopCoroutine("BandSling");
        StartCoroutine(BandSling(slingTime, pos));
    }
    public void PinToLocationInstant(Vector3 pos)
    {
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        pin.position = pos;
        StopCoroutine("BandSling");
        m_slinging = false;
    }
    public void Unpin()
    {
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
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
