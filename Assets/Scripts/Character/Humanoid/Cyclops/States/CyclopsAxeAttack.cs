using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsAxeAttack : CyclopsAttack
{
    #region ChargeAttack Variables

    private float duration;
    private bool isRecoiling;
    private float min;
    private float max;

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

        float angle = Vector3.SignedAngle(data.transform.forward, (player.transform.position - data.transform.position).normalized, Vector3.up);
        if (angle > 0)
        {
            min = 0.1f;
            max = 60;
        }
        else
        {
            max = -0.1f;
            min = -60;
        }

        return base.EnterState(prevState);
    }

    public override IEnumerator ExitState(BaseState prevState)
    {
        anim.ResetTrigger("HackAttack");
        return base.EnterState(prevState);
    }

    protected override void UpdateAnimator()
    {
        float angle = Vector3.SignedAngle(data.transform.forward, (player.transform.position - data.transform.position).normalized, Vector3.up);
        angle = Mathf.Clamp(angle, min, max);
        anim.SetFloat("HackDirection", angle);
    }

    protected override void UpdateAI()
    {
        if (attackCollider.firstHitCol != null && !isRecoiling)
        {
            attackCollider.enabled = false;
            data.StartCoroutine(RecoilDelay(0.5f));
            cameraShaker.Shakecamera(200, 0.5f);
        }
        if (elapsedTime >= duration)
        {
            StopRecoil();
            stateManager.ChangeState(new CyclopsAttackState(data));
        }
    }

    private IEnumerator RecoilDelay(float delay)
    {
        isRecoiling = true;
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
