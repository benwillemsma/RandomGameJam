using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsAxeAttack : CyclopsAttack
{
    #region ChargeAttack Variables

    private float duration;

    #endregion

    public CyclopsAxeAttack(CyclopsData characterData, EffectManager attackCollider) : base(characterData, attackCollider)
    {
        duration = attackCollider.duration;
    }

    protected override void UpdateAI()
    {
        if (attackCollider.lastHitCol != null)
        {
            StartRecoil();
            duration = elapsedTime;
            elapsedTime = 0;
        }
        if (elapsedTime >= duration)
        {
            StopRecoil();
            stateManager.ChangeState(new CyclopsAttackState(data));
        }
    }

    private void StartRecoil()
    {
        anim.StopRecording();
        anim.StartPlayback();
        anim.speed = -0.5f;
    }

    private void StopRecoil()
    {
        anim.StopPlayback();
        anim.speed = 1;
        anim.Play("Stand", 0);
    }

    public override IEnumerator EnterState(BaseState prevState)
    {
        float direction = Vector3.Dot(data.transform.right, (player.transform.position - data.transform.position).normalized) * 2;
        
        anim.SetFloat("HackDirection", direction);
        anim.SetTrigger("HackAttack");
        anim.StartRecording(0);

        return base.EnterState(prevState);
    }

    public override IEnumerator ExitState(BaseState prevState)
    {
        anim.ResetTrigger("HackAttack");
        return base.EnterState(prevState);
    }
}
