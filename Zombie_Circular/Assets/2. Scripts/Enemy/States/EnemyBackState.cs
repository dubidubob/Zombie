using UnityEngine;

public class EnemyBackState : EnemyState
{
    private float m_moveSpeed = 1f;

    private float m_backOffTimer = 0f;
    private float m_backOffDuration = 2f;
    private bool m_timeExpired = false;

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
        m_backOffDuration = enemy.BackOffDuration;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void CheckToChangeState()
    {
        base.CheckToChangeState();
        if (m_timeExpired) return;

        if (enemy.IsZombieFront)
        {
            if (enemy.IsZombieBack)
                enemyStateMachine.ChangeState(enemy.GroupState);
            else
                enemyStateMachine.ChangeState(enemy.JumpState);
        }
        else if (!enemy.IsZombieFront)
        {
            enemyStateMachine.ChangeState(enemy.RunState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        m_backOffTimer += Time.fixedDeltaTime;
        if (m_backOffTimer >= m_backOffDuration && !m_timeExpired)
        {
            m_timeExpired = true;
        }
        enemy.RB.velocity = new Vector2(m_moveSpeed, enemy.RB.velocity.y);
    }
}
