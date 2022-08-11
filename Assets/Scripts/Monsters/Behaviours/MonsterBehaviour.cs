using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : ScriptableObject
{

    public virtual void Move(MonsterController controller)
    {

    }

    public virtual bool CanMove(MonsterController controller)
    {
        return true;
    }

    public virtual void Attack(MonsterController controller)
    {

    }

    public virtual bool CanAttack(MonsterController controller)
    {
        return true;
    }

    public virtual void Die(MonsterController controller)
    {

    }

    public virtual void Think(MonsterController controller)
    {

    }

}
