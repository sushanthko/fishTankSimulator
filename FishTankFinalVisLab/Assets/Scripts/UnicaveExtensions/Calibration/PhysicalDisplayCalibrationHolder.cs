using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A helper class for holding <see cref="PhysicalDisplayCalibration" /> objects,
/// which is used to hold references for <see cref="PhysicalDisplayCalibrationSaver" />
/// so we can show/hide the field in the inspector.
/// 
/// Thats why this class is serializable, so Unity is able to display
/// the fields in the inspector.
/// 
/// Author: Christoffer A Træen
/// </summary>
[Serializable]
public class PhysicalDisplayCalibrationHolder
{
	/// <summary>
	/// Holds objects of PhysicalDisplayCalibration
	/// </summary>
	[SerializeField]
	private List<PhysicalDisplayCalibration> calibratedDisplays = new List<PhysicalDisplayCalibration>();

	/// <summary>
	/// Returns a Enumerable of PhysicalDisplayCalibration objects
	/// </summary>
	/// <returns>Array of PhysicalDisplayCalibration</returns>
	public IEnumerable<PhysicalDisplayCalibration> GetCalibratedDisplays()
	{
		return this.calibratedDisplays;
	}

	/// <summary>
	/// Adds a display to the list of calibrated displays
	/// </summary>
	public void AddCalibratedDisplay(PhysicalDisplayCalibration display)
	{
		this.calibratedDisplays.Add(display);
	}
}
