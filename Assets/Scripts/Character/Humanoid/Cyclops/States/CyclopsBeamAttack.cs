using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsBeamAttack : CyclopsAttack
{
    #region BeamAttack Variables

    #endregion

    public CyclopsBeamAttack(CyclopsData characterData, EffectManager attackCollider) : base(characterData, attackCollider) { }

    public override IEnumerator EnterState(BaseState prevState)
    {
        data.StartCoroutine(ShootLaser(3));
        return base.EnterState(prevState);
    }

    protected override void UpdateAI() { }

    private IEnumerator ShootLaser(float chargetime)
    {
        beamCooldown = 10f;
        if (!data.EyeBeam.isPlaying) data.EyeBeam.Play();
        yield return new WaitForSeconds(chargetime);

        if (data.EyeBeam.isPlaying) data.EyeBeam.Stop();
        stateManager.ChangeState(new CyclopsAttackState(data));
    }

    public override void OnCollisionEnter(Collision collision)
    {
        Debug.Log("here");
        base.OnCollisionEnter(collision);
    }
}
