using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubberBand : MonoBehaviour
{
    //Public Properties
    public float naturalLength = 1f;
    public float thinningPower = 1f;
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
        pin.position = pos;
        rigidBody.simulated = false;
    }
    public void Unpin()
    {
        rigidBody.simulated = true;
    }

}
