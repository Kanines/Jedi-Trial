using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
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
    protected bool isFlipped = false;

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

        if (transform.position == pointA.transform.position)
        {
            pathTarget = pointB.position;
            anim.SetTrigger("Idle");
        }
        else if (transform.position == pointB.transform.position)
        {
            pathTarget = pointA.position;
            anim.SetTrigger("Idle");
        }

        if (isHit == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, pathTarget, speed * Time.deltaTime);
        }

        // melee
        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        if (playerDistance > 4.0f)
        {
            isHit = false;
            anim.SetBool("InCombat", false);
        }

        Vector3 playerDirection = player.transform.localPosition - transform.localPosition;
        if (playerDirection.x > 0 && anim.GetBool("InCombat"))
        {
            FlipX(true);
        }
        else if (playerDirection.x < 0 && anim.GetBool("InCombat"))
        {
            FlipX(false);
        }
    }

    public virtual void FlipX(bool state)
    {
        if (state)
        {
            sprite.flipX = true;
            if (isFlipped == false)
            {
                sprite.transform.Translate(-mainSpriteSize.x, 0, 0);
                isFlipped = true;
            }
        }
        else
        {
            sprite.flipX = false;
            if (isFlipped)
            {
                sprite.transform.Translate(mainSpriteSize.x, 0, 0);
                isFlipped = false;
            }
        }
    }
}
