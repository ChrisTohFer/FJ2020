using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    //Static data
    public static Transform playerTransform;

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
    public float swingRadius = 3f;
    public float swingFrequency = 0.6f;
    public float swingMaxAngle = Mathf.PI / 4f;
    public float swingSpeedBoost = 15f;
    public float swingTolerance = Mathf.PI / 4f;
    public bool debugFlyMode = false;

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
    bool m_swinging = false;
    bool m_preparedToFling = false;
    bool m_preparedToSwing = false;
    bool m_swingCoroutineActive = false;

    int m_collisionStayFrames = 0;

    private void Awake()
    {
        playerTransform = transform;
    }
    private void Start()
    {
        m_band.maxStretch = bandMaxStretch;
    }

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
        if (!m_grounded)
            return;

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
        if (m_flinging || m_flying || m_swinging)
            return;

        Vector2 rayStart = transform.position + Vector3.down * (m_collider.bounds.extents.y + 0.1f);
        Debug.DrawRay(rayStart, Vector3.down * 0.1f, Color.red);

        RaycastHit2D result;
        result = Physics2D.Raycast(rayStart, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
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
        if(m_band.Pinned && m_preparedToFling && m_band.StretchVector.magnitude > bandMaxStretch)
        {
            SetFlinging();
        }
        if (m_band.Pinned && m_preparedToSwing)
        {
            SetSwinging();
        }
    }

    void SetGrounded()
    {
        m_grounded = true;
        m_flinging = false;
        m_flying = false;
        m_swinging = false;
        m_rigidBody.simulated = true;
        m_rigidBody.mass = 1f;
        m_rigidBody.gravityScale = defaultGravity;
        StopAllCoroutines();
    }
    void SetAirborne()
    {
        m_grounded = false;
        m_flinging = false;
        m_flying = false;
        m_swinging = false;
        m_preparedToSwing = false;
        m_swingCoroutineActive = false;
        m_rigidBody.simulated = true;
        m_rigidBody.mass = 1f;
        m_rigidBody.gravityScale = defaultGravity;
        m_joint.distance = defaultBandLength;
        StopAllCoroutines();
    }
    void SetFlinging()
    {
        m_grounded = false;
        m_flinging = true;
        m_flying = false;
        m_swinging = false;
        m_preparedToFling = false;
        m_rigidBody.simulated = true;
        m_rigidBody.mass = 1f;
        m_rigidBody.gravityScale = 0f;
        m_joint.distance = defaultBandLength;
        StopAllCoroutines();
        StartCoroutine(FlingToPin());
    }
    void SetFlying(Vector2 velocity)
    {
        m_grounded = false;
        m_flinging = false;
        m_flying = true;
        m_swinging = false;
        m_rigidBody.simulated = true;
        m_rigidBody.mass = 1f;
        m_rigidBody.gravityScale = flyingGravity;
        m_rigidBody.velocity = velocity;
        m_joint.distance = defaultBandLength;
        StopAllCoroutines();
        StartCoroutine(Fly());
        m_band.Unpin();
    }
    void SetSwinging()
    {
        m_grounded = false;
        m_flinging = false;
        m_flying = false;
        m_swinging = true;
        m_preparedToSwing = false;
        m_rigidBody.simulated = true;
        m_rigidBody.mass = 0.5f;
        m_rigidBody.gravityScale = defaultGravity;
        m_joint.distance = defaultBandLength;
        StopAllCoroutines();
        StartCoroutine(Swing());
    }

    //Coroutines

    IEnumerator CoyoteWait(float duration)
    {
        for (float time = 0; time < duration; time += Time.fixedDeltaTime)
        {
            yield return new WaitForFixedUpdate();
        }
        SetAirborne();
    }

    IEnumerator FlingToPin()
    {
        var direction = m_band.StretchVector.normalized;
        var velocity = direction * flingSpeed;
        m_rigidBody.velocity = velocity;

        while (direction.x * m_band.StretchVector.x > 0 && direction.y * m_band.StretchVector.y > 0 && m_flinging) //if either x or y flip sign we have finished flinging
        {
            direction = m_band.StretchVector.normalized;
            velocity = direction.normalized * flingSpeed;
            m_rigidBody.velocity = velocity;
            yield return new WaitForFixedUpdate();
        }

        SetFlying(velocity);
    }

    IEnumerator Fly()
    {
        for (float time = 0f; time < flyingDuration; time += Time.fixedDeltaTime)
            yield return new WaitForFixedUpdate();

        SetAirborne();
    }

    IEnumerator Swing()
    {
        var pivot = m_band.pin.position;
        var vectorFromPivot = transform.position - pivot;
        var angle = Mathf.Asin(vectorFromPivot.x / vectorFromPivot.magnitude);
        if (Mathf.Abs(angle) > swingMaxAngle)
            angle = swingMaxAngle * Mathf.Sign(angle);

        var phase = Mathf.Asin(angle / swingMaxAngle);
        if (angle > 0 && angle < Mathf.PI)
            phase = Mathf.PI - phase;

        m_swingCoroutineActive = true;

        Vector3 targetPosition;
        do
        {
            phase += Mathf.PI * 2f * swingFrequency * Time.deltaTime;
            if (phase > Mathf.PI * 2f)
                phase -= Mathf.PI * 2f;
            var targetAngle = Mathf.Sin(phase) * swingMaxAngle;
            targetPosition = pivot + new Vector3(Mathf.Sin(targetAngle), -Mathf.Cos(targetAngle), 0f) * swingRadius;
            var displacement = targetPosition - transform.position;
            m_rigidBody.velocity = displacement * 5;
            yield return new WaitForFixedUpdate();
        } while (m_swinging);

        if (Vector3.Distance(targetPosition, transform.position) < 1.5f)
        {
            if (phase <= Mathf.PI / 2f && phase > Mathf.PI / 2f - swingTolerance)
            {
                m_rigidBody.velocity = new Vector3(Mathf.Cos(swingMaxAngle), Mathf.Sin(swingMaxAngle), 0f) * swingSpeedBoost;
            }
            else if (phase <= 3f * Mathf.PI / 2f && phase > 3f * Mathf.PI / 2f - swingTolerance)
            {
                m_rigidBody.velocity = new Vector3(-Mathf.Cos(swingMaxAngle), Mathf.Sin(swingMaxAngle), 0f) * swingSpeedBoost;
            }
        }

        m_band.Unpin();
        SetAirborne();
    }

    GrapplePoint DetectGrapplePointAtMouse(Vector3 mousePosition)
    {
        var hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0, ~LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            return hit.collider.GetComponent<GrapplePoint>();
        }
        
        return null;
    }

    //Callbacks

    private void OnMove(InputValue move)
    {
        var vec = (Vector2)move.Get();
        m_movementInput = vec;
    }

    private void OnClick(InputValue button)
    {
        if (m_flinging || m_swinging || m_preparedToSwing)
            return;

        if (button.isPressed)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0;
            var grapplePoint = DetectGrapplePointAtMouse(mousePosition);
            if(debugFlyMode)
            {
                m_band.PinToLocation(mousePosition);
                m_preparedToFling = true;
                m_joint.distance = extendedBandLength;
            }
            else if(grapplePoint != null && grapplePoint.canFling && grapplePoint.InRange(transform))
            {
                m_band.PinToLocation(grapplePoint.transform.position);
                m_preparedToFling = true;
                m_joint.distance = extendedBandLength;
            }
            else if((mousePosition - transform.position).magnitude > extendedBandLength)
            {
                m_band.SlingAtLocation(transform.position + (mousePosition - transform.position).normalized * extendedBandLength);
            }
            else
            {
                m_band.SlingAtLocation(mousePosition);
            }
        }
        else if (m_band.Pinned && m_preparedToFling)
            SetFlinging();
        else if (m_band.Slinging && m_preparedToFling)
        {
            m_band.PinToLocationInstant(m_band.slingDestination);
            SetFlinging();
        }
    }

    private void OnRightClick(InputValue button)
    {
        if (m_flinging || m_preparedToFling || m_preparedToFling)
            return;

        if (button.isPressed)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0;
            var grapplePoint = DetectGrapplePointAtMouse(mousePosition);
            if (debugFlyMode)
            {
                m_band.PinToLocation(mousePosition);
                m_preparedToSwing = true;
            }
            if (grapplePoint != null && grapplePoint.canSwing && grapplePoint.InRange(transform))
            {
                m_band.PinToLocation(grapplePoint.transform.position);
                m_preparedToSwing = true;
            }
            else if ((mousePosition - transform.position).magnitude > extendedBandLength)
            {
                m_band.SlingAtLocation(transform.position + (mousePosition - transform.position).normalized * extendedBandLength);
            }
            else
            {
                m_band.SlingAtLocation(mousePosition);
            }
        }
        else if (m_swinging && m_swingCoroutineActive)
        {
            m_swinging = false;
        }
        else
        {
            SetAirborne();
            m_band.Unpin();
            m_preparedToSwing = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Ground")
        {
            m_flinging = false;
            m_swinging = false;
        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.collider.tag == "Ground")
        {
            if(m_collisionStayFrames > 3)
            {
                m_flinging = false;
                m_swinging = false;
            }
            if (m_flinging || m_swinging)
                ++m_collisionStayFrames;
            else
                m_collisionStayFrames = 0;
        }
        else
            m_collisionStayFrames = 0;
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.collider.tag == "Ground")
        {
            m_collisionStayFrames = 0;
        }
    }
}
