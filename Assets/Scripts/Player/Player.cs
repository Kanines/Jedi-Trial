using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable, IDamageDealer
{
    public int score;
    [SerializeField]
    private int _health;
    [SerializeField]
    private float _horizSpeed = 5.0f;
    [SerializeField]
    private float _jumpForce = 5.0f;
    [SerializeField]
    private int _damageAmount = 1;
    private Rigidbody2D _rigid;
    private PlayerAnimation _playerAnim;
    private SpriteRenderer _sprite;
    private Vector3 _mainSpriteSize;
    private bool _resetJump = false;
    private bool _isGrounded = false;
    private bool _isBlocking = false;
    private bool _isDead = false;
    private bool _isFacingRight = true;

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }

    public int DamageAmount
    {
        get
        {
            return _damageAmount;
        }
        set
        {
            _damageAmount = value;
        }
    }

    public GameObject DamageSource
    {
        get
        {
            return gameObject;
        }
    }

    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _playerAnim = GetComponent<PlayerAnimation>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _mainSpriteSize = _sprite.sprite.bounds.size;
        Health = _health;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, Vector2.down * 1.05f, Color.red);

        if (_isDead)
        {
            return;
        }

        Movement();

        // attack
        if (Input.GetMouseButtonDown(0) && IsGrounded())
        {
            _playerAnim.Attack();
        }

        // block
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

    private void Movement()
    {
        // Horizontal Movement
        float horizInput = Input.GetAxisRaw("Horizontal");

        _isGrounded = IsGrounded();

        if (horizInput > 0)
        {
            if (_isFacingRight == false)
            {
                _isFacingRight = true;
                _sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                _sprite.transform.Translate(-_mainSpriteSize.x, 0, 0);
            }
        }
        else if (horizInput < 0)
        {
            if (_isFacingRight)
            {
                _isFacingRight = false;
                _sprite.transform.rotation = Quaternion.Euler(0, 180, 0);
                _sprite.transform.Translate(-_mainSpriteSize.x, 0, 0);
            }
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, _jumpForce);
            _playerAnim.Jump(true);
            StartCoroutine(ResetJumpRoutine());
        }

        _rigid.velocity = new Vector2(horizInput * _horizSpeed, _rigid.velocity.y);

        _playerAnim.Move(horizInput);
    }

    public void Damage(int damageAmount, GameObject damageSource)
    {
        if (_isDead)
        {
            return;
        }

        if (_isBlocking && isFacingTowardsTarget(damageSource.transform.position))
        {
            return;
        }
        else
        {
            Debug.Log(gameObject.name + "[hp:" + _health + "] received " + damageAmount + " damage from " + damageSource.name);

            Health -= damageAmount;
            UIManager.Instance.UpdateHealth(Health);
            StartCoroutine(Utils.HitFxRoutine(_sprite));

            if (Health < 1)
            {
                _isDead = true;
                _sprite.transform.Translate(_mainSpriteSize.x / 2, 0, 0);
                _playerAnim.Death();
            }
        }
    }

    public void AddScore(int scorePoints)
    {
        score += scorePoints;
        UIManager.Instance.UpdateScore(score);
    }

    private bool IsGrounded()
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

    private bool isFacingTowardsTarget(Vector3 targetPosition)
    {
        Vector3 targetHeading = targetPosition - transform.position;

        if (targetHeading.x > 0 && _isFacingRight)
        {
            return true;
        }
        else if (targetHeading.x < 0 && _isFacingRight == false)
        {
            return true;
        }

        return false;
    }

    private IEnumerator ResetJumpRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        _resetJump = false;
    }
}
