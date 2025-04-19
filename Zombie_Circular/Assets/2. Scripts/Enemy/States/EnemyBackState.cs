using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBackState : EnemyState
{
    private float m_moveSpeed = 1f;
    public EnemyBackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        m_moveSpeed = enemy.BackOffVelocity;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (enemy.IsZombieUp) { }
        else if (enemy.IsZombieFront)
        {
            if (enemy.IsZombieBack)
                enemyStateMachine.ChangeState(enemy.GroupState);
            else
                enemyStateMachine.ChangeState(enemy.JumpState);
        }
        else if(!enemy.IsZombieFront)
        {
            enemyStateMachine.ChangeState(enemy.RunState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemy.RB.velocity = new Vector2(m_moveSpeed, enemy.RB.velocity.y);
    }
}
