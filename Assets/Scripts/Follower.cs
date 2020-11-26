using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Follower : MonoBehaviour
{
    //Static
    static int previousFollowerId = 0;
    static Follower lastFollower = null;
    static int followerCount = 0;

    //Public
    public SpriteRenderer mainRenderer;
    public SpriteRenderer eyeRenderer;
    public Eye eye;
    public float followDistance = 1f;
    public float followBaseSpeed = 1f;
    public float followSpeedPerDistance = 2f;
    public Collider2D followerCollider;
    public int followerValue = 1;

    Transform followTarget = null;
    bool collected = false;
    static UnityEvent m_followerGained = new UnityEvent();

    public static UnityEvent FollowerGained
    {
        get { return m_followerGained; }
    }

    private void LevelStart(int level)
    {
    }
    private void OnEnable()
    {
        followerCount += 1;
    }
    private void OnDisable()
    {
        followerCount -= 1;
        if (followerCount == 0)
            lastFollower = null;
    }

    private void Start()
    {
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
        m_followerGained.Invoke();

        collected = true;
        var gameManager = GameManager.gminstance;
        if(gameManager != null)
        {
            gameManager.ChangeScore(followerValue);
        }
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

        AudioManager.PlaySoundEffect("FollowerCheer");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collected && collision.gameObject.tag == "Player")
        {
            Collect();
        }
    }
}
