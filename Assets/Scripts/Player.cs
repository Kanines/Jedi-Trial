using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rigid;
    [SerializeField]
    private float _jumpForce = 5.0f;
    [SerializeField]
    private bool _isGrounded = false;
    [SerializeField]
    private LayerMask _groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Horizontal movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, _jumpForce);
            _isGrounded = false;
        }

        // Jump
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 1.05f, 1 << 8);
        Debug.DrawRay(transform.position, Vector2.down * 1.05f, Color.red);

        if (hitInfo.collider != null)
        {
            Debug.Log(hitInfo.collider.name);
            _isGrounded = true;
        }

        _rigid.velocity = new Vector2(horizontalInput, _rigid.velocity.y);
    }
}
