using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPointOfInterest : MonoBehaviour
{
    public enum Type
    {
        Point,
        Line,
        Square
    }

    //Public
    public Type type = Type.Point;
    public float weighting = 1f;
    public float radius = 10f;
    public Transform secondPin;

    Transform playerTransform;

    private void Start()
    {
        var moveScript = Camera.main.GetComponent<CameraMovement>();
        moveScript.AddPointOfInterest(this);
        playerTransform = moveScript.playerTransform;
    }

    public bool PlayerWithinCatchmentArea()
    {
        var playerPos = playerTransform.position;
        switch(type)
        {
            case Type.Point:
                return (playerPos - transform.position).magnitude < radius;
            case Type.Line:
                var line = secondPin.position - transform.position;
                if ((Vector3.Dot(line, playerPos - transform.position) < 0))       //Camera is before point 1
                    return false;
                if ((Vector3.Dot(line, secondPin.position - playerPos) < 0))       //Camera is past point 2
                    return false;
                return Vector3.Cross(line.normalized, playerPos - transform.position).magnitude < radius;  //Camera within line radius
            case Type.Square:
                var check1 = playerPos.x > transform.position.x && playerPos.x < secondPin.position.x;
                var check2 = playerPos.y < transform.position.y && playerPos.y > secondPin.position.y;
                return check1 && check2;
            default:
                return false;
        }
    }

    public Vector3 PointOfInterest()
    {
        var playerPos = playerTransform.position;
        switch (type)
        {
            case Type.Point:
                return transform.position;
            case Type.Line:
                var line = secondPin.position - transform.position;
                var perpendicularLine = playerPos - Vector3.Cross(line.normalized, playerPos - transform.position);
                Debug.DrawRay(playerPos, Vector3.Cross(perpendicularLine, line.normalized));
                return playerPos + Vector3.Cross(perpendicularLine, line.normalized);
            case Type.Square:
                return (transform.position + secondPin.position) / 2f;
            default:
                return transform.position;
        }

    }
}
