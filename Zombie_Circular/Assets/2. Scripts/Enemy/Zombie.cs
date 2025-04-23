using UnityEngine;
using UnityEngine.Rendering;
/// <summary>
/// 데미지 수용  - Tag : HurtingEnemy로 데미지를 받고
/// 데미지 주입  - Tag : Truck으로 데미지를 주고
/// 행동 결정   - Finite State 패턴으로 행동 결정
/// 주변 감지   - Capsul Collision으로 주변 감지 (현재 Layer Only)
/// TODO : 위 사항 책임 분리 필요, State Machine 시스템
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(Animator))]
public class Zombie : Pawn
{
    public enum ZombieState
    {
        Stop,
        IdleRun,
        Back,
        Attack,
        Jump, 
        Die
    }

    [field : Header("Physics")]
    [field: SerializeField] public float RunSpeed { get; set; } = 1f;
    [field: SerializeField] public float JumpForce { get; set; } = 1f;

    [Header("Attack")]
    [SerializeField] private Collider2D attackColliderHeadPivot;
    public float MyDamage { get; set; } = 10f;

    [Header("Layer")]
    [SerializeField] private LayerMask firstZombieLayer;

    //[Header("Debugging")]
    //[SerializeField] private TextMeshProUGUI text;

    private Vector2 m_size;
    private Collider2D m_myCollider;
    private Collider2D[] m_buffer = new Collider2D[8];
    private Rigidbody2D m_rigidBody;
    private LayerMask m_enemyLayerMask;
    private ZombieState m_zombieState;
    private Animator m_animator;
    private Rigidbody2D m_frontZombieRB;

