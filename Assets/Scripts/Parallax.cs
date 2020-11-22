using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //Public
    public float parallax = .5f;

    Vector3 initialPosition;
    Vector3 initialCameraPosition;

    private void Start()
    {
        initialPosition = transform.position;
        initialCameraPosition = Camera.main.transform.position;
    }

    private void FixedUpdate()
    {
        transform.position = initialPosition + parallax * (Camera.main.transform.position - initialCameraPosition);
    }

}
