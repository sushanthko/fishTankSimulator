using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotate the game ocject around a target object.
/// 
/// @author Christoffer A Træen
/// </summary>
public class RotateAround : MonoBehaviour
{
    /// <summary>
    /// The object to rotate around
    /// </summary>
    public GameObject rotateAroundTarget;
    /// <summary>
    /// Speed of the rotation
    /// </summary>
    public float speed = 10;

    void Update()
    {
        this.transform.RotateAround(rotateAroundTarget.transform.position, Vector3.up, this.speed * Time.deltaTime);
    }
}