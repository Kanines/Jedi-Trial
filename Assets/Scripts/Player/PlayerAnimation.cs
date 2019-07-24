using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void Move(float move)
    {
        _animator.SetFloat("Move", Mathf.Abs(move));
    }

    public void Jump(bool jumping)
    {
        _animator.SetBool("isJumping", jumping);
    }

    public void Attack()
    {
        _animator.SetTrigger("BasicAttack");
    }

    public void Block(bool blocking)
    {
        _animator.SetBool("isBlocking", blocking);
    }

    public void Death()
    {
        _animator.SetTrigger("Death");
    }
}
