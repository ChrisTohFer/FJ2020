using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //Public properties
    public float minSpeed = 1f;
    public float speedIncreasePerDistance = 1f;
    public Transform playerTransform;
    //public List<>

    private void FixedUpdate()
    {
        var player2DPosition = (Vector2)playerTransform.position;
        var camera2DPosition = (Vector2)Camera.main.transform.position;
        var difference = player2DPosition - camera2DPosition;
        var speedTick = (minSpeed + speedIncreasePerDistance * difference.magnitude) * Time.fixedDeltaTime;

        if (speedTick > difference.magnitude)
            Camera.main.transform.position = playerTransform.position + Vector3.back * 10f;
        else
        {
            var velocityTick = (Vector3)difference.normalized * speedTick;
            Camera.main.transform.position += velocityTick;
        }
    }

}
