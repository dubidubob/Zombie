using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(DistanceJoint2D), typeof(Collider2D))]
public class TestZombieFSM : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpSpeed = 1f;

    public enum State
    {
        Stop,
        IdleRun,
        Back,
        Attack,
        Jump
    }

    [Header("Detect")]
    [SerializeField] private float UpDist = 0.1f;
    [SerializeField] private float DownDist = 0.1f;
    [SerializeField] private float FrontDist = 0.1f;
    [SerializeField] private float BackDist = 0.1f;
    [SerializeField] private float m_dropWaitingTime = 0.5f;

    [SerializeField] private TextMeshProUGUI stateText;

    private Rigidbody2D rb;
    private Collider2D myCollider2D;
    private Bounds bounds;
    private State zombieState;
    private DistanceJoint2D distanceJoint;
    private bool isDropingDone = true;

    private void Awake()
    {
        Physics2D.queriesStartInColliders = false;
        rb = GetComponent<Rigidbody2D>();
        myCollider2D = GetComponent<Collider2D>();
        distanceJoint = GetComponent<DistanceJoint2D>();

        distanceJoint.enabled = false;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.maxDistanceOnly = true;

        zombieState = State.IdleRun;
    }

    private void Update()
    {
        stateText.text = zombieState.ToString();
    }

    private void FixedUpdate()
    {
        bounds = myCollider2D.bounds;
        switch (zombieState)
        {
            case State.IdleRun:
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                stateIdleRun();
                break;

            case State.Stop:
                rb.velocity = new Vector2(0, rb.velocity.y);
                stateStop();
                break;

            case State.Jump:
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                stateJump();
                break;

            case State.Attack:
                rb.velocity = new Vector2(0, rb.velocity.y);
                stateAttack();
                break;

            case State.Back:
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                stateBack();
                break;
        }
    }

    private void stateIdleRun()
    {
        Vector2 rayFront = new Vector2(bounds.min.x, bounds.max.y);
        RaycastHit2D hitFront = Physics2D.Raycast(rayFront, Vector2.left, FrontDist);
        Debug.DrawRay(rayFront, Vector2.left * FrontDist, Color.black);

        Vector2 rayBack = new Vector2(bounds.max.x, bounds.max.y);
        RaycastHit2D hitBack = Physics2D.Raycast(rayBack, Vector2.right, BackDist);
        Debug.DrawRay(rayBack, Vector2.right * BackDist, Color.black);

        if (hitBack.collider == null && hitFront.collider != null && hitFront.collider.CompareTag("Monster"))
        {
            rb.velocity = new Vector2(-moveSpeed, jumpSpeed);
            zombieState = State.Jump;
        }
        else if (hitBack.collider != null && hitBack.collider.CompareTag("Monster"))
        {
            zombieState = State.Stop;
        }
        else if (hitFront.collider != null && hitFront.collider.CompareTag("Tower"))
        {
            zombieState = State.Attack;
        }
    }

    private void stateStop()
    {
        Vector2 rayFront = new Vector2(bounds.min.x, bounds.max.y);
        RaycastHit2D hitFront = Physics2D.Raycast(rayFront, Vector2.left, FrontDist);
        Debug.DrawRay(rayFront, Vector2.left * FrontDist, Color.red);

        Vector2 rayBack = new Vector2(bounds.max.x, bounds.max.y);
        RaycastHit2D hitBack = Physics2D.Raycast(rayBack, Vector2.right, BackDist);
        Debug.DrawRay(rayBack, Vector2.right * BackDist, Color.blue);

        if (hitFront.collider == null)
        {
            if (distanceJoint.enabled)
                distanceJoint.enabled = false;

            // 앞이 비었으면 Stop 상태 해제
            zombieState = State.IdleRun;
        }
        else if (hitFront.collider.CompareTag("Monster"))
        {
            if (hitBack.collider == null)
            {
                if (distanceJoint.enabled)
                    distanceJoint.enabled = false;

                rb.velocity = new Vector2(-moveSpeed, jumpSpeed);
                zombieState = State.Jump;
            }
            else if (hitBack.collider.CompareTag("Monster"))
            {
                if (!distanceJoint.enabled)
                {
                    Rigidbody2D otherRb = hitFront.collider.GetComponent<Rigidbody2D>();
                    distanceJoint.connectedBody = otherRb;

                    Vector2 worldAnchor = transform.TransformPoint(distanceJoint.anchor);
                    Vector2 otherWorldPoint = otherRb.transform.TransformPoint(distanceJoint.connectedAnchor);
                    float initialDistance = Vector2.Distance(worldAnchor, otherWorldPoint);

                    distanceJoint.distance = initialDistance;
                    distanceJoint.enabled = true;
                }
            }
        }
    }

    private void stateJump()
    {
        Vector2 rayOrigin = new Vector2(bounds.center.x, bounds.max.y);
        RaycastHit2D hitGround = Physics2D.Raycast(rayOrigin, Vector2.down, DownDist);
        Debug.DrawRay(rayOrigin, Vector2.down * DownDist, Color.green);

        Vector2 rayBack = new Vector2(bounds.max.x, bounds.max.y);
        RaycastHit2D hitBack = Physics2D.Raycast(rayBack, Vector2.right, BackDist);
        Debug.DrawRay(rayBack, Vector2.right * BackDist, Color.black);

        if (hitGround.collider != null && hitGround.collider.gameObject.CompareTag("Tower"))
        {
            zombieState = State.Attack;
        }
        else if (hitGround.collider != null && rb.velocity.y <= 0f)
        {
            zombieState = State.IdleRun;
        }
    }

    private void stateAttack()
    {
        Vector2 rayUpPos = new Vector2(bounds.center.x, bounds.max.y);
        RaycastHit2D hitUp = Physics2D.Raycast(rayUpPos, Vector2.up, UpDist);
        Debug.DrawRay(rayUpPos, Vector2.up * UpDist, Color.black);

        if (hitUp.collider != null && hitUp.collider.CompareTag("Monster"))
        {
            zombieState = State.Back;
        }
    }

    private void stateBack()
    {
        if (!isDropingDone)
            return;

        Vector2 rayUpPos = new Vector2(bounds.min.x, bounds.max.y);
        Vector2 rayFrontPos = new Vector2(bounds.min.x, bounds.min.y);

        RaycastHit2D hitUp = Physics2D.Raycast(rayUpPos, Vector2.up, UpDist);
        RaycastHit2D hitFront = Physics2D.Raycast(rayFrontPos, Vector2.left, UpDist);

        Debug.DrawRay(rayUpPos, Vector2.up * UpDist, Color.black);
        Debug.DrawRay(rayFrontPos, Vector2.left * UpDist, Color.black);

        if (hitUp.collider != null && hitUp.collider.CompareTag("Monster"))
        {
            zombieState = State.Back;
        }
        else if (hitFront.collider != null && hitFront.collider.CompareTag("Monster"))
        {
            zombieState = State.Stop;
        }
        else
        {
            isDropingDone = false;
            StartCoroutine(DelayedAction(m_dropWaitingTime, () =>
            {
                isDropingDone = true;
                if (hitUp.collider == null && hitFront.collider == null)
                    zombieState = State.IdleRun;
            }));
        }
    }

    private IEnumerator DelayedAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}
