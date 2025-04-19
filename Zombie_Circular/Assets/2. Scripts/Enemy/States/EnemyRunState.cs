using UnityEngine;

public class EnemyRunState : EnemyState
{
    private float m_moveSpeed = 1f;
    public EnemyRunState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        m_moveSpeed = enemy.RunVelocity;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (enemy.IsZombieUp)
        {
            enemyStateMachine.ChangeState(enemy.BackState);
        }
        else if (enemy.IsTowerFront)
        {
            enemyStateMachine.ChangeState(enemy.AttackState);
        }
        else if (enemy.IsZombieFront)
        {
            if (enemy.IsZombieBack)
                enemyStateMachine.ChangeState(enemy.GroupState);
            else
                enemyStateMachine.ChangeState(enemy.JumpState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // 왼쪽으로 이동
        enemy.RB.velocity = new Vector2(-m_moveSpeed, enemy.RB.velocity.y);
    }
}