using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]

public class RuntimeCameraDepthNormal : MonoBehaviour
{
    public Camera Cam;
    public Material Mat;

    void Awake()
    {
        Cam.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        if ( Cam == null )
        {
            Debug.Log("Camera does not exist");
            return;
        }

        if ( Mat == null )
        {
            Mat = new Material(Shader.Find("Unlit/Runtime Camera Depth Unlit"));
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, Mat);
    }
}
