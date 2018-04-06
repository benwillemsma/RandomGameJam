using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    private Vector3 offset;

    private void Start()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        offset = transform.position - GameManager.player.transform.position;
    }

    void Update ()
    {
        Vector3 newPosition = GameManager.player.transform.position.z * Vector3.forward + offset;
        transform.position = Vector3.Lerp(transform.position, newPosition, 5 * Time.deltaTime);
    }
}
