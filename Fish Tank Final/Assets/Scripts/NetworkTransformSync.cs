using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Synchronizes the servers transform position to the client.
/// </summary>
[RequireComponent(typeof(NetworkIdentity))]
public class NetworkTransformSync : NetworkBehaviour
{
    [SyncVar]
    Quaternion newRotation;

    [SyncVar]
    Vector3 newPosition;

    void LateUpdate()
    {
        if (isServer)
        {
            this.newRotation = this.transform.rotation;
            this.newPosition = this.transform.position;
            return;
        };
        this.transform.position = newPosition;
        this.transform.rotation = newRotation;
    }
}