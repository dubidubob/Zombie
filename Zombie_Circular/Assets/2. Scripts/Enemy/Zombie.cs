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

    //[Header("Physics")]
    //[SerializeField] private float m_delay = 0.5f;
    [field: SerializeField] public float RunSpeed { get; set; } = 1f;
    [field: SerializeField] public float JumpForce { get; set; } = 1f;
    [SerializeField] public float MyDamage { get; set; } = 10f;

    [Header("Attack")]
    [SerializeField] private Collider2D attackColliderHeadPivot;
    
    [Header("Debugging")]
    [SerializeField] private TextMeshProUGUI text;

    private Vector2 m_size;
    private Collider2D m_myCollider;
    private Collider2D[] m_buffer = new Collider2D[8];
    private Rigidbody2D m_rigidBody;
    private LayerMask m_enemyLayerMask;
    private State m_zombieState;
    private Animator m_animator;

    private bool hasUp, hasDown, hasLeft, hasRight;
    private bool isJumping = false;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        m_myCollider = GetComponent<Collider2D>();
        m_size = m_myCollider.bounds.size;
        m_animator = GetComponent<Animator>();
        m_enemyLayerMask = gameObject.layer;
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
        StartCoroutine(DeactivateAfterDelay(0.5f));
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
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

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = m_buffer[i];
            if (hit == m_myCollider) continue;

            Vector2 dir = ((Vector2)hit.bounds.center - center).normalized;
            if (Vector2.Dot(dir, Vector2.up) > 0.3f) hasUp = true;
            if (hit.gameObject.CompareTag("Zombie") && Vector2.Dot(dir, Vector2.down) > 0.3f) hasDown = true;
            if (Vector2.Dot(dir, Vector2.left) > 0.7f) hasLeft = true;
            if (Vector2.Dot(dir, Vector2.right) > 0.7f) hasRight = true;
        }

        // 이제 hasUp, hasDown, hasLeft, hasRight를 이용해 로직 처리
        // Debug.Log($"{this.name} hasUp : {hasUp} left : {hasLeft}, right : {hasRight}");
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
        m_rigidBody.velocity = new Vector2(0, m_rigidBody.velocity.y);

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
        if (m_rigidBody.velocity.x > 0 && !hasDown)
        {
            m_rigidBody.velocity = new Vector2(0, m_rigidBody.velocity.y);
        }
        else
        {
            if (!isJumping)
            {
                isJumping = true;
                m_rigidBody.velocity = new Vector2(-RunSpeed, JumpForce);
            }
            m_rigidBody.velocity = new Vector2(-RunSpeed, m_rigidBody.velocity.y);

            if (m_rigidBody.velocity.y == 0)
            {
                isJumping = false;
                TransitionTo(State.Stop);
            }
        }
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

    public void OnAttack()
    {
        attackColliderHeadPivot.enabled = true;
    }

    public void OffAttack()
    {
        attackColliderHeadPivot.enabled = false;
    }

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

    private void TransitionTo(State newState)
    {
        if (m_zombieState == newState) return;

        m_zombieState = newState;
        SetAnimationState(newState);
    }

    private void SetAnimationState(State state)
    {
        //Debug.Log($"{state} 상태");
        m_animator.SetBool("IsAttacking", state == State.Attack);
        m_animator.SetBool("IsIdle", state == State.IdleRun || state == State.Back || state == State.Jump || state == State.Stop);
        m_animator.SetBool("IsDead", state == State.Die);
    }
}
