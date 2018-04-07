using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class EffectManager : MonoBehaviour
{
    public CharacterData owningCharacter;
    public new Collider collider;

    private bool overTimeAvailable = false;
    public float overTimeCooldown;

    [Space(10)]
    public string[] affectTags;
    public List<CharacterData> affectedCharaters = new List<CharacterData>();

    [HideInInspector]
    public List<Effect> effects = new List<Effect>();


    void Start()
    {
        if (!collider) collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        if (!collider) collider = GetComponent<Collider>();
        collider.enabled = true;
    }

    private void OnDisable()
    {
        collider.enabled = false;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < affectTags.Length; i++)
        {
            if (collision.gameObject.tag == affectTags[i])
            {
                CharacterData character = collision.collider.attachedRigidbody.GetComponent<CharacterData>();
                if (character && character != owningCharacter && !affectedCharaters.Contains(character))
                {
                    affectedCharaters.Add(character);
                    for (int e = 0; e < effects.Count; e++)
                        effects[e].OnEffectStart(character);
                }
            }
        }
    }

    protected void Update()
    {
        if (overTimeAvailable)
        {
            overTimeAvailable = false;
            for (int i = 0; i < affectedCharaters.Count; i++)
            {
                if (affectedCharaters[i])
                {
                    for (int e = 0; e < effects.Count; e++)
                        effects[e].OnEffectStay(affectedCharaters[i]);
                }
            }
            StartCoroutine(GameManager.CallAfterDelay(() => overTimeAvailable = true, overTimeCooldown));
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        Debug.Log(collision.collider, collision.collider);
        CharacterData character = collision.collider.attachedRigidbody.GetComponent<CharacterData>();
        if (character && character != owningCharacter && affectedCharaters.Contains(character))
        {
            affectedCharaters.Remove(character);
            for (int e = 0; e < effects.Count; e++)
                effects[e].OnEffectStart(character);
        }
    }
}
