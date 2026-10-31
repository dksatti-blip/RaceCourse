using UnityEngine;
using Unity.Netcode;

public class PlayerCamera : NetworkBehaviour
{
    private Camera playerCamera;
    private AudioListener audioListener;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>(true);

        if (playerCamera != null)
            audioListener = playerCamera.GetComponent<AudioListener>();
    }

    public override void OnNetworkSpawn()
    {
        bool isLocalPlayer = IsOwner;

        if (playerCamera != null)
            playerCamera.enabled = isLocalPlayer;

        if (audioListener != null)
            audioListener.enabled = false;
    }
}