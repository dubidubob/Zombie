using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static System.Net.WebRequestMethods;

[RequireComponent(typeof(Rigidbody2D), typeof(DistanceJoint2D), typeof(Collider2D))]
public class TestZombieFSM : MonoBehaviour
{ }
//    [Header("Physics")]
//    [SerializeField] private float moveSpeed = 1f;
//    [SerializeField] private float jumpSpeed = 1f;

//    public enum State
//    {
//        Stop,
//        IdleRun,
//        Back,
//        Attack,
//        Jump
//    }

//    [Header("Detect")]
//    [SerializeField] private float RayDist = 0.1f;
//    [SerializeField] private float m_dropWaitingTime = 0.5f;
//    [SerializeField] private LayerMask enemyRayLayerMask;
//    [SerializeField] private LayerMask allayRayLayerMask;

//    [SerializeField] private TextMeshProUGUI stateText;

//    private Rigidbody2D m_rigidBody;
//    private Collider2D m_myCollider;
//    private Vector2 m_center;

//    bool isTowerFront;
//    bool isZombieFront;
//    bool isZombieBack;
//    bool isZombieUp;
//    bool isZombieDown;

//    private State zombieState;
//    private DistanceJoint2D distanceJoint;
//    private bool isDropingDone = true;

//    private void Awake()
//    {
//        Physics2D.queriesStartInColliders = false;
//        m_rigidBody = GetComponent<Rigidbody2D>();
//        m_myCollider = GetComponent<Collider2D>();
//        distanceJoint = GetComponent<DistanceJoint2D>();

//        distanceJoint.enabled = false;
//        distanceJoint.autoConfigureDistance = false;
//        distanceJoint.maxDistanceOnly = true;

//        zombieState = State.IdleRun;
//    }

//    private void Update()
//    {
//        stateText.text = zombieState.ToString();
//    }

//    private void FixedUpdate()
//    {
//        m_center = m_myCollider.bounds.center;

//    }

//    private bool CheckExceptMyCollider(RaycastHit2D[] hits)
//    {
//        foreach (var hit in hits)
//        {
//            if (hit.collider == m_myCollider) continue;
//            return true;
//        }
//        return false;
//    }
//    private void CheckStates()
//    {
//        switch (zombieState)
//        {
//            case State.IdleRun:
//                m_rigidBody.velocity = new Vector2(-moveSpeed, m_rigidBody.velocity.y);
//                stateIdleRun();
//                break;

//            case State.Stop:
//                m_rigidBody.velocity = new Vector2(0, m_rigidBody.velocity.y);
//                stateStop();
//                break;

//            case State.Jump:
//                m_rigidBody.velocity = new Vector2(-moveSpeed, m_rigidBody.velocity.y);
//                stateJump();
//                break;

//            case State.Attack:
//                m_rigidBody.velocity = new Vector2(0, m_rigidBody.velocity.y);
//                stateAttack();
//                break;

//            case State.Back:
//                m_rigidBody.velocity = new Vector2(moveSpeed, m_rigidBody.velocity.y);
//                stateBack();
//                break;
//        }
//    }
//    private void Shoot4WayRaysToCheck()
//    {
//        RaycastHit2D[] hitHero = Physics2D.RaycastAll(m_center, Vector2.left, RayDist, allayRayLayerMask);
//        Debug.DrawRay(m_center, Vector2.left * RayDist, Color.red);
//        RaycastHit2D[] hitFront = Physics2D.RaycastAll(m_center, Vector2.left, RayDist, enemyRayLayerMask);
//        Debug.DrawRay(m_center, Vector2.left * RayDist, Color.red);
//        RaycastHit2D[] hitUp = Physics2D.RaycastAll(m_center, Vector2.up, RayDist, enemyRayLayerMask);
//        Debug.DrawRay(m_center, Vector2.left * RayDist, Color.red);
//        RaycastHit2D[] hitDown = Physics2D.RaycastAll(m_center, Vector2.down, RayDist, enemyRayLayerMask);
//        Debug.DrawRay(m_center, Vector2.left * RayDist, Color.red);
//        RaycastHit2D[] hitBack = Physics2D.RaycastAll(m_center, Vector2.right, RayDist, enemyRayLayerMask);
//        Debug.DrawRay(m_center, Vector2.left * RayDist, Color.red);

