using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy, EnemyStateMachine fsm) : base(enemy, fsm) { }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void CheckToChangeState() 
    {
        base.CheckToChangeState();
        if (enemy.IsZombieUp)
        {
            enemyStateMachine.ChangeState(enemy.BackState);
        }
        else if (!enemy.IsZombieFront && !enemy.IsTowerFront)
        {
            enemyStateMachine.ChangeState(enemy.RunState);
        }
    }
}
