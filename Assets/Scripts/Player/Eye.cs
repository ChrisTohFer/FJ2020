using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Eye : MonoBehaviour
{
    private void FixedUpdate()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;

        var eyePosition = transform.position;
        var difference = (Vector2)(mousePosition - eyePosition);
        transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.left, -difference.normalized));
    }
}
