﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CyclopsAxeAttack : CyclopsAttack
{
    #region ChargeAttack Variables

    private float duration;
    private bool recoiling;

    #endregion

    public CyclopsAxeAttack(CyclopsData characterData, EffectManager attackCollider) : base(characterData, attackCollider)
    {
        cameraShaker = attackCollider.GetComponent<CameraShaker>();
        duration = attackCollider.duration;
    }

    public override IEnumerator EnterState(BaseState prevState)
    {
        anim.SetTrigger("HackAttack");
        anim.StartRecording(0);

        return base.EnterState(prevState);
    }

    public override IEnumerator ExitState(BaseState prevState)
    {
        anim.ResetTrigger("HackAttack");
        return base.EnterState(prevState);
    }

    protected override void UpdateAnimator()
    {
        if (!recoiling)
        {
            float angle = Vector3.SignedAngle(data.transform.forward, (player.transform.position - data.transform.position).normalized, Vector3.up);
            anim.SetFloat("HackDirection", angle);
        }
    }

    protected override void UpdateAI()
    {
        SetDestination();
        if (attackCollider.firstHitCol != null && !recoiling)
        {
            attackCollider.enabled = false;
            data.StartCoroutine(RecoilDelay(0.5f));
            cameraShaker.Shakecamera(200, 0.5f);
        }
        else if (elapsedTime >= duration)
        {
            StopRecoil();
            stateManager.ChangeState(new CyclopsAttackState(data));
        }
    }

    private IEnumerator RecoilDelay(float delay)
    {
        recoiling = true;
        anim.speed = 0;
        anim.StopRecording();
        duration = elapsedTime + delay;
        elapsedTime = 0;

        yield return new WaitForSeconds(delay);
        StartRecoil();
    }

    private void StartRecoil()
    {
        anim.StartPlayback();
        anim.speed = -0.8f;
    }

    private void StopRecoil()
    {
        anim.StopPlayback();
        anim.speed = 1;
        anim.Play("Stand", 0);
    }
}
