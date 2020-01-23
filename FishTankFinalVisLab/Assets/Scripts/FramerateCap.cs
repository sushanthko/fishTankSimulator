using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Limits the application frame rate cap with slider.
/// 
/// @author Christoffer A Træen
/// </summary>
public class FramerateCap : MonoBehaviour
{
    [SerializeField]
    [Range(20, 200)]
    private int targetFramerate = 60;
    void Awake()
    {
        Application.targetFrameRate = this.targetFramerate;
    }

}