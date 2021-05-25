using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 8f;
    new public Rigidbody2D rigidbody2D;
    new public Collider2D collider2D;
    public Animator animator;
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        Application.targetFrameRate = 60;
    }
    private void Update()
    {
        // 버블발사
        FireBubble();
        // 움직임
        Move();
        // 점프
        Jump();
    }

    public float jumpForce = 500f;
    private void Jump()
    {
        // 점프할 때 벽을 뚫어야함
        // 낙하시에는 뚫으면 안됨
        // 속도가 음수일 때, 이전 y값과 비교시 -일 때
        if (rigidbody2D.velocity.y < 0)
        {
            collider2D.isTrigger = false;
        }
        if (rigidbody2D.velocity.y == 0)
        {
            // 다중 점프 막아야함
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                rigidbody2D.AddForce(new Vector2(0, jumpForce));
                collider2D.isTrigger = true;
            }
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
    public float minX, maxX;
    private void Move()
    {
        // WASD, W위로, A왼쪽,S아래, D오른쪽
        float moveX = 0;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX = -1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX = 1;



        Vector3 position = transform.position;
        position.x += moveX * speed * Time.deltaTime;
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
}
