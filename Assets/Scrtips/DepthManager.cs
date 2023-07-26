using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;
using System;

public class DepthManager : MonoBehaviour
{
    private readonly MLPermissions.Callbacks permissionCallbacks = new();

    private bool permissionGranted;
    private bool isFrameAvailable = false;
    // private MLDepthCamera depthCamera = null;
    private MLDepthCamera.Data lastData = null;


    // value goes from 0 to 7.5

    public float depthImgMinDist = 0f;
    public float depthImgMaxDist = 7.5f;

    public float ambientRawImgMinDist = 0f;
    public float ambientRawImgMaxDist = 2000f;

    public float confidenceMinDist = 0f;
    public float confidenceMaxDist = 100f;

    [SerializeField, Tooltip("Timeout in milliseconds for data retrieval.")]
    private ulong timeout = 1000;

    [SerializeField]
    private Renderer imgRenderer;
    [SerializeField]
    private Renderer confidenceRenderer;

    private Texture2D ImageTexture = null;

    private readonly int minDepthMatPropId = Shader.PropertyToID("_MinDepth");
    private readonly int maxDepthMatPropId = Shader.PropertyToID("_MaxDepth");
    private readonly int mapTexMatPropId = Shader.PropertyToID("_MapTex");

    private Vector2 scale = new Vector2(1.0f, -1.0f);

    /// Under normal operations long range mode has a maximum frequency of 5fps and a range of up to 5m, in some cases this can go as far 7.5m.
    private MLDepthCamera.Mode mode = MLDepthCamera.Mode.LongRange;

    /// Flags used to specify what kind of data to request from Depth Camera
    private MLDepthCamera.CaptureFlags captureFlag = MLDepthCamera.CaptureFlags.DepthImage;


    [SerializeField, Tooltip("The text used to display error.")]
    private Text statusText = null;


    void Awake()
    {
        permissionCallbacks.OnPermissionGranted += OnPermissionGranted;
        permissionCallbacks.OnPermissionDenied += OnPermissionDenied;
        permissionCallbacks.OnPermissionDeniedAndDontAskAgain += OnPermissionDenied;

        Camera.main.depthTextureMode = DepthTextureMode.Depth;

        /*cameraModeDropdown.AddOptions(
            MLDepthCamera.Mode.LongRange
        );

        captureFlagsDropdown.AddOptions(
            MLDepthCamera.CaptureFlags.DepthImage,
            MLDepthCamera.CaptureFlags.Confidence,
            MLDepthCamera.CaptureFlags.AmbientRawDepthImage
        );*/
    }

    void Start()
    {
        var settings = new MLDepthCamera.Settings()
        {
            Mode = mode,
            Flags = captureFlag
        };
        // public static void SetSettings(Settings settings) => CurrentSettings = settings;
        // depthCamera = new MLDepthCamera();
        // depthCamera = new MLDepthCamera();
        MLDepthCamera.SetSettings(settings);

        MLPermissions.RequestPermission(MLPermission.DepthCamera, permissionCallbacks);
    }

    void Update()
    {
        try
        {

            if (!permissionGranted || !MLDepthCamera.IsConnected)
            {
                return;
            }

            var result = MLDepthCamera.GetLatestDepthData(timeout, out MLDepthCamera.Data data);
            isFrameAvailable = result.IsOk;
            if (result.IsOk)
            {
                lastData = data;
                // statusText.text += "Data retrived";
            }

            switch (captureFlag)
            {
                case MLDepthCamera.CaptureFlags.AmbientRawDepthImage:
                    if (lastData.AmbientRawDepthImage != null)
                    {
                        CheckAndCreateTexture(imgRenderer, (int)lastData.AmbientRawDepthImage.Value.Width, (int)lastData.AmbientRawDepthImage.Value.Height);

                        // ambientRawImgMinDist = ambientDepthMin.GetComponentInChildren<Slider>().value;
                        // ambientRawImgMaxDist = ambientDepthMax.GetComponentInChildren<Slider>().value;

                        AdjustRendererFloats(imgRenderer, ambientRawImgMinDist, ambientRawImgMaxDist);
                        ImageTexture.LoadRawTextureData(lastData.AmbientRawDepthImage.Value.Data);
                        ImageTexture.Apply();
                    }
                    break;
                case MLDepthCamera.CaptureFlags.DepthImage:
                    if (lastData.DepthImage != null)
                    {
                        CheckAndCreateTexture(imgRenderer, (int)lastData.DepthImage.Value.Width, (int)lastData.DepthImage.Value.Height);

                        statusText.text += "Texture created";


                        // depthImgMinDist = depthImgMin.GetComponentInChildren<Slider>().value;
                        // depthImgMaxDist = depthImgMax.GetComponentInChildren<Slider>().value;

                        AdjustRendererFloats(imgRenderer, depthImgMinDist, depthImgMaxDist);
                        ImageTexture.LoadRawTextureData(lastData.DepthImage.Value.Data);
                        ImageTexture.Apply();

                        statusText.text += "Image Texture Applied";
                    }
                    break;
                case MLDepthCamera.CaptureFlags.Confidence:
                    if (lastData.ConfidenceBuffer != null)
                    {
                        CheckAndCreateTexture(imgRenderer, (int)lastData.ConfidenceBuffer.Value.Width, (int)lastData.ConfidenceBuffer.Value.Height);

                        // confidenceMinDist = confidenceMin.GetComponentInChildren<Slider>().value;
                        // confidenceMaxDist = confidenceMax.GetComponentInChildren<Slider>().value;

                        // AdjustRendererFloats(confidenceRenderer, confidenceMinDist, confidenceMaxDist);

                        confidenceRenderer.material.SetTexture(mapTexMatPropId, ImageTexture);
                        ImageTexture.LoadRawTextureData(lastData.ConfidenceBuffer.Value.Data);
                        ImageTexture.Apply();
                    }
                    break;
            }

            statusText.text = "\nIs frame available: " + isFrameAvailable;
        }
        catch (Exception e)
        {
            statusText.text = e.Message +"\n" + e.ToString();

        }
    }

