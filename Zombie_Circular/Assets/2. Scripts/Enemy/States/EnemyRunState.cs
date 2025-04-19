using UnityEngine;

public class EnemyRunState : EnemyState
{
    private float moveSpeed = 1f;
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
        moveSpeed = enemy.RunVelocity;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (enemy.IsTowerFront)
        {
            enemyStateMachine.ChangeState(enemy.AttackState);
        }
        else if (enemy.IsZombieFront)
        { 
            enemyStateMachine.ChangeState(enemy.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // 왼쪽으로 이동
        enemy.RB.velocity = new Vector2(-moveSpeed, enemy.RB.velocity.y);
    }
}