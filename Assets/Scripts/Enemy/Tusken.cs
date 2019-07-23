using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tusken : Enemy
{
    public override void Init()
    {
        base.Init();
        attackCooldown = 1.0f;
    }

    protected override void Chase()
    {
        float playerDistanceSquare = playerHeading.sqrMagnitude;

        HeadTowardsTarget(target.transform.position);

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
        else if (playerDistanceSquare < chaseDistanceSquare)
        {
            if (transform.position.x > pathPoints[0].position.x
            && transform.position.x < pathPoints[pathPoints.Length - 1].position.x)
            {
                animator.SetBool("isMoving", true);

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    return;
                }

                travelTarget = player.transform.position;
                travelTarget.y = transform.position.y;
                transform.position = Vector3.MoveTowards(transform.position, travelTarget, speed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("isMoving", false);
                animator.SetTrigger("Idle");

            }
        }
        else
        {
            animator.SetTrigger("Idle");
            NextTravelPoint(currentPathPointIdx);
            aiState = AIState.Guard;
        }
    }
}
