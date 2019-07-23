using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected GameObject dropPrefab;
    [SerializeField]
    private int _health;
    [SerializeField]
    protected int speed;
    [SerializeField]
    protected int rewardPoints;
    [SerializeField]
    protected float attackCooldown = 1.5f;
    [SerializeField]
    protected int damage = 1;
    [SerializeField]
    protected Transform[] pathPoints;
    protected Animator animator;
    protected SpriteRenderer sprite;
    protected Vector3 mainSpriteSize;
    protected bool isFacingRight = false;
    protected AIState aiState = AIState.Guard;
    protected Vector3 travelTarget;
    protected Player player;
    protected bool canAttack = true;
    protected int currentPathPointIdx;
    protected GameObject target;
    protected Vector3 playerHeading;
    protected float playerDistance;
    protected Vector3 playerDirection;
    protected float viewDistance = 6.0f;
    protected float viewDistanceSquare;
    protected float attackRange = 1.5f;
    protected float attackRangeSquare;
    protected float chaseDistance = 8.0f;
    protected float chaseDistanceSquare;

    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        mainSpriteSize = sprite.sprite.bounds.size;

        if (pathPoints.Length > 0)
        {
            currentPathPointIdx = 0;
            travelTarget = pathPoints[currentPathPointIdx].position;
        }

        viewDistanceSquare = viewDistance * viewDistance;
        attackRangeSquare = attackRange * attackRange;
        chaseDistanceSquare = chaseDistance * chaseDistance;
    }

    public virtual void Update()
    {
        // if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        // {
        //     return;
        // }

        if (aiState == AIState.Dead)
        {
            return;
        }

        playerHeading = player.transform.position - transform.position;

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
        if (playerHeading.x > 0 && isFacingRight)
        {
            if (playerHeading.sqrMagnitude < viewDistanceSquare)
            {
                aiState = AIState.Combat;
                target = player.gameObject;
            }
        }
        else if (playerHeading.x < 0 && isFacingRight == false)
        {
            if (playerHeading.sqrMagnitude < viewDistanceSquare)
            {
                aiState = AIState.Combat;
                target = player.gameObject;
            }
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

    public virtual void Damage(int damageAmount)
    {
        if (aiState == AIState.Dead)
        {
            return;
        }

        animator.SetTrigger("Hit");

        if (aiState != AIState.Combat)
        {
            target = player.gameObject; // need to get info who is damage dealer to set real target
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
            if (isFacingRight == false)
            {
                isFacingRight = true;
                sprite.transform.rotation = Quaternion.Euler(0, 180, 0);
                sprite.transform.Translate(mainSpriteSize.x, 0, 0);
            }
        }
        else
        {
            if (isFacingRight)
            {
                isFacingRight = false;
                sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                sprite.transform.Translate(mainSpriteSize.x, 0, 0);
            }
        }
    }

    protected virtual IEnumerator Die()
    {
        yield return new WaitForSeconds(0.2f);
        if (isFacingRight == false)
        {
            sprite.transform.Translate(-mainSpriteSize.x / 2, 0, 0);
        }
        else
        {
            sprite.transform.Translate(-mainSpriteSize.x / 2, 0, 0);
        }
        animator.SetTrigger("Death");
        Instantiate(dropPrefab, transform.position, Quaternion.identity);
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
