using UnityEngine;

public class Trooper : Enemy
{
    [SerializeField]
    private float dangerRange = 3.0f;
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private Transform _firePoint;
    private float dangerRangeSquare;
    private Vector3 targetHeading;
    private bool canFlee = true;

    protected override void Init()
    {
        base.Init();
        dangerRangeSquare = dangerRange * dangerRange;
    }

    protected override void Chase()
    {
        targetHeading = target.transform.position - transform.position;
        float targetDistanceSquare = targetHeading.sqrMagnitude;

        if (targetDistanceSquare < dangerRangeSquare)
        {
            // target withing danger range, kite him

            if (canAttack)
            {
                animator.SetBool("isMoving", false);
                HeadTowardsTarget(target.transform.position);
                canAttack = false;
                animator.SetTrigger("Attack");
                Shoot();
                StartCoroutine(ResetAttackCooldown());
            }
            else
            {
                if (Utils.isNear(transform.position, travelTarget, 0.1f))
                {
                    animator.SetBool("isMoving", false);
                    canFlee = true;
                }
                else
                {
                    animator.SetBool("isMoving", true);
                }

                if (canFlee)
                {
                    travelTarget = transform.position;

                    if (targetHeading.x > 0)
                    {
                        travelTarget.x -= dangerRange;

                        if (travelTarget.x < pathPoints[0].position.x)
                        {
                            travelTarget = target.transform.position;
                            travelTarget.x += dangerRange;
                            canFlee = false;
                        }
                    }
                    else if (targetHeading.x < 0)
                    {
                        travelTarget.x += dangerRange;
                        if (travelTarget.x > pathPoints[pathPoints.Length - 1].position.x)
                        {
                            travelTarget = target.transform.position;
                            travelTarget.x -= dangerRange;
                            canFlee = false;
                        }
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    return;
                }

                if (animator.GetBool("isMoving"))
                {
                    HeadTowardsTarget(travelTarget);
                    transform.position = Vector3.MoveTowards(transform.position, travelTarget, speed * Time.deltaTime);
                }
            }
        }
        else if (targetDistanceSquare < attackRangeSquare)
        {
            // target within attack range, attack him   

            HeadTowardsTarget(target.transform.position);
            animator.SetBool("isMoving", false);
            animator.SetTrigger("Idle");
            if (canAttack)
            {
                canAttack = false;
                animator.SetTrigger("Attack");
                Shoot();
                StartCoroutine(ResetAttackCooldown());
            }

        }
        else if (targetDistanceSquare < chaseDistanceSquare)
        {
            // target within chase range, chase him up to dangerRange

            if (Utils.isNear(transform.position, travelTarget, 0.1f))
            {
                animator.SetBool("isMoving", false);
            }
            else
            {
                animator.SetBool("isMoving", true);
            }

            travelTarget = target.transform.position;
            if (targetHeading.x > 0)
            {
                travelTarget.x -= dangerRange;
            }
            else if (targetHeading.x < 0)
            {
                travelTarget.x += dangerRange;
            }
            travelTarget.y = transform.position.y;

            HeadTowardsTarget(travelTarget);

            // don't move outside roam boundaries
            if (travelTarget.x < pathPoints[0].position.x)
            {
                travelTarget.x = pathPoints[0].position.x;
            }
            else if (travelTarget.x > pathPoints[pathPoints.Length - 1].position.x)
            {
                travelTarget.x = pathPoints[pathPoints.Length - 1].position.x;
            }

            // don't move while in idle state
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                return;
            }

            if (animator.GetBool("isMoving"))
            {
                transform.position = Vector3.MoveTowards(transform.position, travelTarget, speed * Time.deltaTime);
            }
        }
        // target too far, get back to roaming
        else
        {
            animator.SetTrigger("Idle");
            NextTravelPoint(currentPathPointIdx);
            aiState = AIState.Guard;
            target = null;
        }
    }

    public override void Damage(int damageAmount, GameObject damageSource)
    {
        if (aiState != AIState.Dead)
        {
            StartCoroutine(Utils.HitFxRoutine(sprite));
        }
        base.Damage(damageAmount, damageSource);
    }

    private void Shoot()
    {
        GameObject prefab = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
        Bullet bullet = prefab.GetComponent<Bullet>();

        Vector3 targetHeadingNorm = targetHeading.normalized;

        bullet.DamageSource = gameObject;
        bullet.Rigid.velocity = targetHeadingNorm * bullet.Speed;

        float angle = Mathf.Atan2(bullet.Rigid.velocity.y, bullet.Rigid.velocity.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
