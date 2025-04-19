using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpState : EnemyState
{
    private float m_jumpForce =1f;
    public EnemyJumpState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        m_jumpForce = enemy.JumpForce;
        Vector2 impulse = Vector2.up * m_jumpForce;
        enemy.RB.AddForce(impulse, ForceMode2D.Impulse);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (enemy.IsZombieFront)
        {
            if (enemy.IsZombieBack)
                enemyStateMachine.ChangeState(enemy.GroupState);
            else
                enemyStateMachine.ChangeState(enemy.JumpState);
        }
        else
        {
            enemyStateMachine.ChangeState(enemy.RunState);
        }
    }
}