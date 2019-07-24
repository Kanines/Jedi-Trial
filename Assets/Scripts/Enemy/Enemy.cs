using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected Transform[] pathPoints;
    [SerializeField]
    protected float speed = 3.0f;
    [SerializeField]
    protected float viewDistance = 6.0f;
    [SerializeField]
    protected float chaseDistance = 8.0f;
    [SerializeField]
    protected float attackRange = 1.5f;
    [SerializeField]
    protected float attackCooldown = 1.5f;
    protected Animator animator;
    protected AIState aiState = AIState.Guard;
    protected Vector3 travelTarget;
    protected int currentPathPointIdx;
    protected GameObject target;
    protected Vector3 playerHeading;
    protected bool canAttack = true;
    protected float viewDistanceSquare;
    protected float chaseDistanceSquare;
    protected float attackRangeSquare;
    [SerializeField]
    private int _health;
    [SerializeField]
    private GameObject _dropPrefab;
    [SerializeField]
    private int _rewardPoints;
    private SpriteRenderer _sprite;
    private bool _isFacingRight = false;
    private Vector3 _mainSpriteSize;
    private Player _player;

    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (aiState == AIState.Dead)
        {
            return;
        }

        playerHeading = _player.transform.position - transform.position;

        switch (aiState)
        {
            case AIState.Guard:
                Roam();
                break;
            case AIState.Combat:
                Chase();
                break;
        }
    }

    protected virtual void Init()
    {
        animator = GetComponentInChildren<Animator>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _mainSpriteSize = _sprite.sprite.bounds.size;

        if (pathPoints.Length > 0)
        {
            currentPathPointIdx = 0;
            travelTarget = pathPoints[currentPathPointIdx].position;
        }

        viewDistanceSquare = viewDistance * viewDistance;
        attackRangeSquare = attackRange * attackRange;
        chaseDistanceSquare = chaseDistance * chaseDistance;
    }

    protected abstract void Chase();

    protected virtual void Roam()
    {
        animator.SetBool("isMoving", true);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            return;
        }

        HeadTowardsTarget(travelTarget);

        if (Utils.isNear(transform.position, travelTarget))
        {
            NextTravelPoint();
            animator.SetTrigger("Idle");
        }

        travelTarget.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, travelTarget, speed * Time.deltaTime);

        // check if enemy can see player
        if (playerHeading.x > 0 && _isFacingRight)
        {
            if (playerHeading.sqrMagnitude < viewDistanceSquare)
            {
                aiState = AIState.Combat;
                target = _player.gameObject;
            }
        }
        else if (playerHeading.x < 0 && _isFacingRight == false)
        {
            if (playerHeading.sqrMagnitude < viewDistanceSquare)
            {
                aiState = AIState.Combat;
                target = _player.gameObject;
            }
        }
    }

    public virtual void Damage(int damageAmount)
    {
        if (aiState == AIState.Dead)
        {
            return;
        }

        animator.SetTrigger("Hit");

        if (aiState != AIState.Combat)
        {
            target = _player.gameObject; // need to get info who is damage dealer to set real target
            aiState = AIState.Combat;
        }

        Debug.Log(this.name + " obtained " + damageAmount + " damage! Health: " + _health);
        _health -= damageAmount;

        if (_health < 1)
        {
            aiState = AIState.Dead;
            StartCoroutine(Die());
        }
    }

    protected void NextTravelPoint(int idx = -1)
    {
        if (idx != -1)
        {
            travelTarget = pathPoints[idx].position;
            return;
        }

        currentPathPointIdx++;
        if (currentPathPointIdx >= pathPoints.Length)
        {
            currentPathPointIdx = 0;
        }
        travelTarget = pathPoints[currentPathPointIdx].position;
    }

    protected void HeadTowardsTarget(Vector3 targetPosition)
    {
        Vector3 targetHeading = targetPosition - transform.position;

        if (targetHeading.x > 0)
        {
            FaceRight(true);
        }
        else
        {
            FaceRight(false);
        }
    }

    protected void FaceRight(bool faceRight)
    {
        if (faceRight)
        {
            if (_isFacingRight == false)
            {
                _isFacingRight = true;
                _sprite.transform.rotation = Quaternion.Euler(0, 180, 0);
                _sprite.transform.Translate(_mainSpriteSize.x, 0, 0);
            }
        }
        else
        {
            if (_isFacingRight)
            {
                _isFacingRight = false;
                _sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                _sprite.transform.Translate(_mainSpriteSize.x, 0, 0);
            }
        }
    }

    protected virtual IEnumerator Die()
    {
        yield return new WaitForSeconds(0.2f);
        if (_isFacingRight == false)
        {
            _sprite.transform.Translate(-_mainSpriteSize.x / 2, 0, 0);
        }
        else
        {
            _sprite.transform.Translate(-_mainSpriteSize.x / 2, 0, 0);
        }
        animator.SetTrigger("Death");
        Instantiate(_dropPrefab, transform.position, Quaternion.identity);
        StartCoroutine(VanishCorpse());
    }

    protected IEnumerator VanishCorpse()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(this.gameObject);
    }

    protected IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
