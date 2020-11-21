using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //Public properties
    public float minSpeed = 1f;
    public float speedIncreasePerDistance = 1f;
    public Transform playerTransform;
    List<CameraPointOfInterest> pointsOfInterest = new List<CameraPointOfInterest>();

    private void FixedUpdate()
    {
        var targetPos = CalculateCameraTarget();
        var cameraPos= (Vector3)(Vector2)Camera.main.transform.position;
        var difference = targetPos - cameraPos;
        var speedTick = (minSpeed + speedIncreasePerDistance * difference.magnitude) * Time.fixedDeltaTime;

        if (speedTick > difference.magnitude)
            Camera.main.transform.position = targetPos;
        else
        {
            var velocityTick = (Vector3)difference.normalized * speedTick;
            Camera.main.transform.position += velocityTick;
        }

        //Make sure camera doesn't move in z
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10f);
    }

    public Vector3 CalculateCameraTarget()
    {
        var target = playerTransform.position;
        var totalWeight = 1f;

        foreach(CameraPointOfInterest poi in pointsOfInterest)
        {
            if(poi.PlayerWithinCatchmentArea())
            {
                target += poi.PointOfInterest() * poi.weighting;
                totalWeight += poi.weighting;
            }
        }

        target /= totalWeight;
        return target;
    }

    public void AddPointOfInterest(CameraPointOfInterest poi)
    {
        pointsOfInterest.Add(poi);
    }

}
