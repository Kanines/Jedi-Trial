using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _jumpForce = 5.0f;
    [SerializeField]
    private float _horizSpeed = 5.0f;
    private bool _resetJump = false;
    private bool _isGrounded = false;
    private bool _isBlocking = false;

    private Rigidbody2D _rigid;
    private PlayerAnimation _playerAnim;
    private SpriteRenderer _playerSprite;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _playerAnim = GetComponent<PlayerAnimation>();
        _playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, Vector2.down * 1.05f, Color.red);
        Movement();

        if (Input.GetMouseButtonDown(0) && IsGrounded())
        {
            _playerAnim.Attack();
        }

        if (Input.GetMouseButtonDown(1) && IsGrounded() && !_isBlocking)
        {
            _playerAnim.Block(true);
            _isBlocking = true;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            _playerAnim.Block(false);
            _isBlocking = false;
        }
    }

    void Movement()
    {
        // Horizontal Movement
        float horizInput = Input.GetAxisRaw("Horizontal");

        _isGrounded = IsGrounded();

        if (horizInput > 0)
        {
            if (_playerSprite.flipX)
            {
                _playerSprite.flipX = false;
                _playerSprite.transform.position = new Vector3(_playerSprite.transform.position.x - _playerSprite.sprite.bounds.size.x,
                    _playerSprite.transform.position.y, _playerSprite.transform.position.z);
            }
        }
        else if (horizInput < 0)
        {
            if (_playerSprite.flipX == false)
            {
                _playerSprite.flipX = true;
                _playerSprite.transform.position = new Vector3(_playerSprite.transform.position.x + _playerSprite.sprite.bounds.size.x,
                    _playerSprite.transform.position.y, _playerSprite.transform.position.z);
            }
        }
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, _jumpForce);
            StartCoroutine(ResetJumpRoutine());
            _playerAnim.Jump(true);
        }

        _rigid.velocity = new Vector2(horizInput * _horizSpeed, _rigid.velocity.y);

        _playerAnim.Move(horizInput);
    }

    bool IsGrounded()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 1.05f, 1 << 8);

        if (hitInfo.collider != null)
        {
            //Debug.Log(hitInfo.collider.name);
            if (_resetJump == false)
            {
                Debug.Log("Grounded");
                _playerAnim.Jump(false);
                return true;
            }
        }

        return false;
    }

    IEnumerator ResetJumpRoutine()
    {
        _resetJump = true;
        yield return new WaitForSeconds(0.5f);
        _resetJump = false;
    }
}
