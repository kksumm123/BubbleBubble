
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public int moveForwardFrame = 6;
    public int currentFrame = 0;
    public float speed = 0.7f;
    new public Rigidbody2D rigidbody2D;
    public float gravityScale = -0.7f;
    // 앞쪽 방향으로 이동., 6프레임 움직이고 나서 위로 이동(중력에 의해)
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
    }

    public LayerMask wallLayer;
    private State state;

    // 벽 뚫는 현상 수정되었음, 
    // 벽과 이미 충돌한상태로 생성되면 충돌되지 않음 <- 이 상황에선 버블이 터져야함.

    private void FixedUpdate()
    {
        if (currentFrame++ < moveForwardFrame)
        {
            var pos = rigidbody2D.position;
            pos.x += (speed * transform.forward.z);
            // 버블이 앞으로 가고 있으면 최대 X값을 레이캐스트로 찾자
            //          뒤로 가고 있으면 최소 x값을 레이캐스트로 찾자
            if (transform.forward.z > 0)
            { // 앞으로 가고 있다
                var hit = Physics2D.Raycast(transform.position, new Vector2(1, 0), 100, wallLayer);
                Debug.Assert(hit.transform != null, "레이어지정해야함", rigidbody2D);
                float maxX = hit.point.x;
                pos.x = Mathf.Min(pos.x, maxX);
            }
            else
            { // 뒤로 가고 있다
                var hit = Physics2D.Raycast(transform.position, new Vector2(-1, 0), 100, wallLayer);
                float minX = hit.point.x;
                pos.x = Mathf.Max(pos.x, minX);
            }

            rigidbody2D.position = pos;
        }
        else
        {
            state = State.FREEFLY;
            rigidbody2D.gravityScale = gravityScale;
            enabled = false;
        }
    }

    // 버블이 앞으로 나아가는 상태 - 몬스터 닿으면 몬스터 잡힘
    // 버블이 자유롭게 이동하는 상태 - 플레이어가 닿으면 버블 터짐
    // 버블이 터지고 있는 상태 - 필요없음
    enum State
    {
        FASTMODE,
        FREEFLY,
        NONE
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"collision : {collision.transform.name}");
        // 버블이 터질만큼 만힝 붙어 있다면 터트리자.
        OnTouchCoillision(collision.transform);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"trigger : {collision.transform.name}");
        OnTouchCoillision(collision.transform);
    }
    void OnTouchCoillision(Transform tr)
    {
        if (state == State.FREEFLY)
        {
            if (tr.CompareTag("Player"))
            {
                //플레이어
                Destroy(gameObject);
            }
        }
    }
}