using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for saving the calibration config data
/// for objects that has script <see cref="PhysicalDisplayCalibration" /> attached.
/// 
/// The class can handle idividual displays with <c>PhysicalDisplayCalibration</c> attached to them
///  or a set of displays managed by a <see cref="PhysicalDisplayManager" />.
/// 
/// Author: Christoffer A Træen
/// </summary>
public class PhysicalDisplayCalibrationSaver : MonoBehaviour
{
    /// <summary>
    /// Flag to tell if we are using DisplayManager for 
    /// our displays or not
    /// </summary>
    public bool isUsingDisplayManager = false;

    /// <summary>
    /// Holds the display manager which manages
    /// a set of displays. Where the controlled displays has
    /// <see cref="PhysicalDisplayCalibration" /> scripts attached.
    /// </summary>
    [SerializeField]
    [Tooltip("The display manager parrent")]
    [ConditionalHide("isUsingDisplayManager", false, true)]
    public PhysicalDisplayManager physicalDisplayManager;

    /// <summary>
    /// Holds all instances of displays that 
    /// </summary>
    /// <returns></returns>
    [SerializeField]
    [Tooltip("The display manager parrent")]
    [ConditionalHide("isUsingDisplayManager", true, true)]
    public PhysicalDisplayCalibrationHolder calibratedDisplays = new PhysicalDisplayCalibrationHolder();

    /// <summary>
    /// Holds the name of the machine this instance is running on
    /// </summary>
    private string machineName;

    [SerializeField]
    [Tooltip("Should we only save the config file for the displays on the coresponding machine, or for all the displays on the netowrk. This will save all config files on all machines.")]
    private bool saveThisMachineOnly = true;


    /// <summary>
    /// Check if we are using display manager, if we are
    /// check if a reference is added, if not log an error.
    /// </summary>
    void Start()
    {
        this.machineName = SystemInfo.deviceName;

        if (isUsingDisplayManager)
        {
            if (this.physicalDisplayManager == null)
            {
                Debug.LogError($"Please provide a Physical display manager: field isUsingDisplayManager is {isUsingDisplayManager}");
            }
        }

    }

    /// <summary>
    /// Saves the calibrations for provided PhysicalDisplayManager
    /// or from the list of calibrated displays in <c>calibratedDisplays</c>.
    /// If is using display manager, calls <see cref="SaveManagerDisplays" /> else
    /// calls <see cref="SaveListOfCalibratedDisplays" />
    /// </summary>
    public void SaveCalibrations()
    {
        if (this.isUsingDisplayManager)
        {
            this.SaveManagerDisplays();
        }
        else
        {
            this.SaveListOfCalibratedDisplays();
        }
    }

    /// <summary>
    /// Loops over all displays on the manager, gets the <c>PhysicalDisplayCalibration</c> reference and calls <see cref="PhysicalDisplayCalibration.SaveWarpFile">
    /// on them to save the WarpFile to disk.
    /// If <c>saveThisMachineOnly</c> is true, we only save the config files for the displays that has the same machine name value as
    /// the machine running the instance of Unity. Else we save the config file for each display. If we have 3 machines, with 3 displays on each
    /// we will save all 9 config files on each machine.
    /// </summary>
    private void SaveManagerDisplays()
    {
        this.physicalDisplayManager.displays.ForEach(display =>
        {
            if (this.saveThisMachineOnly)
            {
                if (this.machineName.Equals(this.physicalDisplayManager.machineName))
                {
                    display.gameObject.GetComponent<PhysicalDisplayCalibration>()?.SaveWarpFile();
                }
            }
            else
            {
                display.gameObject.GetComponent<PhysicalDisplayCalibration>()?.SaveWarpFile();
            }

        });
    }

    /// <summary>
    /// Loops over all <c>PhysicalDisplayCalibration</c> objects in <c>calibratedDisplays</c> and calls <see cref="PhysicalDisplayCalibration.SaveWarpFile">
    /// on them to save the WarpFile to disk.
    /// If <c>saveThisMachineOnly</c> is true, we only save the config files for the displays that has the same machine name value as
    /// the machine running the instance of Unity. Else we save the config file for each display. If we have 3 machines, with 3 displays on each
    /// we will save all 9 config files on each machine.
    /// </summary>
    private void SaveListOfCalibratedDisplays()
    {
        IEnumerable<PhysicalDisplayCalibration> calibrationEnumerator = this.calibratedDisplays.GetCalibratedDisplays();
        foreach (var display in calibrationEnumerator)
        {
            if (this.saveThisMachineOnly)
            {
                string machineName = display.GetComponent<PhysicalDisplay>()?.machineName;
                if (this.machineName.Equals(machineName))
                {
                    display.SaveWarpFile();
                }
            }
            else
            {
                display.SaveWarpFile();
            }

        }
    }

    private void OnDestroy()
    {
        this.SaveCalibrations();
    }

}
