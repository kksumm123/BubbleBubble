using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    // 앞으로 움직여라.
    // 벽을 만나면 뒤로 방향전환

    new Rigidbody2D rigidbody2D;
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    public float speed = 0.1f;
    void FixedUpdate()
    {
        var pos = rigidbody2D.position;
        pos.x += speed * transform.forward.z;
        rigidbody2D.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
            return;
        transform.rotation =
            new Quaternion(0, (transform.rotation.y == 0 ? 180 : 0), 0, 0);
    }
}
