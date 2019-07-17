using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public GameObject coinGoldPrefab;
    [SerializeField]
    protected int health;
    [SerializeField]
    protected int speed;
    [SerializeField]
    protected int rewardPoints;
    [SerializeField]
    protected Transform pointA, pointB;
    protected Vector3 pathTarget;
    protected Animator anim;
    protected SpriteRenderer sprite;
    protected bool isHit = false;
    protected Player player;
    protected bool isDead;
    protected Vector3 mainSpriteSize;
    public bool isFlipped = false;
    public bool canAttack = true;
    protected float attackCooldown = 2.0f;
    public bool isChasing = false;

    private void Start()
    {
        Init();
        pathTarget = pointA.position;
    }

    public virtual void Init()
    {
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        mainSpriteSize = sprite.sprite.bounds.size;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public virtual void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && anim.GetBool("InCombat") == false)
        {
            return;
        }

        if (isDead == false)
        {
            Movement();
        }
    }

    public virtual void Movement()
    {
        Vector3 pathTargetDirection = pathTarget - transform.localPosition;

        if (pathTargetDirection.x > 0 && anim.GetBool("InCombat") == false)
        {
            FlipX(true);
        }
        else if (pathTargetDirection.x < 0 && anim.GetBool("InCombat") == false)
        {
            FlipX(false);
        }

        if (transform.position == pointA.transform.position && anim.GetBool("InCombat") == false)
        {
            pathTarget = pointB.position;
            anim.SetTrigger("Idle");
        }
        else if (transform.position == pointB.transform.position && anim.GetBool("InCombat") == false)
        {
            pathTarget = pointA.position;
            anim.SetTrigger("Idle");
        }

        if (anim.GetBool("InCombat") == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, pathTarget, speed * Time.deltaTime);
        }

        // melee
        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        Vector3 playerDirection = player.transform.localPosition - transform.localPosition;

        if (playerDistance <= 1.5f && isChasing)
        {
            if (anim.GetBool("InCombat") == false)
                anim.SetBool("InCombat", true);

            if (canAttack)
            {
                canAttack = false;
                anim.SetTrigger("Attack");
                StartCoroutine(ResetAttackCooldown());
            }
        }
        else if (playerDistance > 1.5f && playerDistance <= 5.0f
                && ((isFlipped == false && playerDirection.x < 0) || (isFlipped && playerDirection.x > 0)))
        {
            if (anim.GetBool("InCombat"))
                anim.SetBool("InCombat", false);

            isChasing = true;
            //isHit = false;        
            pathTarget = player.transform.position;
        }
        else
        {
            if (anim.GetBool("InCombat"))
                anim.SetBool("InCombat", false);
            
            if(isChasing)
            {            
                isChasing = false;

                float pointADist = Vector3.Distance(transform.position, pointA.position);
                float pointBDist = Vector3.Distance(transform.position, pointB.position);

                if (pointADist < pointBDist)
                {
                    pathTarget = pointA.position;
                }
                else
                {
                    pathTarget = pointB.position;
                }  
            }                       
        }

        if (playerDirection.x > 0 && isChasing)
        {
            FlipX(true);
        }
        else if (playerDirection.x < 0 && isChasing)
        {
            FlipX(false);
        }
    }

    public virtual void FlipX(bool flip)
    {
        if (flip)
        {
            if (isFlipped == false)
            {
                isFlipped = true;
                sprite.transform.rotation = Quaternion.Euler(0, 180, 0);
                sprite.transform.Translate(mainSpriteSize.x, 0, 0);
            }
        }
        else
        {
            if (isFlipped)
            {
                isFlipped = false;
                sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                sprite.transform.Translate(mainSpriteSize.x, 0, 0);
            }
        }
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
