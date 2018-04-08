using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Breakable : MonoBehaviour
{
    public GameObject brokenObject;
    public string[] checkTags;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < checkTags.Length; i++)
        {
            if (other.attachedRigidbody.tag.Contains(checkTags[i]))
                Break(); 
        }
    }

    public void Break()
    {
        Transform objects = Instantiate(brokenObject, transform.position, transform.rotation).transform;

        for (int i = 0; i < objects.childCount; i++)
        {
            float delay = Random.Range(0, 5) + GameManager.Instance.objectDestroyDelay;
            Destroy(objects.GetChild(i).gameObject, delay);
        }
        Destroy(gameObject);
    }
}