    private bool hasUp, hasDown, hasLeft, hasRight;
    private bool jumpScheduled = false;

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        m_rigidBody = GetComponent<Rigidbody2D>();
        m_rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        m_myCollider = GetComponent<Collider2D>();
        m_size = m_myCollider.bounds.size;
        m_animator = GetComponent<Animator>();
        m_enemyLayerMask = 1 << gameObject.layer;
    }

    private void Start()
    {
        GetComponent<SortingGroup>().sortingOrder = (int)firstZombieLayer  - gameObject.layer;
    }

    private void OnEnable()
    {
        TransitionTo(ZombieState.IdleRun);
    }

    //private void Update()
    //{
    //    text.text = m_zombieState.ToString();
    //}

    private void FixedUpdate()
    {
        Detect();
        RunState();
    }

    #region Detect
    private void Detect()
    {
        ResetDetectionFlags();
        DetectSurroundings();
    }
    private void ResetDetectionFlags()
    {
        hasUp = hasDown = hasLeft = hasRight = false;
    }

    private void DetectSurroundings()
    {
        Vector2 center = m_myCollider.bounds.center;
        int count = Physics2D.OverlapCapsuleNonAlloc(
            center,
            m_size,
            CapsuleDirection2D.Vertical,
            0f,
            m_buffer,
            m_enemyLayerMask
        );

        Collider2D hit = m_myCollider;
        for (int i = 0; i < count; i++)
        {
            hit = m_buffer[i];
            if (hit == m_myCollider) continue;

            Vector2 dir = ((Vector2)hit.bounds.center - center).normalized;
            if (Vector2.Dot(dir, Vector2.up) > 0.5f) { hasUp = true; m_frontZombieRB = hit.GetComponent<Rigidbody2D>(); }
            if (hit.gameObject.CompareTag("Zombie") && Vector2.Dot(dir, Vector2.down) > 0.5f) hasDown = true;
            if (Vector2.Dot(dir, Vector2.left) > 0.7f) hasLeft = true;
            if (Vector2.Dot(dir, Vector2.right) > 0.7f) hasRight = true;
        }

        // Debug.Log($"{this.name}, hasup : {hasUp} left : {hasLeft}, right : {hasRight} down : {hasDown}");
    }
    #endregion

    #region States
    private void RunState()
    {
        switch (m_zombieState)
        {
            case ZombieState.IdleRun: stateIdleRun(); break;
            case ZombieState.Stop: stateStop(); break;
            case ZombieState.Back: stateBack(); break;
            case ZombieState.Attack: stateAttack(); break;
            case ZombieState.Jump: stateJump(); break;
        }
    }

    private void stateIdleRun() // !hasLeft
    {
        
        m_rigidBody.velocity = new Vector2(-RunSpeed, m_rigidBody.velocity.y);

        if (hasUp)
        {
            TransitionTo(ZombieState.Back);
        }
        else if (hasLeft)
        {
            if (hasRight)
                TransitionTo(ZombieState.Stop);
            else
                TransitionTo(ZombieState.Jump);
        }
    }
    private void stateStop() // hasLeft, hasRight
    {
        // 앞 좀비의 속도 × 시간 = 이동 거리
        if (m_frontZombieRB != null)
        {
            Vector2 pushDelta = m_frontZombieRB.velocity * Time.fixedDeltaTime;
            transform.Translate(pushDelta);
        }
        
        if (hasLeft && !hasRight)
            TransitionTo(ZombieState.Jump);
        else if (!hasLeft)
            TransitionTo(ZombieState.IdleRun);
    }

    private void stateBack() // hasUp
    {
        m_rigidBody.velocity = new Vector2(RunSpeed, m_rigidBody.velocity.y);

        if (!hasUp)
        {
            TransitionTo(ZombieState.IdleRun);
        }
    }

    private void stateJump() // hasLeft, !hasRight
    {
        if (hasUp)
            TransitionTo(ZombieState.Back);
        else
            TransitionTo(ZombieState.IdleRun);
    }

        private void jump()
        {
            m_rigidBody.velocity = new Vector2(-RunSpeed, JumpForce);
            jumpScheduled = false;
        }

    private void stateAttack()
    {
        m_rigidBody.velocity = new Vector2(0, m_rigidBody.velocity.y);
        if (hasUp && !hasDown)
            TransitionTo(ZombieState.Back);
    }
    #endregion

    #region Tower, Bullet, Damage, Die
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Truck"))
        {
            if (m_zombieState != ZombieState.Back)
            {
                TransitionTo(ZombieState.Attack);
            }
            
        }
        else if (collision.CompareTag("HurtingEnemy"))
        {
            float damage = collision.gameObject.GetComponent<Bullet>().BulletDamage;
            Damage(damage);
        }
    }

    
    //  Zombie Attack Animation Events
    public void OnAttack() { attackColliderHeadPivot.enabled = true; }
    public void OffAttack() { attackColliderHeadPivot.enabled = false; }

    protected override void OnDamage(float damageAmount)
    {
        base.OnDamage(damageAmount);

        DamagePopupSpawner.Instance.CreatePopup(transform.position, damageAmount.ToString());
    }

    protected override void OnDie()
    {
        TransitionTo(ZombieState.Die);
        ObjectPoolManager.ReturnObjectPool(gameObject);
    }
    #endregion

    #region State Helpers : Transition, Animation
    private void TransitionTo(ZombieState newState)
    {
        if (m_zombieState == newState) return;

        m_zombieState = newState;
        
        if (newState == ZombieState.Jump && !jumpScheduled)
        {
            jumpScheduled = true;
            Invoke(nameof(jump), 0.5f);
        }

        SetAnimationState(newState);
    }

    private void SetAnimationState(ZombieState state)
    {
        //Debug.Log($"{state} 상태");
        m_animator.SetBool("IsAttacking", state == ZombieState.Attack);
        m_animator.SetBool("IsIdle", state == ZombieState.IdleRun || state == ZombieState.Back || state == ZombieState.Jump || state == ZombieState.Stop);
        m_animator.SetBool("IsDead", state == ZombieState.Die);
    }
    #endregion
}
