using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, ITriggerCheckable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; }
    #region Physics
    public Rigidbody2D RB { get; set; }
    public Transform TargetTransform { get; set; }
    [field: SerializeField] public float RunVelocity { get; set; } = 1f;
    [field: SerializeField] public float JumpForce { get; set; } = 1f;
    [field: SerializeField] public float BackOffVelocity { get; set; } = 1f;
    [field: SerializeField] public float BackOffDuration { get; set; } = 0.5f;
    #endregion
    #region State Machine Variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyGroupState GroupState { get; set; }
    public EnemyRunState RunState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    public EnemyJumpState JumpState { get; set; }
    public EnemyBackState BackState { get; set; }
    #endregion
    #region Trigger Check Variables
    public bool IsTowerFront { get; set; }
    public bool IsZombieFront { get; set; }
    public bool IsZombieBack { get; set; }
    public bool IsZombieUp { get; set; }
    #endregion

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        GroupState = new EnemyGroupState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        RunState = new EnemyRunState(this, StateMachine);
        JumpState = new EnemyJumpState(this, StateMachine);
        BackState = new EnemyBackState(this, StateMachine);
    }
    private void Start()
    {
        CurrentHealth = MaxHealth;
        RB = GetComponent<Rigidbody2D>();

        StateMachine.Initialize(RunState);
    }

    private void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();   
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    #region Health / Die
    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }
    public void Die()
    {
        //TODO : Object Pooling
        //Destroy(gameObject);
    }
    #endregion

    #region Move
    public void MoveEnemy(Vector2 velocity)
    {
        RB.velocity = velocity;
    }
    #endregion

    #region Distance Checks
    public void SetTowerFront(bool isTowerFront)
    {
        IsTowerFront = isTowerFront;
        StateMachine.CurrentEnemyState.CheckToChangeState();
    }

    public void SetZombieFront(bool isZombieFront)
    {
        IsZombieFront = isZombieFront;
        StateMachine.CurrentEnemyState.CheckToChangeState();
    }

    public void SetZombieBack(bool isZombieBack)
    {
        IsZombieBack = isZombieBack;
        StateMachine.CurrentEnemyState.CheckToChangeState();
    }

    public void SetZombieUp(bool isZombieUp)
    {
        IsZombieUp = isZombieUp;
        StateMachine.CurrentEnemyState.CheckToChangeState();
    }

    #endregion
    #region Animation Triggers
    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
        //TODO
    }

    public enum AnimationTriggerType
    { 
        FrontWalk,
        BackWalk,
        Attack,
        Idle,
        Die
    }
    #endregion
}