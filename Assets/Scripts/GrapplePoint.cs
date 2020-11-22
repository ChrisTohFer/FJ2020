using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    //Public data
    public bool canFling = true;
    public bool canSwing = true;
    public float radius = 6f;
    public float inRangeScaleMin = 1.3f;
    public float inRangeScaleMax = 1.5f;
    public float inRangeFrequency = 1f;
    public Transform transformToScale = null;
    float growthPhase = 0f;

    private void Start()
    {
        if (transformToScale == null)
            transformToScale = transform;
    }

    private void FixedUpdate()
    {
        var playerDistance = Vector3.Distance(Movement.playerTransform.position, transform.position);
        if(playerDistance < radius)
        {
            growthPhase += Time.fixedDeltaTime * Mathf.PI * 2f * inRangeFrequency;
            var scaleFactor = inRangeScaleMin + (inRangeScaleMax - inRangeScaleMin) * Mathf.Cos(growthPhase);
            transformToScale.localScale = Vector3.one * scaleFactor;
        }
        else
        {
            growthPhase = 0f;
            transformToScale.localScale = Vector3.one;
        }
    }

    public bool InRange(Transform playerTransform)
    {
        return (transform.position - playerTransform.position).magnitude <= radius;
    }
}
