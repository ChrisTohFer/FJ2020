using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Eye : MonoBehaviour
{
    public enum Target
    {
        Mouse,
        Player
    }

    public Target target = Target.Mouse;

    private void FixedUpdate()
    {
        Vector3 targetPosition = Vector3.zero;
        if(target == Target.Mouse)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0;
            targetPosition = mousePosition;
        }
        else
        {
            targetPosition = Movement.playerTransform.position;
        }

        var eyePosition = transform.position;
        var difference = (Vector2)(targetPosition - eyePosition);
        transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.left, -difference.normalized));
    }
}
