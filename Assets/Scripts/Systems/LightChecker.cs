using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LightChecker : MonoBehaviour
{
    public static LightChecker instance;
    private new Camera camera;

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(gameObject);
        camera = GetComponent<Camera>();
    }

    public float GetLIghtValue(Vector3 position)
    {
        transform.position = position + Vector3.up * (camera.nearClipPlane + Mathf.Epsilon);

        Texture2D tex = new Texture2D(32, 32, TextureFormat.RGB24, false);

        camera = GetComponent<Camera>();
        camera.orthographic = true;

        RenderTexture RT = new RenderTexture(32, 32, 24);
        camera.targetTexture = RT;
        camera.Render();
        RenderTexture.active = RT;

        tex.ReadPixels(new Rect(0, 0, 32, 32), 0, 0);
        tex.Apply();
        Color[] pixels = tex.GetPixels();

        camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(RT);

        float lightlevel = 0;
        for (int i = 0; i < pixels.Length; i++)
            lightlevel += pixels[i].grayscale;
        lightlevel /= pixels.Length;
        return lightlevel;
    }
}
