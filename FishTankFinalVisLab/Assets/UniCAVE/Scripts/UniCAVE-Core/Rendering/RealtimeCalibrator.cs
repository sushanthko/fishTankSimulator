using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RealtimeCalibrator : NetworkBehaviour {
    public struct CalibrationSelection {
        public string machineName;
        public PhysicalDisplayCalibration calibration;
    }

    private enum CalibrationType {
        VERTEX,
        POSITION,
        ROTATION
    }

    private CalibrationType calibrationType = CalibrationType.VERTEX;

    public int selectedIndex = 0;
    public int lastSelectedIndex = 0;
    public List<CalibrationSelection> allOptions;

    public int vertexIndex = 0;

    /// <summary>
    /// Holds a reference of the display object to be shown
    /// when calibrating
    /// </summary>
    [SerializeField]
    private InfoDisplay infoDisplay;

    /// <summary>
    /// The instantiated instance of InfoDisplay for the right eye/cam
    /// </summary>
    private InfoDisplay infoDisplayInstance;

    private int gridSelectSize = 1;

    void Start () {
        allOptions = new List<CalibrationSelection> ();
        //generate list of options
        List<PhysicalDisplay> displays = gameObject.GetComponent<UCNetwork> ().GetAllDisplays ();
        foreach (PhysicalDisplay disp in displays) {
            PhysicalDisplayCalibration cali = disp.gameObject.GetComponent<PhysicalDisplayCalibration> ();
            if (cali != null) {
                CalibrationSelection selection;
                selection.machineName = (disp.manager == null) ? disp.machineName : disp.manager.machineName;
                selection.calibration = cali;
                allOptions.Add (selection);
            }
        }

        Debug.Log ("RealtimeCalibration: Found " + allOptions.Count + " calibration objects");

        StartCoroutine (InitiateInfoScreen ());
    }

    /// <summary>
    /// Instatiate the info screen with a delay.
    /// So we are sure everything has initialized before
    /// setting the info screen
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitiateInfoScreen () {
        yield return new WaitForSeconds (3);
        this.CreateInfoDisplays ();
        this.InfoDisplayShift (this.selectedIndex);
        this.RpcInfoDisplayShift (this.selectedIndex);
        yield break;
    }

    /// <summary>
    /// Instantiates the info displays.
    /// And set them to disabled at start
    /// </summary>
    private void CreateInfoDisplays () {
        if (this.infoDisplay == null) return;
        this.infoDisplayInstance = Instantiate (infoDisplay);
        this.infoDisplayInstance.gameObject.SetActive (false);
    }

    private void LocalShift (Vector2 direction, float delta, int selectedIndex, int vertexIndex) {
        PhysicalDisplayCalibration lastCalibration = allOptions[lastSelectedIndex].calibration;
        lastCalibration.HideVisualMarker ();
        PhysicalDisplayCalibration calibration = allOptions[selectedIndex].calibration;
        calibration.SetVisualMarkerVertextPoint (vertexIndex);

        Debug.Log ("RealtimeCalibration: LocalShift called " + delta + ", " + selectedIndex + ", " + vertexIndex);

        MeshFilter lastWarpedFilter = null;
        foreach (Dewarp dewarp in calibration.GetDisplayWarpsValues ()) {
            MeshFilter meshFilter = dewarp.GetDewarpMeshFilter ();
            lastWarpedFilter = meshFilter;
            Vector3[] verts = meshFilter.sharedMesh.vertices;
            // verts[vertexIndex] = new Vector3(verts[vertexIndex].x + direction.x * delta, verts[vertexIndex].y + direction.y * delta, verts[vertexIndex].z);

            Dictionary<int, float> vertsToShift = this.moveVerts (dewarp.xSize, vertexIndex);
            foreach (var ind in vertsToShift) {
                verts[ind.Key] = new Vector3 (verts[ind.Key].x + (direction.x * delta * ind.Value), verts[ind.Key].y + (direction.y * delta * ind.Value), verts[ind.Key].z);
            }

            meshFilter.sharedMesh.vertices = verts;
            meshFilter.sharedMesh.UploadMeshData (false);
            meshFilter.mesh.RecalculateBounds ();
            meshFilter.mesh.RecalculateTangents ();

        }

        calibration.UpdateMeshPositions (lastWarpedFilter?.sharedMesh.vertices);

    }

    public Dictionary<int, float> moveVerts (int size, int index) {

        int sizeX = size;

        int indexSizeX = size + 1;

        // The row the selected index is on
        int indexRow = (index / (sizeX + 1)) < 1.0f ? 0 : (int) Mathf.Floor (index / (sizeX + 1));

        int startRow = (indexRow - this.gridSelectSize) >= 0 ? (indexRow - this.gridSelectSize) : 0;
        int endRow = (indexRow + this.gridSelectSize) <= sizeX ? (indexRow + this.gridSelectSize) : sizeX;

        // Vertecies to move
        Dictionary<int, float> vertsToShift = new Dictionary<int, float> ();

        float moveFactor = (float) this.gridSelectSize / (float) ((this.gridSelectSize * 2));
        float currentMoveFactor = moveFactor;

        for (int row = startRow; row <= endRow; row++) {
            int rowDiff = indexRow - row;

            int startIndexForRow = 0;
            int stopIndexForRow = 0;
            int midIndexForRow = 0;

            int minSize = row * indexSizeX;
            int maxSize = minSize + sizeX;

            startIndexForRow = index - (indexSizeX * (rowDiff)) - this.gridSelectSize;
            startIndexForRow = (startIndexForRow < minSize) ? minSize : startIndexForRow;

            midIndexForRow = index - (indexSizeX * (rowDiff));

            stopIndexForRow = index - (indexSizeX * (rowDiff)) + this.gridSelectSize;
            stopIndexForRow = (stopIndexForRow > maxSize) ? maxSize : stopIndexForRow;

            int factor = 2;
            for (int i = midIndexForRow + 1; i <= stopIndexForRow; i++) {
                vertsToShift.Add (i, moveFactor / factor);
                factor++;
            }
            factor = 2;
            for (int i = midIndexForRow - 1; i >= startIndexForRow; i--) {
                vertsToShift.Add (i, moveFactor / factor);
                factor++;
            }

            if (midIndexForRow == index) {
                vertsToShift.Add (midIndexForRow, 1);
            } else {
                vertsToShift.Add (midIndexForRow, moveFactor);
            }

        }
        return vertsToShift;

    }

    private void LocalPositionShift (Vector3 direction, float delta, int selectedIndex) {
        PhysicalDisplayCalibration lastCalibration = allOptions[lastSelectedIndex].calibration;
        lastCalibration.HideVisualMarker ();
        PhysicalDisplayCalibration calibration = allOptions[selectedIndex].calibration;
        calibration.SetVisualMarkerVertextPoint (vertexIndex);

        calibration.MoveDisplay (new Vector3 (direction.x * delta, direction.y * delta, direction.z * delta));
    }

    private void LocalRotationShift (Vector3 direction, float delta, int selectedIndex) {
        PhysicalDisplayCalibration lastCalibration = allOptions[lastSelectedIndex].calibration;
        lastCalibration.HideVisualMarker ();
        PhysicalDisplayCalibration calibration = allOptions[selectedIndex].calibration;
        calibration.SetVisualMarkerVertextPoint (vertexIndex);

        calibration.RotateDisplay (new Vector3 (direction.x * delta, direction.y * delta, direction.z * delta));
    }

    /// <summary>
    /// Shifts the info window around to the display on the given index
    /// </summary>
    /// <param name="selectedIndex">index of the display</param>
    private void InfoDisplayShift (int selectedIndex) {
        PhysicalDisplayCalibration currentDisplay = this.allOptions[selectedIndex].calibration;
        if (currentDisplay == null || this.infoDisplayInstance == null) return;

        if (currentDisplay.GetDisplayWarpsValues ().Count () > 0) {
            this.SetInfoDisplay (infoDisplayInstance.gameObject, currentDisplay.GetDisplayWarpsValues ().First ().GetDewarpGameObject ().transform);
            this.infoDisplayInstance.SetText (this.calibrationType.ToString ());
        }
    }

    /// <summary>
    /// Activates the info display, sets it parent and resets its local position
    /// so it is center to the parent
    /// </summary>
    /// <param name="infoDisplay"></param>
    /// <param name="parent"></param>
    private void SetInfoDisplay (GameObject infoDisplay, Transform parent) {
        infoDisplay.gameObject.SetActive (true);
        infoDisplay.transform.SetParent (parent);
        infoDisplay.transform.localPosition = new Vector2 (0, 0);
    }

    [ClientRpc]
    void RpcShift (Vector2 direction, float delta, int selectedIndex, int vertexIndex) {
        LocalShift (direction, delta, selectedIndex, vertexIndex);
    }

    [ClientRpc]
    void RpcMovePosition (Vector2 direction, float delta, int selectedIndex) {
        Debug.Log (direction);

        LocalPositionShift (direction, delta, selectedIndex);
    }

    [ClientRpc]
    void RpcRotate (Vector2 direction, float delta, int selectedIndex) {
        Debug.Log (direction);

        LocalRotationShift (direction, delta, selectedIndex);
    }

    [ClientRpc]
    void RpcSetCalibrationType (CalibrationType calibrationType) {
        this.calibrationType = calibrationType;
        this.infoDisplayInstance.SetText (calibrationType.ToString ());
    }

    /// <summary>
    /// Triggers <c>InfoDisplayShift</c> on connected clients
    /// </summary>
    /// <param name="selectedIndex">the selected display index</param>
    [ClientRpc]
    void RpcInfoDisplayShift (int selectedIndex) {
        this.InfoDisplayShift (selectedIndex);
    }

    /// <summary>
    /// RPC method to trigger <c>LocalAdjustGridSelectSize</c> the adjustment of grid selection size of vertex movement.
    /// Uses bool to tell if it is a increase or decrease.
    /// 
    /// </summary>
    /// <param name="increase">true to increase, false to decrease</param>
    [ClientRpc]
    void RpcAdjustGridSelectSize (bool increase) {
        LocalAdjustGridSelectSize (increase);
    }

    /// <summary>
    /// Adjust the grid selection size of vertex movement.
    /// Uses bool to tell if it is a increase or decrease.
    /// 
    /// </summary>
    /// <param name="increase">true to increase, false to decrease</param>
    private void LocalAdjustGridSelectSize (bool increase) {
        if (increase) {
            this.gridSelectSize++;
        } else {
            this.gridSelectSize = (this.gridSelectSize--) <= 0 ? 0 : this.gridSelectSize--;
        }
    }

    /// <summary>
    /// Triggers adjustment of grid selection size of vertex movement.
    /// Uses bool to tell if it is a increase or decrease.
    /// 
    /// </summary>
    /// <param name="increase">true to increase, false to decrease</param>
    private void AdjustGridSelectSize (bool increase) {
        this.LocalAdjustGridSelectSize (increase);
        this.RpcAdjustGridSelectSize (increase);
    }

    /// <summary>
    /// Moves a vertex in a given direction in a speed/steps of delta
    /// </summary>
    /// <param name="direction">the direction to move</param>
    /// <param name="delta">the speed/steps of movement</param>
    private void VertexShift (Vector2 direction, float delta) {
        LocalShift (direction, delta, selectedIndex, vertexIndex);
        RpcShift (direction, delta, selectedIndex, vertexIndex);
    }

    /// <summary>
    /// Moves a window in a given direction in a speed/steps of delta
    /// </summary>
    /// <param name="direction">the direction to move</param>
    /// <param name="delta">the speed/steps of movement</param>
    private void PositionShift (Vector3 direction, float delta) {
        this.LocalPositionShift (direction, delta, selectedIndex);
        this.RpcMovePosition (direction, delta, selectedIndex);
    }

    /// <summary>
    /// Rotates a window in a given direction in a speed/steps delta
    /// </summary>
    /// <param name="direction">the direction of rotation</param>
    /// <param name="delta">the speed/steps of the rotation</param>
    private void RotationShift (Vector3 direction, float delta) {
        this.LocalRotationShift (direction, delta, selectedIndex);
        this.RpcRotate (direction, delta, selectedIndex);
    }

    /// <summary>
    /// Moves the info display to the next window.
    /// </summary>
    private void DisplayShift () {
        InfoDisplayShift (selectedIndex);
        RpcInfoDisplayShift (selectedIndex);
    }

    /// <summary>
    /// Sets the give index as the last index, and trigger RPC
    /// method <c>RpcSetLastIndex</c> so slaves follow.
    /// </summary>
    /// <param name="index">the index to set as last index</param>
    private void SetLastIndex (int index) {
        this.lastSelectedIndex = index;
        this.RpcSetLastIndex (index);
    }

    [ClientRpc]
    private void RpcSetLastIndex (int index) {
        this.lastSelectedIndex = index;
    }

    /// <summary>
    /// Cycles to the next calibration type, if it reaches the end
    /// start over.
    /// </summary>
    /// <returns>type of calibration :(pos, rot, vertex)</returns>
    private CalibrationType CycleNextCalibrationType () {
        this.calibrationType = (from CalibrationType val in Enum.GetValues (typeof (CalibrationType)) where val > this.calibrationType orderby val select val).DefaultIfEmpty ().First ();
        this.RpcSetCalibrationType (this.calibrationType);
        this.infoDisplayInstance.SetText (this.calibrationType.ToString ());
        return this.calibrationType;
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.Space)) {
            this.CycleNextCalibrationType ();
        }

        if (Input.GetKeyDown (KeyCode.KeypadPlus)) {
            this.AdjustGridSelectSize (true);
        }

        if (Input.GetKeyDown (KeyCode.KeypadMinus)) {
            this.AdjustGridSelectSize (false);
        }

        Vector3 direction = Vector3.zero;
        bool anyPressed = false;
        bool noOptions = allOptions.Count == 0;

        if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
            if (Input.GetKeyDown (KeyCode.Return)) {
                int index = this.selectedIndex - 1;
                if (index < 0) {
                    this.selectedIndex = allOptions.Count - 1;
                } else {
                    this.selectedIndex = Mathf.Abs ((selectedIndex - 1) % allOptions.Count);
                }
                if (!noOptions) {
                    DisplayShift ();
                    VertexShift (direction, 1f);
                }
            }

        } else if (Input.GetKeyDown (KeyCode.Return)) {
            this.selectedIndex = (selectedIndex + 1) % allOptions.Count;
            if (!noOptions) {
                DisplayShift ();
                VertexShift (direction, 1f);
            }
        }

        if (Input.GetKeyDown (KeyCode.A)) {

            int lastIndex = this.vertexIndex - 1;
            this.vertexIndex = (lastIndex < 0) ? 63 : lastIndex;
            Debug.Log (vertexIndex);
            if (!noOptions) {
                Debug.Log (vertexIndex);
                this.VertexShift (direction, 1f);
            }

        }

        if (Input.GetKeyDown (KeyCode.D)) {
            this.vertexIndex = (vertexIndex + 1) % 63;
            if (!noOptions) {
                Debug.Log (vertexIndex);
                this.VertexShift (direction, 1f);
            }
        }

        if (noOptions) { return; }

        if (Input.GetKey (KeyCode.RightArrow)) {
            direction.x = 1;
            anyPressed = true;
        } else if (Input.GetKey (KeyCode.UpArrow)) {
            direction.y = 1;
            anyPressed = true;
        } else if (Input.GetKey (KeyCode.LeftArrow)) {
            direction.x = -1;
            anyPressed = true;
        } else if (Input.GetKey (KeyCode.DownArrow)) {
            direction.y = -1;
            anyPressed = true;
        } else if (Input.GetKey (KeyCode.Keypad8)) {
            direction.z = -1;
            anyPressed = true;
        } else if (Input.GetKey (KeyCode.Keypad2)) {
            direction.z = 1;
            anyPressed = true;
        }

        if (anyPressed) {
            Debug.Log ("RealtimeCalibration: isServer = " + isServer);
            if (isServer) {
                switch (this.calibrationType) {
                    case CalibrationType.POSITION:
                        this.PositionShift (direction, 0.0015f);
                        break;
                    case CalibrationType.ROTATION:
                        this.RotationShift (direction, 0.10f);
                        break;
                    case CalibrationType.VERTEX:
                        this.VertexShift (direction, 0.0015f);
                        break;
                }

            }
        }
        this.SetLastIndex (selectedIndex);
    }
}