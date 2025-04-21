using System.Collections;
using TMPro;
using UnityEngine;

public class Zombie : Pawn, IEnemyMovable
{
    public enum State
    {
        Stop,
        IdleRun,
        Back,
        Attack,
        Jump
    }
    [Header("Physics")]
    [SerializeField] private float m_delay = 0.5f;
    [field: SerializeField] public float RunSpeed { get; set; } = 1f;
    [field: SerializeField] public float JumpForce { get; set; } = 1f;
    [Header("Detect")]
    [SerializeField] private LayerMask enemyLayerMask;
    [Header("Debuggine")]
    [SerializeField] private TextMeshProUGUI text;
    private Vector2 m_size;
    private Collider2D m_myCollider;
    private Collider2D[] m_buffer = new Collider2D[8];
    private Rigidbody2D m_rigidBody;
    private State m_zombieState;

    private bool hasUp, hasDown, hasLeft, hasRight;
    private bool isJumping=false;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_myCollider = GetComponent<Collider2D>();
        m_size = m_myCollider.bounds.size;
    }

    private void OnEnable()
    {
        m_zombieState = State.IdleRun;
    }

    private void Update()
    {
        text.text = m_zombieState.ToString();
    }
    private void FixedUpdate()
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
        hasUp  = hasDown = hasLeft = hasRight = false;
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
            enemyLayerMask
        );

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = m_buffer[i];
            if (hit == m_myCollider) continue;

            Vector2 dir = ((Vector2)hit.bounds.center - center).normalized;
            if (Vector2.Dot(dir, Vector2.up) > 0.3f) hasUp = true;
            if (Vector2.Dot(dir, Vector2.down) > 0.3f) hasDown = true;
            if (Vector2.Dot(dir, Vector2.left) > 0.7f) hasLeft = true;
            if (Vector2.Dot(dir, Vector2.right) > 0.7f) hasRight = true;
        }

        // 이제 hasUp, hasDown, hasLeft, hasRight를 이용해 로직 처리
        Debug.Log($"{this.name} hasUp : {hasUp} left : {hasLeft}, right : {hasRight}");
    }

    private void stateIdleRun() // !hasLeft
    {
        m_rigidBody.velocity = new Vector2(-RunSpeed, m_rigidBody.velocity.y);

        if (hasUp)
        {
            m_zombieState = State.Back;
        }
        else if (hasLeft)
        {
            m_zombieState = hasRight ? State.Stop : State.Jump;
        }
    }
    private void stateStop() // hasLeft, hasRight
    {
        m_rigidBody.velocity = new Vector2(0, m_rigidBody.velocity.y);

        if (hasLeft && !hasRight)
            m_zombieState = State.Jump;
        else if (!hasLeft)
            m_zombieState = State.IdleRun;
    }

    private void stateBack() // hasUp
    {
        m_rigidBody.velocity = new Vector2(RunSpeed, m_rigidBody.velocity.y);

        if (!hasUp)
        {
            m_zombieState = State.IdleRun;
        }
    }

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
                m_zombieState = State.Stop;
            }
        }
    }

    private void stateAttack()
    {
        m_rigidBody.velocity = new Vector2(0, m_rigidBody.velocity.y);
        if (hasUp)
            m_zombieState = State.Back;
    }

    #region Tower
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
            if(m_zombieState!=State.Back)
                m_zombieState = State.Attack;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
            if (m_zombieState != State.Back)
                m_zombieState = State.IdleRun;
    }
    #endregion
}
