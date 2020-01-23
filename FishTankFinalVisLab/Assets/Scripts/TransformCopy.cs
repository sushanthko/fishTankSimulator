using System;
using UnityEngine;

/// <summary>
/// Copies the transform of a one gameobject to another.
/// 
/// @author Christoffer A Træen
/// </summary>
public class TransformCopy : MonoBehaviour
{

    /// <summary>
    /// Target to copy transform from
    /// </summary>
    public Transform copyFrom;
    /// <summary>
    /// Target to copy transform to
    /// </summary>
    public Transform copyTo;

    /// <summary>
    /// Flag to enable copy of rotation
    /// </summary>
    public Boolean rotation;
    public Vector3 offset = new Vector3(0f, 7.5f, 0f);

    private void Update()
    {
        copyTo.position = copyFrom.position + offset;
        if (rotation)
        {
            copyTo.rotation = copyFrom.rotation;
        }
    }
}