using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tusken : Enemy, IDamageable
{
    public int Health { get; set; }

    public override void Init()
    {
        base.Init();
        Health = base.health;
    }

    public void Damage(int damageAmount)
    {
        isHit = true;
        anim.SetTrigger("Hit");
        anim.SetBool("InCombat", true);
        
        Debug.Log(this.name + " obtained " + damageAmount + " damage!");
        Health -= damageAmount;

        if (Health < 1)
        {
            // die
            Destroy(this.gameObject);
        }
    }
}
