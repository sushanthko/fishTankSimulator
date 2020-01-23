using UnityEngine;

public class SubMarineMovement : MonoBehaviour
{
    public Rigidbody submarine;
    public float movementSpeed = 5.0f;
    public float rotationSpeed = 200.0f;

    void FixedUpdate()
    {
        transform.Rotate(0, Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed, 0);
        transform.Translate(0, 0, Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed);
    }
}
