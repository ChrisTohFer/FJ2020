using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    //Public properties
    public float horizontalMoveSpeed = 4f;
    public float horizontalAccel = 2f;
    public float coyoteTime = 0.2f;
    public float bandMaxStretch = 5f;   //The distance at which the band will auto fling the player
    public float flyingDuration = .5f; //The time after flinging during which physics is altered
    public float defaultGravity = 1f;
    public float flyingGravity = 1f;
    public float flingSpeed = 15f;
    public float maxSpeed = 15f;
    public float drag = 10f;
    public float defaultBandLength = 1f;
    public float extendedBandLength = 4f;

    //Public references
    public Rigidbody2D m_rigidBody;
    public Collider2D m_collider;
    public RubberBand m_band;
    public DistanceJoint2D m_joint;

    //Private data
    Vector2 m_movementInput;
    bool m_grounded = false;
    bool m_flinging = false;
    bool m_flying = false;

    private void FixedUpdate()
    {
        CheckGrounded();
        ApplyPlayerMovement();
        CheckBandState();
        ApplyDrag();
    }

    //Functions

    void ApplyPlayerMovement()
    {
        if(m_grounded)
        {
            var vel = m_rigidBody.velocity;
            var accelTick = m_movementInput.x * horizontalAccel * Time.deltaTime;
            if (m_rigidBody.velocity.x * Mathf.Sign(accelTick) < horizontalMoveSpeed * 0.25f)
            {
                m_rigidBody.velocity = new Vector2(vel.x + accelTick * 5f, vel.y);
            }
            else if (m_rigidBody.velocity.x * Mathf.Sign(accelTick) < horizontalMoveSpeed)
            {
                m_rigidBody.velocity = new Vector2(vel.x + accelTick, vel.y);
            }

        }
    }

    void ApplyDrag()
    {
        if (m_flinging || m_rigidBody.velocity.magnitude < maxSpeed)
            return;

        var excessVelocity = m_rigidBody.velocity.normalized * (m_rigidBody.velocity.magnitude - maxSpeed);
        var dragPerFrame = drag * Time.fixedDeltaTime;

        if (excessVelocity.magnitude < dragPerFrame)
            m_rigidBody.velocity -= excessVelocity;
        else
            m_rigidBody.velocity -= excessVelocity.normalized * dragPerFrame;
    }

    void CheckGrounded()
    {
        if (m_flinging || m_flying)
            return;

        Vector2 rayStart = transform.position + Vector3.down * (m_collider.bounds.extents.y + 0.1f);
        Debug.DrawRay(rayStart, Vector3.down * 0.1f, Color.red);

        RaycastHit2D result;
        result = Physics2D.Raycast(rayStart, Vector2.down, 0.1f);
        if (result.transform != null)
        {
            if (result.transform.tag == "Ground")
            {
                SetGrounded();
                StopCoroutine("CoyoteWait");
            }
        }
        else if (m_grounded)
            StartCoroutine(CoyoteWait(coyoteTime));
    }

    void CheckBandState()
    {
        if(m_band.Pinned && m_band.StretchVector.magnitude > bandMaxStretch)
        {
            SetFlinging();
        }
    }

    void SetGrounded()
    {
        m_grounded = true;
        m_flinging = false;
        m_flying = false;
        m_rigidBody.simulated = true;
        m_rigidBody.gravityScale = defaultGravity;
        StopAllCoroutines();
    }
    void SetAirborne()
    {
        m_grounded = false;
        m_flinging = false;
        m_flying = false;
        m_rigidBody.simulated = true;
        m_rigidBody.gravityScale = defaultGravity;
        StopAllCoroutines();
    }
    void SetFlinging()
    {
        m_grounded = false;
        m_flinging = true;
        m_flying = false;
        m_rigidBody.simulated = true;
        m_rigidBody.gravityScale = 0f;
        StopAllCoroutines();
        m_joint.distance = defaultBandLength;
        StartCoroutine(FlingToPin());
    }
    void SetFlying(Vector2 velocity)
    {
        m_grounded = false;
        m_flinging = false;
        m_flying = true;
        m_rigidBody.simulated = true;
        m_rigidBody.gravityScale = flyingGravity;
        m_rigidBody.velocity = velocity;
        StopAllCoroutines();
        StartCoroutine(Fly());
        m_band.Unpin();
    }

    //Coroutines

    IEnumerator CoyoteWait(float duration)
    {
        for (float time = 0; time < duration; time += Time.fixedDeltaTime)
        {
            Debug.Log(time + ", " + duration);
            yield return new WaitForFixedUpdate();
        }
        SetAirborne();
    }

    IEnumerator FlingToPin()
    {
        var direction = m_band.StretchVector.normalized;
        var velocity = direction * flingSpeed;
        m_rigidBody.velocity = velocity;

        while (direction.x * m_band.StretchVector.x > 0 && direction.y * m_band.StretchVector.y > 0) //if either x or y flip sign we have finished flinging
            yield return new WaitForFixedUpdate();

        SetFlying(velocity);
    }

    IEnumerator Fly()
    {
        for (float time = 0f; time < flyingDuration; time += Time.fixedDeltaTime)
            yield return new WaitForFixedUpdate();

        SetAirborne();
    }

    //Callbacks

    private void OnMove(InputValue move)
    {
        var vec = (Vector2)move.Get();
        m_movementInput = vec;
    }

    private void OnClick(InputValue button)
    {
        if (m_flinging)
            return;

        if (button.isPressed)
        {
            var position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            position.z = 0;
            m_band.PinToLocation(position);
            m_joint.distance = extendedBandLength;
        }
        else if (m_band.Pinned)
            SetFlinging();
        else if (m_band.Slinging)
        {
            m_band.PinToLocationInstant(m_band.slingDestination);
            SetFlinging();
        }
    }

    private void OnRightClick(InputValue button)
    {
        if (m_flinging)
            return;

    }
}
