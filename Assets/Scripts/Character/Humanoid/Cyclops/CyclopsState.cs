using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CyclopsState : EnemyState<CyclopsData>
{
    #region CyclopsState Variables

    protected PlayerData player;
    protected bool hasDetectedPlayer;

    #endregion

    public CyclopsState(CyclopsData cyclops) : base(cyclops)
    {
        player = GameManager.player;
    }

    protected override void UpdateAI()
    {
        TryDetectPlayer();
        UpdateDetectionState();
    }

    protected void TryDetectPlayer()
    {
        hasDetectedPlayer = CheckLineOfSight();

        if (hasDetectedPlayer) data.detectionLevel += Time.deltaTime;
        else data.detectionLevel -= Time.deltaTime; 

        data.detectionLevel = Mathf.Clamp(data.detectionLevel, 0, PlayerData.MaxDetection);
    }

    protected void UpdateDetectionState()
    {
        float playerDetection = player.TotalDetection(data.transform.position) + data.detectionLevel;
        if (playerDetection > data.attackThreshold)
        {
            if (GetType() != typeof(CyclopsAttackState))
                stateManager.ChangeState(new CyclopsAttackState(data));
        }

        else if (playerDetection > data.walkingThreshold)
        {
            if (GetType() != typeof(CyclopsSearchState))
                stateManager.ChangeState(new CyclopsSearchState(data));
        }
        else
        {
            if (GetType() != typeof(CyclopsWalkingState))
                stateManager.ChangeState(new CyclopsWalkingState(data));
        }
    }

    protected bool CheckLineOfSight()
    {
        if (player)
        {
            Vector3 playerPos = player.transform.position + Vector3.up;
            Vector3 direction = playerPos - data.eye.position;
            if (Vector3.Dot(data.eye.forward, direction) > 1 - data.visionAngle / 90)
            {
                RaycastHit hit;
                if (!Physics.Raycast(data.eye.position, direction, out hit, direction.magnitude, data.VisionMask))
                    return true;
            }
        }
        return false;
    }
}
