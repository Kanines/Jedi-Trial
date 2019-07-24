using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private bool _canDamage = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Hit: " + other.name);

        IDamageable hit = other.GetComponent<IDamageable>();
        IDamageDealer dmg = gameObject.GetComponentInParent<IDamageDealer>();

        if (hit != null && _canDamage)
        {
            hit.Damage(dmg.DamageAmount, dmg.DamageSource);
            _canDamage = false;
            StartCoroutine(ResetDamage());
        }
    }

    IEnumerator ResetDamage()
    {
        yield return new WaitForSeconds(0.4f);
        _canDamage = true;
    }
}
