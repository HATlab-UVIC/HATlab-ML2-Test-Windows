using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]

public class RuntimeCameraDepthNormal : MonoBehaviour
{
    public Camera Cam;
    public Material Mat;

    void OnEnable()
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
            Mat = new Material(Shader.Find("Hidden/Runtime Camera Depth Image Effect Shader"));
        }
    }

    /*private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit (source, destination, Mat);
    }*/
}
