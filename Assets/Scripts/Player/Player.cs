using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int _health;
    [SerializeField]
    private float _jumpForce = 5.0f;
    [SerializeField]
    private float _horizSpeed = 5.0f;
    private bool _resetJump = false;
    private bool _isGrounded = false;
    private bool _isBlocking = false;
    private Rigidbody2D _rigid;
    private PlayerAnimation _playerAnim;
    private SpriteRenderer _sprite;
    private Vector3 _mainSpriteSize;
    private Color _hitColor = new Color(1.0f, 0.7f, 0.1f, 1.0f);
    private bool _isDead = false;
    private bool _isFlipped = false;

    public int Health { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _playerAnim = GetComponent<PlayerAnimation>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _mainSpriteSize = _sprite.sprite.bounds.size;
        Health = _health;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, Vector2.down * 1.05f, Color.red);

        if (_isDead)
            return;

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
        else if (Input.GetMouseButtonUp(1))
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
            if (_isFlipped)
            {
                _isFlipped = false;
                _sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                _sprite.transform.Translate(-_mainSpriteSize.x, 0, 0);
            }
        }
        else if (horizInput < 0)
        {
            if (_isFlipped == false)
            {
                _isFlipped = true;
                _sprite.transform.rotation = Quaternion.Euler(0, 180, 0);
                _sprite.transform.Translate(-_mainSpriteSize.x, 0, 0);
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
            if (_resetJump == false)
            {
                _playerAnim.Jump(false);
                return true;
            }
        }

        return false;
    }

    public void Damage(int damageAmount)
    {
        if (_isDead)
            return;

        if (_isBlocking == false)
        {
            Debug.Log(this.name + " obtained " + damageAmount + " damage! Health: " + Health);
            Health -= damageAmount;
            StartCoroutine(HitFxRoutine());

            if (Health < 1)
            {
                _isDead = true;
                if (_isFlipped == false)
                {
                    _sprite.transform.Translate(_mainSpriteSize.x / 2, 0, 0);
                }
                else
                {
                    _sprite.transform.Translate(_mainSpriteSize.x / 2, 0, 0);
                }
                _playerAnim.Death();
            }
        }
    }

    IEnumerator HitFxRoutine()
    {
        _sprite.color = _hitColor;
        yield return new WaitForSeconds(0.25f);
        _sprite.color = Color.white;
    }

    IEnumerator ResetJumpRoutine()
    {
        _resetJump = true;
        yield return new WaitForSeconds(0.5f);
        _resetJump = false;
    }
}
