using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(Animator))]
public class Zombie : Pawn, IEnemyMovable
{
    public enum State
    {
        Stop,
        IdleRun,
        Back,
        Attack,
        Jump, 
        Die
    }

    private enum AnimState
    { 
        Run,
        Attack,
        Dead
    }

    [field : Header("Physics")]
    [field: SerializeField] public float RunSpeed { get; set; } = 1f;
    [field: SerializeField] public float JumpForce { get; set; } = 1f;

    [Header("Attack")]
    [SerializeField] private Collider2D attackColliderHeadPivot;
    [SerializeField] public float MyDamage { get; set; } = 10f;

    [Header("Debugging")]
    [SerializeField] private TextMeshProUGUI text;

    private Vector2 m_size;
    private Collider2D m_myCollider;
    private Collider2D[] m_buffer = new Collider2D[8];
    private Rigidbody2D m_rigidBody;
    private LayerMask m_enemyLayerMask;
    private State m_zombieState;
    private Animator m_animator;
    private Rigidbody2D m_frontZombieRB;

    private bool hasUp, hasDown, hasLeft, hasRight;
    private bool isJumping = false;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        m_myCollider = GetComponent<Collider2D>();
        m_size = m_myCollider.bounds.size;
        m_animator = GetComponent<Animator>();
        m_enemyLayerMask = 1 << gameObject.layer;
    }

    private void OnEnable()
    {
        TransitionTo(State.IdleRun);
    }

    private void Update()
    {
        text.text = m_zombieState.ToString();
    }

    private void FixedUpdate()
    {
        Detect();
    }

    protected override void OnDie()
    {
        TransitionTo(State.Die);
        //StartCoroutine(DeactivateAfterDelay(0.5f));
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPoolManager.ReturnObjectPool(gameObject);
    }

    #region Detect
    private void Detect()
    {
        ResetDetectionFlags();
        DetectSurroundings();

        switch (m_zombieState)
        {
            case State.IdleRun: stateIdleRun(); break;
            case State.Stop: stateStop(); break;
            case State.Back: stateBack(); break;
            case State.Attack: stateAttack(); break;
            case State.Jump: stateJump(); break;
        }
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
            if (Vector2.Dot(dir, Vector2.up) > 0.3f) { hasUp = true; m_frontZombieRB = hit.GetComponent<Rigidbody2D>(); }
            if (hit.gameObject.CompareTag("Zombie") && Vector2.Dot(dir, Vector2.down) > 0.3f) hasDown = true;
            if (Vector2.Dot(dir, Vector2.left) > 0.7f) hasLeft = true;
            if (Vector2.Dot(dir, Vector2.right) > 0.7f) hasRight = true;
        }

        // ���� hasUp, hasDown, hasLeft, hasRight�� �̿��� ���� ó��
        if (m_frontZombieRB == null)
        {
            Debug.Log($"{this.name}, hasUp : {hasUp} left : {hasLeft}, right : {hasRight}");
        }
        else
        {
            Debug.Log($"{this.name}, {m_frontZombieRB.name} hasUp : {hasUp} left : {hasLeft}, right : {hasRight}");
        }
    }
    #endregion

    #region States
    private void stateIdleRun() // !hasLeft
    {
        
        m_rigidBody.velocity = new Vector2(-RunSpeed, m_rigidBody.velocity.y);

        if (hasUp)
        {
            TransitionTo(State.Back);
        }
        else if (hasLeft)
        {
            if (hasRight)
                TransitionTo(State.Stop);
            else
                TransitionTo(State.Jump);
        }
    }
    private void stateStop() // hasLeft, hasRight
    {
        // �� ������ �ӵ� �� �ð� = �̵� �Ÿ�
        if (m_frontZombieRB != null)
        {
            Vector2 pushDelta = m_frontZombieRB.velocity * Time.fixedDeltaTime;
            transform.Translate(pushDelta);
        }
        
        if (hasLeft && !hasRight)
            TransitionTo(State.Jump);
        else if (!hasLeft)
            TransitionTo(State.IdleRun);
    }

    private void stateBack() // hasUp
    {
        m_rigidBody.velocity = new Vector2(RunSpeed, m_rigidBody.velocity.y);

        if (!hasUp)
        {
            TransitionTo(State.IdleRun);
        }
    }

    // TODO 
    private void stateJump() // hasLeft, !hasRight
    {
        Invoke("jump", 0.5f);

        if (hasUp)
            TransitionTo(State.Back);
    }

    private void jump()
    {
        m_rigidBody.velocity = new Vector2(-RunSpeed, JumpForce);
    }

    private void stateAttack()
    {
        m_rigidBody.velocity = new Vector2(0, m_rigidBody.velocity.y);
        if (hasUp)
            TransitionTo(State.Back);
    }
    #endregion

    #region Tower, Bullet
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            if (m_zombieState != State.Back)
                TransitionTo(State.Attack);
        }
        else if (collision.CompareTag("Damageble"))
        {
            float damage = collision.gameObject.GetComponent<Bullet>().BulletDamage;
            Damage(damage);
        }
    }

    /// <summary>
    ///  Zombie Attack Animation Events
    /// </summary>
    public void OnAttack() { attackColliderHeadPivot.enabled = true; }
    public void OffAttack() { attackColliderHeadPivot.enabled = false; }

    protected override void OnDamage(float damageAmount)
    {
        base.OnDamage(damageAmount);

        DamagePopupGenerator.Instance.CreatePopup(transform.position, damageAmount.ToString());
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
            if (m_zombieState != State.Back)
                TransitionTo(State.IdleRun);
    }
    #endregion

    #region State Helpers : Transition, Animation
    private void TransitionTo(State newState)
    {
        if (m_zombieState == newState) return;

        m_zombieState = newState;
        SetAnimationState(newState);
    }

    private void SetAnimationState(State state)
    {
        //Debug.Log($"{state} ����");
        m_animator.SetBool("IsAttacking", state == State.Attack);
        m_animator.SetBool("IsIdle", state == State.IdleRun || state == State.Back || state == State.Jump || state == State.Stop);
        m_animator.SetBool("IsDead", state == State.Die);
    }
    #endregion
}
