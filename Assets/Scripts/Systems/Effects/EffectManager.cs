using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class EffectManager : MonoBehaviour
{
    public CharacterData owningCharacter;
    public new Collider collider;

    private float elapsedTime;
    public float duration;
    private bool overTimeAvailable = false;
    public float overTimeCooldown;

    [Space(10)]
    public string[] affectTags;
    public List<CharacterData> affectedCharaters = new List<CharacterData>();
    [HideInInspector]
    public Collider lastHitCol;

    [HideInInspector]
    public List<Effect> effects = new List<Effect>();

    private void Awake()
    {
        if (!collider) collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void OnEnable()
    {
        if (!collider) collider = GetComponent<Collider>();
        lastHitCol = null;
        collider.enabled = true;
        elapsedTime = 0;
    }

    private void OnDisable()
    {
        elapsedTime = 0;
        collider.enabled = false;
        lastHitCol = null;
    }

    protected void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < affectTags.Length; i++)
        {
            if (other.gameObject.tag == affectTags[i])
            {
                CharacterData character = other.attachedRigidbody.GetComponent<CharacterData>();
                if (character && character != owningCharacter && !affectedCharaters.Contains(character))
                {
                    affectedCharaters.Add(character);
                    for (int e = 0; e < effects.Count; e++)
                        effects[e].OnEffectStart(character);
                }
            }
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        lastHitCol = collision.collider;
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
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= duration) enabled = false;
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            CharacterData character = other.attachedRigidbody.GetComponent<CharacterData>();
            if (character && character != owningCharacter && affectedCharaters.Contains(character))
            {
                affectedCharaters.Remove(character);
                for (int e = 0; e < effects.Count; e++)
                    effects[e].OnEffectEnd(character);
            }
        }
    }
}
