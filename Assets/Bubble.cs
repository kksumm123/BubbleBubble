
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    internal int testint;
    public int testint2;
    static public List<Bubble> Items = new List<Bubble>();
    public int moveForwardFrame = 6;
    public int currentFrame = 0;
    public float speed = 0.7f;
    new public Rigidbody2D rigidbody2D;
    public float gravityScale = -0.2f;
    private void Awake()
    {
        Items.Add(this);
    }
    private void OnDestroy()
    {
        Items.Remove(this);
    }
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
            float distance = Vector3.Distance(Player.instance.transform.position
                    , transform.position);

            if (distance < nearPlayerCheckDistance)
            {
                // 공룡이 인근에 있으면 자신(버블)을 터트리자
                ExplosionByPlayer();
            }
            else
            {
                state = State.FREEFLY;
                rigidbody2D.gravityScale = gravityScale;
                GetComponent<Collider2D>().isTrigger = false;
                enabled = false;
            }

        }
    }
    public float nearPlayerCheckDistance = 1.9f;
    public float nearBubbleDistance = 2.2f;
    private void ExplosionByPlayer()
    {
        ////// 인근의 버블을 모두 터트리자
        // 모든 버블에 접근하자
        // 인근의 버블을 모으자
        // 인근의 버블을 모두 터트리자
        Vector2 pos = transform.position;
        List<Bubble> nearBubbles = new List<Bubble>();
        FindNearBubbles(pos, nearBubbles);

        nearBubbles.ForEach(x => Destroy(x.gameObject));
    }

    private void FindNearBubbles(Vector2 pos, List<Bubble> nearBubbles)
    {
        nearBubbles.Add(this);
        foreach (var item in Items)
        {
            //pos 가까이(2.2)에 있는 버블 모으자
            if (nearBubbles.Contains(item))
                continue;
            float distance = Vector2.Distance(item.transform.position, pos);
            if (distance < nearBubbleDistance)
            {
                nearBubbles.Add(item);
                FindNearBubbles(item.transform.position, nearBubbles);
            }
        }
    }


    // 버블이 앞으로 나아가는 상태 - 몬스터 닿으면 몬스터 잡힘
    // 버블이 자유롭게 이동하는 상태 - 플레이어가 닿으면 버블 터짐
    // 버블이 터지고 있는 상태 - 필요없음
    enum State
    {
        FASTMODE,
        FREEFLY,
        NONE,
        Capture
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 버블이 터질만큼 만힝 붙어 있다면 터트리자.
        OnTouchCollision(collision.transform);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTouchCollision(collision.transform);
    }
    public float jumpHeight = 0.5f; // 플레이어가 버블 밟은걸로 인정되는 높이
    void OnTouchCollision(Transform tr)
    {
        if (state == State.FREEFLY)
        {

            if (tr.CompareTag("Player"))
            {
                //플레이어
                // 플레이어가 나보다 높게 있다면
                // 플레이어가 방향키를 위로 하고 있다면
                bool pressedUpKey = Input.GetKey(KeyCode.W)
                    || Input.GetKey(KeyCode.UpArrow);
                if (pressedUpKey
                    && transform.position.y + jumpHeight < tr.position.y)
                {
                    // 버블을 터트리지말고 놔두자
                    //버블이 작아졌다가 터지는 트윈효과
                    tr.GetComponent<Player>().StartJump();
                }
                else
                    ExplosionByPlayer();
            }
        }
        else if (state == State.FASTMODE)
        {
            if (tr.CompareTag("Enemy"))
            {
                // 적을 안보이게 하자
                tr.gameObject.SetActive(false);
                // 버블 트랜스폼에 자식으로 달자
                tr.parent = transform;

                // 버블의 상태를 캡처 상태로 변경
                state = State.Capture;
                string monsterName = tr.GetComponent<Monster>().monsterName;
                // 버블 이미지를 몬스터 잡은 애니메이션 플레이
                GetComponent<Animator>().Play(monsterName + "Bubble");

                // 버블이 몇초 후에 터짐.
                // 시간이 지날수록 녹색, 파란색, 노란색, 빨간색

            }
        }
    }
}