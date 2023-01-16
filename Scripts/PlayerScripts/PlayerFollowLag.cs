using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowLag : MonoBehaviour
{
    public Transform PlayerTransform;

    [Range(0f, 40f)]
    public float FollowSpeed;

    [Range(0f, 40f)]
    public float VerticalFollowSpeed;

    private void Update()
    {
        Vector3 lerpVector = Vector3.Lerp(this.transform.position, PlayerTransform.position, Time.deltaTime * FollowSpeed);
        lerpVector.y = Mathf.Lerp(this.transform.position.y, PlayerTransform.position.y, Time.deltaTime * VerticalFollowSpeed);
        transform.position = lerpVector;
    }
}
