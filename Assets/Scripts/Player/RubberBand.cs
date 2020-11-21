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

    private void Update()
    {
        var difference = pin.position - transform.position;
        var angle = -Vector2.SignedAngle(difference, Vector2.right);

        transform.eulerAngles = new Vector3(0f, 0f, angle);
        transform.localScale = new Vector3(difference.magnitude, 1f / Mathf.Pow(difference.magnitude, thinningPower));
    }

    public void PinToLocation(Vector3 pos)
    {
        rigidBody.simulated = false;
        StartCoroutine(BandSling(slingTime, pos));
    }
    public void Unpin()
    {
        rigidBody.simulated = true;
        StopCoroutine("BandSling");
    }

    //Coroutines

    IEnumerator BandSling(float duration, Vector3 destination)
    {
        if(duration == 0)
        {
            Debug.Log("Duration0");
            pin.position = destination;
            yield break;
        }

        var originalPosition = pin.transform.position;
        var difference = destination - originalPosition;
        Debug.Log("Start:" + originalPosition);
        for (float time = Time.fixedDeltaTime; time < duration; time += Time.fixedDeltaTime)
        {
            pin.position = originalPosition + (difference * time / duration);
            Debug.Log("Intermediate point:" + pin.position);

            yield return new WaitForFixedUpdate();
        }
        pin.position = destination;
    }
    
}