//        isTowerFront = CheckExceptMyCollider(hitHero);
//        isZombieFront = CheckExceptMyCollider(hitFront);
//        isZombieBack = CheckExceptMyCollider(hitBack);
//        isZombieUp = CheckExceptMyCollider(hitUp);
//        isZombieDown = CheckExceptMyCollider(hitDown);
//    }

//    private void stateBack()
//    {
//        앞 좀비 떨어지는 시간 기다리기
//        if (!isDropingDone)
//            return;

//        if (isZombieUp) { return; }
//        else if (isZombieFront && isZombieBack)
//        {
//            zombieState = State.Stop;
//        }
//        else if (isZombieFront && !isZombieBack)
//        {
//            zombieState = State.Jump;
//        }


//        else
//        {
//            isDropingDone = false;
//            StartCoroutine(DelayedAction(m_dropWaitingTime, () =>
//            {
//                isDropingDone = true;
//                if (isZombieFront)
//                    zombieState = State.IdleRun;
//            }));
//        }
//    }

//    private void stateIdleRun()
//    {
//        if (hitBack.collider == null && hitFront.collider != null && hitFront.collider.CompareTag("Monster"))
//        {
//            m_rigidBody.velocity = new Vector2(-moveSpeed, jumpSpeed);
//            zombieState = State.Jump;
//        }
//        else if (hitBack.collider != null && hitBack.collider.CompareTag("Monster"))
//        {
//            zombieState = State.Stop;
//        }
//        else if (hitFront.collider != null && hitFront.collider.CompareTag("Tower"))
//        {
//            zombieState = State.Attack;
//        }
//    }

//    private void stateStop()
//    {
//        Vector2 rayFront = new Vector2(bounds.min.x, bounds.max.y);
//        RaycastHit2D hitFront = Physics2D.Raycast(rayFront, Vector2.left, FrontDist);
//        Debug.DrawRay(rayFront, Vector2.left * FrontDist, Color.red);

//        Vector2 rayBack = new Vector2(bounds.max.x, bounds.max.y);
//        RaycastHit2D hitBack = Physics2D.Raycast(rayBack, Vector2.right, BackDist);
//        Debug.DrawRay(rayBack, Vector2.right * BackDist, Color.blue);

//        if (hitFront.collider == null)
//        {
//            if (distanceJoint.enabled)
//                distanceJoint.enabled = false;

//            앞이 비었으면 Stop 상태 해제
//           zombieState = State.IdleRun;
//        }
//        else if (hitFront.collider.CompareTag("Monster"))
//        {
//            if (hitBack.collider == null)
//            {
//                if (distanceJoint.enabled)
//                    distanceJoint.enabled = false;

//                m_rigidBody.velocity = new Vector2(-moveSpeed, jumpSpeed);
//                zombieState = State.Jump;
//            }
//            else if (hitBack.collider.CompareTag("Monster"))
//            {
//                if (!distanceJoint.enabled)
//                {
//                    Rigidbody2D otherRb = hitFront.collider.GetComponent<Rigidbody2D>();
//                    distanceJoint.connectedBody = otherRb;

//                    Vector2 worldAnchor = transform.TransformPoint(distanceJoint.anchor);
//                    Vector2 otherWorldPoint = otherRb.transform.TransformPoint(distanceJoint.connectedAnchor);
//                    float initialDistance = Vector2.Distance(worldAnchor, otherWorldPoint);

//                    distanceJoint.distance = initialDistance;
//                    distanceJoint.enabled = true;
//                }
//            }
//        }
//    }

//    private void stateJump()
//    {


//        if (hitGround.collider != null && hitGround.collider.gameObject.CompareTag("Tower"))
//        {
//            zombieState = State.Attack;
//        }
//        else if (hitGround.collider != null && m_rigidBody.velocity.y <= 0f)
//        {
//            zombieState = State.IdleRun;
//        }
//    }

//    private void stateAttack()
//    {
//        Vector2 rayUpPos = new Vector2(bounds.center.x, bounds.max.y);
//        RaycastHit2D hitUp = Physics2D.Raycast(rayUpPos, Vector2.up, UpDist);
//        Debug.DrawRay(rayUpPos, Vector2.up * UpDist, Color.black);

//        if (hitUp.collider != null && hitUp.collider.CompareTag("Monster"))
//        {
//            zombieState = State.Back;
//        }
//    }



//    private IEnumerator DelayedAction(float delay, Action action)
//    {
//        yield return new WaitForSeconds(delay);
//        action();
//    }
//}
