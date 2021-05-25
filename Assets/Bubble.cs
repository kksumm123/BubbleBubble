using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    // 6프레임 동안 중력 무시하고
    // 앞으로 가다가
    // 위로 올라가도록 
    public int moveForwardFrame = 6;
    public float speed = 5f;
    public Rigidbody2D rigid;
    public float gravityScale = -0.7f;
    private int currentFrame;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.gravityScale = gravityScale;
    }

    private void FixedUpdate()
    {
        if (currentFrame < moveForwardFrame)
        {
            var pos = rigid.position;
            pos.x += speed * transform.forward.x;
            rigid.position = pos;
        }
    }
}
