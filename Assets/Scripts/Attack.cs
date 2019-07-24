using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private bool _canDamage = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Hit: " + other.name);
        
        IDamageable hit = other.GetComponent<IDamageable>();

        if (hit != null && _canDamage)
        {
            hit.Damage(1);
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
