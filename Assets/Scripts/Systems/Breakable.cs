using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Breakable : MonoBehaviour
{
    public GameObject brokenObject;
    public string[] checkTags;
    public Breakable[] childBreakables;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < checkTags.Length; i++)
        {
            if (other.attachedRigidbody.tag.Contains(checkTags[i]))
                StartCoroutine(Break(0));
        }
    }

    public IEnumerator Break(float delay)
    {
        yield return new WaitForSeconds(delay);
        Transform objects = Instantiate(brokenObject, transform.position, transform.rotation).transform;

        for (int i = 0; i < objects.childCount; i++)
        {
            delay = Random.Range(0, 5) + GameManager.Instance.objectDestroyDelay;
            Breakable childBreakable = objects.GetChild(i).GetComponent<Breakable>();

            if (!childBreakable) Destroy(objects.GetChild(i).gameObject, delay);
        }
        DestroyImmediate(gameObject);
    }
}
