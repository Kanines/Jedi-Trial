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
        if (isDead)
            return;

        isHit = true;
        anim.SetTrigger("Hit");
        anim.SetBool("InCombat", true);

        Debug.Log(this.name + " obtained " + damageAmount + " damage! Health: " + Health);
        Health -= damageAmount;

        if (Health < 1)
        {
            isDead = true;
            if (isFlipped == false)
            {
                sprite.transform.Translate(-mainSpriteSize.x / 2, 0, 0);
            }
            else
            {
                sprite.transform.Translate(mainSpriteSize.x / 2, 0, 0);
            }
            anim.SetTrigger("Death");

            StartCoroutine(VanishCorpse());
        }
    }

    IEnumerator VanishCorpse()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(this.gameObject);
    }
}
