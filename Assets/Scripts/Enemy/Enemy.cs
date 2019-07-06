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
    private Vector3 _mainSpriteSize;

    private void Start()
    {
        Init();
        pathTarget = pointA.position;
    }

    public virtual void Init()
    {
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        _mainSpriteSize = sprite.sprite.bounds.size;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public virtual void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && anim.GetBool("InCombat") == false)
        {
            return;
        }

        Movement();
    }

    public virtual void Movement()
    {
        if (pathTarget == pointA.position)
        {
            if (sprite.flipX == false)
            {
                sprite.flipX = true;
                sprite.transform.position = new Vector3(sprite.transform.position.x - sprite.sprite.bounds.size.x,
                        sprite.transform.position.y, sprite.transform.position.z);
            }
        }
        else
        {
            if (sprite.flipX)
            {
                sprite.flipX = false;
                sprite.transform.position = new Vector3(sprite.transform.position.x + sprite.sprite.bounds.size.x,
                        sprite.transform.position.y, sprite.transform.position.z);
            }
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

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= 10.0f)
        {
            Debug.Log("Distance to " + this.name + " is: " + distance);
        }

        if (distance > 2.0f)
        {
            isHit = false;
            anim.SetBool("InCombat", false);
        }
    }
}
