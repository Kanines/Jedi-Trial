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

    protected override void Init()
    {
        base.Init();
        dangerRangeSquare = dangerRange * dangerRange;
    }

    protected override void Chase()
    {
        float playerDistanceSquare = playerHeading.sqrMagnitude;

        if (playerDistanceSquare < dangerRangeSquare)
        {
            // target withing danger range, kite him

            if (canAttack)
            {
                HeadTowardsTarget(target.transform.position);

                animator.SetBool("isMoving", false);
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
                }
                else
                {
                    animator.SetBool("isMoving", true);
                }

                travelTarget = transform.position;

                //TODO fix kiting near roam boundaries
                if (playerHeading.x > 0)
                {
                    travelTarget.x -= dangerRange;

                    if (travelTarget.x < pathPoints[0].position.x)
                    {
                        travelTarget = target.transform.position;
                        travelTarget.x += dangerRange;
                    }
                }
                else if (playerHeading.x < 0)
                {
                    travelTarget.x += dangerRange;
                    if (travelTarget.x > pathPoints[pathPoints.Length - 1].position.x)
                    {
                        travelTarget = target.transform.position;
                        travelTarget.x -= dangerRange;
                    }
                }

                HeadTowardsTarget(travelTarget);

                // if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                // {
                //     return;
                // }

                if (animator.GetBool("isMoving"))
                {
                    transform.position = Vector3.MoveTowards(transform.position, travelTarget, speed * Time.deltaTime);
                }
            }
        }
        else if (playerDistanceSquare < attackRangeSquare)
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
        else if (playerDistanceSquare < chaseDistanceSquare)
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
            if (playerHeading.x > 0)
            {
                travelTarget.x -= dangerRange;
            }
            else if (playerHeading.x < 0)
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
        //TODO bullet rotation

        GameObject prefab = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
        Bullet bullet = prefab.GetComponent<Bullet>();

        //TODO refactor target & player directions etc
        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.Normalize();

        bullet.DamageSource = gameObject;
        bullet.Rigid.velocity = targetDirection * bullet.Speed;
    }

}
