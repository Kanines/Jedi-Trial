using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 5.0f;
    private Rigidbody2D _rigid;
    
    void Start()
    {
        _rigid.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Player player = other.GetComponent<Player>();
        if(player != null)
        {
            player.Damage(1);
        }
        Destroy(gameObject);
    }
}
