using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    //Public properties


    //Private data
    Rigidbody2D m_rigidBody;
    Vector2 m_movementInput;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        m_rigidBody.velocity = new Vector2(m_movementInput.x * 4, m_rigidBody.velocity.y);
    }

    private void OnMove(InputValue move)
    {
        Debug.Log("Test");
        var vec = (Vector2)move.Get();
        m_movementInput = vec;
    }
}
