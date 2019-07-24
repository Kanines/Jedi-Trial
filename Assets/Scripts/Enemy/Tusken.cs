using UnityEngine;

public class Tusken : Enemy
{
    protected override void Init()
    {
        base.Init();
        attackCooldown = 1.0f;
    }

    protected override void Chase()
    {
        float playerDistanceSquare = playerHeading.sqrMagnitude;

        HeadTowardsTarget(target.transform.position);

        // target within attack range, attack him
        if (playerDistanceSquare < attackRangeSquare)
        {
            animator.SetBool("isMoving", false);
            if (canAttack)
            {
                canAttack = false;
                animator.SetTrigger("Attack");
                StartCoroutine(ResetAttackCooldown());
            }
        }
        // target within chase range, chase him
        else if (playerDistanceSquare < chaseDistanceSquare)
        {
            if (Utils.isNear(transform.position, travelTarget, 0.1f))
            {
                animator.SetBool("isMoving", false);
            }
            else
            {
                animator.SetBool("isMoving", true);
            }

            travelTarget = target.transform.position;
            travelTarget.y = transform.position.y;

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
}
