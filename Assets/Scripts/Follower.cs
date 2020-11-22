using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    //Static
    int previousFollowerId = 0;
    static Follower lastFollower = null;

    //Public
    public SpriteRenderer mainRenderer;
    public SpriteRenderer eyeRenderer;
    public Eye eye;
    public float followDistance = 1f;
    public float followBaseSpeed = 1f;
    public float followSpeedPerDistance = 2f;
    public Collider2D followerCollider;

    Transform followTarget = null;
    bool collected = false;

    private void Start()
    {
        lastFollower = null;
        ++previousFollowerId;
        mainRenderer.sortingOrder = mainRenderer.sortingOrder + 2 * previousFollowerId;
        eyeRenderer.sortingOrder = mainRenderer.sortingOrder + 2 * previousFollowerId;
    }

    private void FixedUpdate()
    {
        if (followTarget == null)
            return;

        var difference = followTarget.position - transform.position;
        var distance = difference.magnitude;
        if(distance > followDistance)
        {
            var gap = distance - followDistance;
            var speedTick = (followBaseSpeed + followSpeedPerDistance * gap) * Time.fixedDeltaTime;
            transform.position = transform.position + speedTick * difference.normalized;
        }
    }

    public void Collect()
    {
        collected = true;
        eye.target = Eye.Target.Mouse;
        followerCollider.enabled = false;
        if(lastFollower == null)
        {
            followTarget = Movement.playerTransform;
        }
        else
        {
            followTarget = lastFollower.transform;
        }
        lastFollower = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collected && collision.gameObject.tag == "Player")
        {
            Collect();
        }
    }
}
