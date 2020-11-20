using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    //Public properties
    public float coyoteTime = 0.2f;

    //Public references
    public Rigidbody2D m_rigidBody;
    public Collider2D m_collider;
    public RubberBand m_band;

    //Private data
    Vector2 m_movementInput;
    bool m_grounded = false;

    private void FixedUpdate()
    {
        ApplyPlayerMovement();
    }

    //Functions

    void ApplyPlayerMovement()
    {
        CheckGrounded();
        if(m_grounded)
            m_rigidBody.velocity = new Vector2(m_movementInput.x * 4, m_rigidBody.velocity.y);
    }

    void CheckGrounded()
    {
        Vector2 rayStart = transform.position + Vector3.down * (m_collider.bounds.extents.y + 0.1f);
        //Debug.DrawRay(rayStart, Vector3.down * 0.1f, Color.red);

        RaycastHit2D result;
        result = Physics2D.Raycast(rayStart, Vector2.down, 0.1f);
        if (result.transform != null)
        {
            if (result.transform.tag == "Ground")
            {
                m_grounded = true;
                StopCoroutine("CoyoteWait");
            }
        }
        else if (m_grounded)
            StartCoroutine(CoyoteWait(coyoteTime));
    }

    //Coroutines

    IEnumerator CoyoteWait(float duration)
    {
        for (float time = 0; time < duration; time += Time.fixedDeltaTime)
        {
            Debug.Log(time + ", " + duration);
            yield return new WaitForFixedUpdate();
        }
        m_grounded = false;
    }

    //Callbacks

    private void OnMove(InputValue move)
    {
        var vec = (Vector2)move.Get();
        m_movementInput = vec;
    }

    private void OnClick(InputValue button)
    {
        if (button.isPressed)
        {
            var position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            position.z = 0;
            m_band.PinToLocation(position);
        }
        else
            m_band.Unpin();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.otherCollider.tag == "Ground")
        {
            m_grounded = true;
            StopCoroutine("CoyoteWait");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.otherCollider.tag == "Ground")
            StartCoroutine(CoyoteWait(coyoteTime));
    }


}
