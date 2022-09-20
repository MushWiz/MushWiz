using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decision/TargetFound")]
public class MonsterDecisionTargetFound : MonsterDecision
{
    public override bool Decide(MonsterStateController controller)
    {
        Vector2 direction = controller.target.transform.position - controller.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(controller.transform.position, direction, controller.monsterController.chaseRange, LayerMask.GetMask("Player"));
        bool targetFound = true;
        if (hit.collider == null || hit.collider.gameObject.tag != "Player")
        {
            targetFound = false;
        }
        if (targetFound && Vector2.Distance(controller.target.transform.position, controller.transform.position) <= controller.monsterController.chaseRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}