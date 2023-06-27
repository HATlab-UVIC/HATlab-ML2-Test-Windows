using System;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;

public class VoiceIntentsTest : MonoBehaviour
{
    public PlaneDetection PlaneDetection;

    private readonly MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();

    [SerializeField, Tooltip("The text used to display status information for the example.")]
    private Text statusText = null;
    private bool KeyIsDown = false;

    // handle voice events

    public void Lock()
    {
        Debug.Log("Voice Command: Lock");
        statusText.text = "Voice Command: Lock";
        PlaneDetection.Lock();
    }

    public void Unlock()
    {
        Debug.Log("Voice Command: Unlock");
        statusText.text = "Voice Command: Unlock";
        PlaneDetection.Unlock();
    }

    public void OpenWindow()
    {
        Debug.Log("Voice Command: Open Window");
        statusText.text = "Voice Command: Open Window";
        PlaneDetection.OpenWindow();
    }

    public void CloseWindow()
    {
        Debug.Log("Voice Command: Close Windows");
        statusText.text = "Voice Command: Close Window";
        PlaneDetection.CloseWindow();
    }

}