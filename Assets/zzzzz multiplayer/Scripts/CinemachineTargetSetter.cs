using Cinemachine;
using UnityEngine;
using Mirror;

public class CinemachineTargetSetter : NetworkBehaviour
{
    public CinemachineFreeLook freeLookCamera;

    void Start()
    {
        if (!isLocalPlayer) return;

        if (freeLookCamera == null)
            freeLookCamera = FindObjectOfType<CinemachineFreeLook>();

        if (freeLookCamera != null)
        {
            freeLookCamera.Follow = transform;
            freeLookCamera.LookAt = transform;
        }
        else
        {
            Debug.LogError("No CinemachineFreeLook camera found in the scene!");
        }
    }
}
