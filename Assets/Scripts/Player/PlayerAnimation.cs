﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    public void Move(float move)
    {
        _anim.SetFloat("Move", Mathf.Abs(move));
    }

    public void Jump(bool jumping)
    {
        _anim.SetBool("isJumping", jumping);
    }

    public void Attack()
    {
        _anim.SetTrigger("BasicAttack");
    }

    public void Block(bool blocking)
    {
        _anim.SetBool("isBlocking", blocking);
    }

    public void Death()
    {
        _anim.SetTrigger("Death");
    }
}
