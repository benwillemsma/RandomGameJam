using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public float preset1_strength = 50;
    public float preset1_duration = 0.5f;

    [Space(10)]
    public float preset2_strength = 100;
    public float preset2_duration = 0.5f;

    public static ThirdPersonCamera mainCamera;
    public static Transform player;

    private void Start()
    {
        if(!mainCamera) mainCamera = ThirdPersonCamera.Instance;
        if (!player) player = GameManager.player.transform;
    }

    public void Shakecamera(float strength, float duration)
    {
        float distance = (transform.position - player.position).magnitude;
        float falloff = Mathf.Max(0, distance);
        mainCamera.ShakeCamera(strength - falloff, duration);
    }

    public void ShakecameraPreset1()
    {
        float distance = (transform.position - player.position).magnitude;
        float falloff = Mathf.Max(0, distance);
        mainCamera.ShakeCamera(preset1_strength - falloff, preset1_duration);
    }

    public void ShakecameraPreset2()
    {
        float distance = (transform.position - player.position).magnitude;
        float falloff = Mathf.Max(0, distance);
        mainCamera.ShakeCamera(preset2_strength - falloff, preset2_duration);
    }
}
