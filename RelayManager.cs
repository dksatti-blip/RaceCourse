
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;

public class RelayManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeText;
    public TMP_Text statusText;
    public GameObject menuCanvas;

    private bool busy;

    public async void StartHost()
    {
        if (busy) return;
        busy = true;

        try
        {
            await InitializeServices();

            Allocation allocation =
                await RelayService.Instance.CreateAllocationAsync(7);

            string joinCode =
                await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            if (joinCodeText != null)
                joinCodeText.text = "Join Code: " + joinCode;

            var transport =
                NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, "dtls"));

            if (NetworkManager.Singleton.StartHost())
            {
                if (joinCodeText != null)
                    joinCodeText.text = "Join Code: " + joinCode;

                SetStatus("Host started. Share the join code!");

                if (menuCanvas != null)
                    menuCanvas.SetActive(false);
            }
            else
            {
                SetStatus("Could not start host.");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            SetStatus("Host failed: " + e.Message);
        }
        finally
        {
            busy = false;
        }
    }

    public async void StartClient()
    {
        if (busy) return;

        string code = joinCodeInput != null
            ? joinCodeInput.text.Trim()
            : "";

        if (string.IsNullOrWhiteSpace(code))
        {
            SetStatus("Enter the host's join code first.");
            return;
        }

        busy = true;

        try
        {
            await InitializeServices();

            JoinAllocation allocation =
                await RelayService.Instance.JoinAllocationAsync(code);

            var transport =
                NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, "dtls"));

            if (NetworkManager.Singleton.StartClient())
            {
                SetStatus("Connecting to host...");

                if (menuCanvas != null)
                    menuCanvas.SetActive(false);
                }
                else
                {
                    SetStatus("Could not start client.");
                }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            SetStatus("Join failed: " + e.Message);
        }
        finally
        {
            busy = false;
        }
    }

    private async Task InitializeServices()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void SetStatus(string message)
    {
        Debug.Log(message);
        if (statusText != null)
            statusText.text = message;
    }
}
