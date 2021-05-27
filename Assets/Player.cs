using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 0.1f;
    new public Rigidbody2D rigidbody2D;
    new public CircleCollider2D collider2D;
    public Animator animator;
    public float wallOffset = 0.02f;
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        Application.targetFrameRate = 60;


        RaycastHit2D rightMostHit = new RaycastHit2D();
        RaycastHit2D leftMostHit = new RaycastHit2D();
        Vector2 checkPosion = transform.position;

        // 오른쪽 체크
        int count = 0;
        checkPosion.x += 100f;
        rightMostHit = Physics2D.Raycast(checkPosion
            , Vector2.left, 100f, wallLayer);
        while (count++ < 10)
        {
            checkPosion.x = rightMostHit.point.x - 1.01f;
            var hit = Physics2D.Raycast(checkPosion, Vector2.left, 1f, wallLayer);
            if (hit.transform == null)
                break;
            rightMostHit = hit;
        }
        checkPosion.x -= 1;
        var hit0 = Physics2D.Raycast(checkPosion, Vector2.right, 2f, wallLayer);
        rightMostHit = hit0;
        Debug.Log("최종 우측 벽 위치 " + rightMostHit.point.x);
        maxX = rightMostHit.point.x - collider2D.radius - wallOffset;
        

        // 왼쪽 체크
        count = 0;
        checkPosion.x -= 100f;
        leftMostHit = Physics2D.Raycast(checkPosion
            , Vector2.right, 100f, wallLayer);
        while (count++ < 10)
        {
            checkPosion.x = leftMostHit.point.x + 1.01f;
            var hit = Physics2D.Raycast(checkPosion, Vector2.right, 1f, wallLayer);
            if (hit.transform == null)
                break;
            leftMostHit = hit;
        }
        // 오른쪽으로 1 이동, 다시 왼쪽으로 레이 발사
        checkPosion.x += 1;
        var hit1 = Physics2D.Raycast(checkPosion, Vector2.left, 2f, wallLayer);
        leftMostHit = hit1;
        Debug.Log("최종 좌측 벽 위치 " + leftMostHit.point.x);
        minX = leftMostHit.point.x + collider2D.radius + wallOffset;
        {

            //RaycastHit2D rightmostHit = new RaycastHit2D();
            //RaycastHit2D Leftmost = new RaycastHit2D();
            //RaycastHit2D hit;
            //Vector2 checkPosition = transform.position;
            //int count = 0;
            //List<RaycastHit2D> hits = new List<RaycastHit2D>();
            //float wallWidth = 1.01f;
            //while (count++ < 10) // 최대 10회만 검사.
            //{
            //    hit = Physics2D.Raycast(checkPosition, Vector2.right, 100f, wallLayer);
            //    if (hit.transform == null)
            //        break;
            //    hits.Add(hit);
            //    rightmostHit = hit; // 마지막 벽이 2중 벽일때 바로 앞에 벽을 마지막 오른쪽 벽으로 하자.
            //    checkPosition.x = hit.point.x + wallWidth; //
            //};

            //if (hits.Count >= 2)
            //{
            //    var previousHit = hits[hits.Count - 2];
            //    if (rightmostHit.point.x < previousHit.point.x + wallWidth + 0.01f)
            //    {
            //        rightmostHit = previousHit;
            //    }
            //}

            //count = 0;
            //hits.Clear();
            //checkPosition = transform.position;
            //while (count++ < 10) // 최대 10회만 검사.
            //{
            //    hit = Physics2D.Raycast(checkPosition, Vector2.left, 100f, wallLayer);
            //    if (hit.transform == null)
            //        break;
            //    hits.Add(hit);
            //    Leftmost = hit;
            //    checkPosition.x = hit.point.x - 1.01f; // 1.01은 벽 두께
            //};

            //if (hits.Count >= 2)
            //{
            //    var previousHit = hits[hits.Count - 2];
            //    if (Leftmost.point.x > previousHit.point.x - wallWidth - 0.01f)
            //    {
            //        Leftmost = previousHit;

            //        if (hits.Count >= 3) // 왼쪽은 3중벽이어서 추가 확인
            //        {
            //            previousHit = hits[hits.Count - 3];
            //            if (Leftmost.point.x > previousHit.point.x - wallWidth - 0.01f)
            //            {
            //                Leftmost = previousHit;
            //            }
            //        }
            //    }
            //}

            //float halfSize = collider2D.bounds.size.x * 0.5f + wallOffset;
            //maxX = rightmostHit.point.x - halfSize;
            //minX = Leftmost.point.x + halfSize;
        }
    }
    private void Update()
    {
        // 버블발사
        FireBubble();
        // 움직임
        Move();
        // 점프
        Jump();
        // 아래점프
        DownJump();
    }
    public float minX, maxX;
    private void Move()
    {
        // WASD, W위로, A왼쪽,S아래, D오른쪽
        float moveX = 0;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX = -1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX = 1;



        Vector3 position = transform.position;
        position.x += moveX * speed;
        position.x = Mathf.Max(minX, position.x);
        position.x = Mathf.Min(maxX, position.x);
        transform.position = position;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack") == false)
        {
            if (moveX != 0)
            {
                // moveX양수면 180도 로테이션
                float rotateY = 0;
                if (moveX < 0)
                    rotateY = 180;

                //var rotation = transform.rotation;
                //rotation.y = rotateY;
                //transform.rotation = rotation;

                transform.rotation = new Quaternion(0, rotateY, 0, 0);

                animator.Play("run");
            }
            else
                animator.Play("idle");
        }
    }

    public GameObject bubble;
    public Transform bubbleSpawnPos;
    private void FireBubble()
    {
        // 버블 날리기
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bubble, bubbleSpawnPos.position, transform.rotation);
        }
    }
    public float jumpForce = 1100f;
    private void Jump()
    {
        // 점프할 때 벽을 뚫어야함
        // 낙하시에는 뚫으면 안됨
        // 속도가 음수일 때, 이전 y값과 비교시 -일 때
        if (ingDownJump == false && rigidbody2D.velocity.y < 0)
        {
            collider2D.isTrigger = false;
        }

        // 다중 점프 막아야함
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            bool isGround = IsGround();
            if (isGround)
            {
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.AddForce(new Vector2(0, jumpForce));
                collider2D.isTrigger = true;
            }
        }
    }
    public float groundCheckOffsetX = 0.4f;
    bool IsGround()
    {
        bool result = false;

        if (IsGroundCheckRay(transform.position))
            return true;
        if (IsGroundCheckRay(transform.position + new Vector3(-groundCheckOffsetX, 0, 0)))
            return true;
        if (IsGroundCheckRay(transform.position + new Vector3(groundCheckOffsetX, 0, 0)))
            return true;


        return result;
    }
    bool IsGroundCheckRay(Vector3 pos)
    {
        var hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), 1.1f, wallLayer);
        if (hit.transform)
            return true;
        return false;
    }
    private void OnDrawGizmos()
    {
        DrawRay(transform.position);

        // 좌.
        DrawRay(transform.position + new Vector3(-groundCheckOffsetX, 0, 0));

        // 우.
        DrawRay(transform.position + new Vector3(groundCheckOffsetX, 0, 0));
    }

    private void DrawRay(Vector3 position)
    {
        Gizmos.DrawRay(position + new Vector3(0, downWallCheckY, 0), new Vector2(0, -1) * 20f);
    }
    public LayerMask wallLayer;
    public float downWallCheckY = -2.1f;
    private void DownJump()
    {
        // s키 아래점프
        // 점프 가능한지 판단
        // 아래로 광선을 쏴서 벽이 있따면 아래로 점프
        if (rigidbody2D.velocity.y == 0 && Input.GetKeyDown(KeyCode.S))
        {
            var hit = Physics2D.Raycast(
                transform.position + new Vector3(0, downWallCheckY)
                , new Vector2(0, -1), 20, wallLayer);
            if (hit.transform)
            {
                Debug.Log(hit.point);
                ingDownJump = true;
                collider2D.isTrigger = true;
            }
        }
    }


    bool ingDownJump = false;

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("탈출");
        if (ingDownJump)
        {
            ingDownJump = false;
            collider2D.isTrigger = false;
        }
    }
}
