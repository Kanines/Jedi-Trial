using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tusken : Enemy
{
    private Vector3 _pathTarget;
    private Animator _anim;
    private SpriteRenderer _tuskenSprite;

    // Start is called before the first frame update
    void Start()
    {
        _pathTarget = pointA.position;
        _anim = GetComponentInChildren<Animator>();
        _tuskenSprite = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Attack()
    {

    }

    // Update is called once per frame
    public override void Update()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;

        Movement();
    }

    private void Movement()
    {
        if (_pathTarget == pointA.position)
        {
            if (_tuskenSprite.flipX == false)
            {
                _tuskenSprite.flipX = true;
                _tuskenSprite.transform.position = new Vector3(_tuskenSprite.transform.position.x - _tuskenSprite.sprite.bounds.size.x,
                        _tuskenSprite.transform.position.y, _tuskenSprite.transform.position.z);
            }
        }
        else
        {
            if (_tuskenSprite.flipX)
            {
                _tuskenSprite.flipX = false;
                _tuskenSprite.transform.position = new Vector3(_tuskenSprite.transform.position.x + _tuskenSprite.sprite.bounds.size.x,
                        _tuskenSprite.transform.position.y, _tuskenSprite.transform.position.z);
            }
        }

        if (transform.position == pointA.transform.position)
        {
            _pathTarget = pointB.position;
            _anim.SetTrigger("Idle");
        }
        else if (transform.position == pointB.transform.position)
        {
            _pathTarget = pointA.position;
            _anim.SetTrigger("Idle");
        }

        transform.position = Vector3.MoveTowards(transform.position, _pathTarget, speed * Time.deltaTime);
    }
}