    private void OnDestroy()
    {
        permissionCallbacks.OnPermissionGranted -= OnPermissionGranted;
        permissionCallbacks.OnPermissionDenied -= OnPermissionDenied;
        permissionCallbacks.OnPermissionDeniedAndDontAskAgain -= OnPermissionDenied;
        DisonnectCamera();
    }

    private void OnPermissionDenied(string permission)
    {
        if (permission == MLPermission.Camera)
        {
            MLPluginLog.Error($"{permission} denied, example won't function.");
            statusText.text = $"{permission} denied, example won't function.";
        }
        else if (permission == MLPermission.DepthCamera)
        {
            MLPluginLog.Error($"{permission} denied, example won't function.");
            statusText.text = $"{permission} denied, example won't function.";

        }
    }

    private void OnPermissionGranted(string permission)
    {
        MLPluginLog.Debug($"Granted {permission}.");
        permissionGranted = true;
        ConnectCamera();
    }

    private void ConnectCamera()
    {
        var result = MLDepthCamera.Connect();
        if (result.IsOk && MLDepthCamera.IsConnected)
        {
            Debug.Log($"Connected to new depth camera with mode = {MLDepthCamera.CurrentSettings.Mode} and flags = {MLDepthCamera.CurrentSettings.Flags}");
        }
        else
        {
            Debug.LogError($"Failed to connect to camera: {result.Result}");
        }
    }

    private void DisonnectCamera()
    {
        var result = MLDepthCamera.Disconnect();
        if (result.IsOk && !MLDepthCamera.IsConnected)
        {
            Debug.Log($"Disconnected depth camera with mode = {MLDepthCamera.CurrentSettings.Mode} and flags = {MLDepthCamera.CurrentSettings.Flags}");
        }
        else
        {
            Debug.LogError($"Failed to disconnect to camera: {result.Result}");
        }
    }

    private void CheckAndCreateTexture(Renderer renderer, int width, int height)
    {
        if (ImageTexture == null || ImageTexture.width != width || ImageTexture.height != height)
        {
            ImageTexture = new Texture2D(width, height, TextureFormat.RFloat, false);
            ImageTexture.filterMode = FilterMode.Bilinear;
            var material = renderer.material;
            material.mainTexture = ImageTexture;
            material.mainTextureScale = scale;
        }
        if (ImageTexture == null )
        {
            statusText.text += "Empty image texture did not go through";
        }
        else
        {
            statusText.text += "Empty image texture went through";
        }
    }

    private void AdjustRendererFloats(Renderer renderer, float minValue, float maxValue)
    {
        renderer.material.SetFloat(minDepthMatPropId, minValue);
        renderer.material.SetFloat(maxDepthMatPropId, maxValue);
        renderer.material.SetTextureScale(mapTexMatPropId, scale);
    }

    // Update Setting is called after UI update.

    private void UpdateSettings()
    {
        var settings = new MLDepthCamera.Settings()
        {
            Mode = mode,
            Flags = captureFlag
        };

        MLDepthCamera.UpdateSettings(settings);
    }
}
