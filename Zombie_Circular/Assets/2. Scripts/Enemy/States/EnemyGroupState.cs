using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyGroupState : EnemyState
{
    private Transform m_frontTarget;
    private Vector3 m_lastTargetPosition;
    private bool m_isFollowing = false;
    public EnemyGroupState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        SetTarget(enemy.TargetTransform);
    }

    public override void ExitState()
    {
        base.ExitState();
        StopFollowing();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (!enemy.IsZombieFront)
            enemyStateMachine.ChangeState(enemy.RunState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (m_isFollowing)
        {
            if (m_frontTarget == null || !m_frontTarget.gameObject.activeInHierarchy)
            {
                enemyStateMachine.ChangeState(enemy.RunState);
            }
            else
            {
                Vector3 currentPos = m_frontTarget.position;
                Vector3 delta = currentPos - m_lastTargetPosition;
                enemy.RB.MovePosition(enemy.RB.position + new Vector2(delta.x, delta.y));
                m_lastTargetPosition = currentPos;
            }
        }
    }
    private void SetTarget(Transform target)
    {
        Debug.Log($"{enemy.name}, {target.name}에 붙었더요");
        m_frontTarget = target;
        m_lastTargetPosition = m_frontTarget.position;
        m_isFollowing = true;
    }

    public void StopFollowing()
    {
        Debug.Log($"{enemy.name}, 떨어졌더요");
        m_isFollowing = false;
        m_frontTarget = null;
    }
}