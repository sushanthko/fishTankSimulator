using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Activates cameras on spesific machines.
/// Usefull if you want to dislay a standard camera
/// instead of a UniCave camera.
/// 
/// @author Christoffer A Træen
/// </summary>
public class CameraActivator : MonoBehaviour
{
    /// <summary>
    /// Name of the machine that the camera should activate on
    /// </summary>
    [SerializeField]
    private string machineName = "";

    /// <summary>
    /// The camera that should be activated
    /// </summary>
    [SerializeField]
    private Camera cameraToActivate;

    void Start()
    {
        if (Util.GetMachineName().Equals(this.machineName))
        {
            this.cameraToActivate.gameObject.SetActive(true);
        }
    }
}