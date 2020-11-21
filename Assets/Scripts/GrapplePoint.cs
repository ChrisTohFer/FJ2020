using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    //Public data
    public bool canFling = true;
    public bool canSwing = true;
    public float radius = 6f;

    public bool InRange(Transform playerTransform)
    {
        return (transform.position - playerTransform.position).magnitude <= radius;
    }
}
