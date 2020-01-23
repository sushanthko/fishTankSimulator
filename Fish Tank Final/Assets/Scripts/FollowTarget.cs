using System;
using UnityEngine;

/// <summary>
/// Follows a target with optional offset(Position only).
/// Can follow both position and rotation.
/// </summary>
public class FollowTarget : MonoBehaviour
{
    /// <summary>
    /// Target to follow
    /// </summary>
    [SerializeField]
    private Transform target;
    /// <summary>
    /// Flag to enable follow rotation
    /// </summary>
    [SerializeField]
    private Boolean rotation;
    /// <summary>
    /// Position offset
    /// </summary>
    /// <returns></returns>
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 7.5f, 0f);

    private void LateUpdate()
    {
        transform.position = target.position + offset;
        if (rotation)
        {
            transform.rotation = target.rotation;
        }
    }
}