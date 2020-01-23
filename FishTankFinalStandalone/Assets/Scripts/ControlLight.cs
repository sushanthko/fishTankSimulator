using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlLight : MonoBehaviour
{
    public Transform Light;
    public Light Sunlight;
    public float rotationSpeed = 10.0f;
    public float IntensityChangeRate = 0.5f;

    void FixedUpdate()
    {
        float intensity = Sunlight.intensity;

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.KeypadMinus))
        {
            transform.Rotate(Vector3.right, -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey("i"))
        {
            Sunlight.intensity = intensity + IntensityChangeRate * Time.deltaTime;
        }
        else if (Input.GetKey("j"))
        {
            if (intensity != 0)
            {
                Sunlight.intensity = intensity - IntensityChangeRate * Time.deltaTime;
            }
        }
    }
}
