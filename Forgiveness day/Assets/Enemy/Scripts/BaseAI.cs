using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI : MonoBehaviour
{
    public EnemyState State;

    private void Update()
    {
        switch (State)
        {
            case EnemyState.Attack:
                Attacking();
                return;
            case EnemyState.Stun:
                Stuning();
                return;
            case EnemyState.Run:
                Runing(); 
                return;
            case EnemyState.Chase:
                Chaseing();
                return;
            case EnemyState.Chill:
                Chilling();
                return;
            case EnemyState.Evade:
                Evade();
                return;
            case EnemyState.FindTheWay:
                Finding();
                return;
        }
    }

    protected virtual void Attacking() { }
    protected virtual void Stuning() { }
    protected virtual void Runing() { }
    protected virtual void Chaseing() { }
    protected virtual void Chilling() { }
    protected virtual void Evade() { }
    protected virtual void Finding() { }
}
