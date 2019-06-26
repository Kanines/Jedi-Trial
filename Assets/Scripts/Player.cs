using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rigid;
    [SerializeField]
    private float _jumpForce = 5.0f;
    private bool _resetJump = false;
    [SerializeField]
    private float _horizSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        // Horizontal Movement
        float horizInput = Input.GetAxisRaw("Horizontal");
        _rigid.velocity = new Vector2(horizInput * _horizSpeed, _rigid.velocity.y);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, _jumpForce);
            StartCoroutine(ResetJumpRoutine());
        }
    }

    bool IsGrounded()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 1.05f, 1 << 8);
        //Debug.DrawRay(transform.position, Vector2.down * 1.05f, Color.red);

        if (hitInfo.collider != null)
        {
            //Debug.Log(hitInfo.collider.name);
            if (_resetJump == false)
                return true;
        }

        return false;
    }

    IEnumerator ResetJumpRoutine()
    {
        _resetJump = true;
        yield return new WaitForSeconds(0.1f);
        _resetJump = false;
    }
}
