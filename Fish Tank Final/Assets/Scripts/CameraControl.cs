using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Camera subMarineCamera;
    public Camera overheadCamera;
    public float FOVChangeRate = 2.0f;

    void ShowOverheadView()
    {
        subMarineCamera.enabled = false;
        overheadCamera.enabled = true;
    }

    void ShowSubMarineView()
    {
        subMarineCamera.enabled = true;
        overheadCamera.enabled = false;
    }

    void FixedUpdate()
    {
        float fov = subMarineCamera.fieldOfView;

        if (Input.GetKey("1"))
        {
            ShowSubMarineView();
        } else if(Input.GetKey("2"))
        {
            ShowOverheadView();
        }
        else if (Input.GetKey("f"))
        {
            if(fov != 179)
            {
                subMarineCamera.fieldOfView = fov + FOVChangeRate * Time.deltaTime;
            }
            
        }
        else if (Input.GetKey("v"))
        {
            if (fov != 0)
            {
                subMarineCamera.fieldOfView = fov - FOVChangeRate * Time.deltaTime;
            }
        }
    }
}
