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
    public Collider firstHitCol;
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
        affectedCharaters.Clear();
        firstHitCol = null;
        lastHitCol = null;
        collider.enabled = true;
    }

    private void OnDisable()
    {
        collider.enabled = false;
        lastHitCol = null;
        firstHitCol = null;
        affectedCharaters.Clear();
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
        if (collision.rigidbody && collision.rigidbody == owningCharacter.RB)
            return;
        
        lastHitCol = collision.collider;
        if (!firstHitCol) firstHitCol = collision.collider; 
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
        if (!enabled)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= duration) enabled = false;
        }
        else elapsedTime = 0;
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
