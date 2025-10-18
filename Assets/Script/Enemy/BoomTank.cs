using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomTank : EnemyBase
{
    [SerializeField] private float explodeRadius = 0.7f;

    protected override void Explode(IGetHit getHit)
    {
        base.Explode(getHit);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explodeRadius);
        foreach (Collider2D c in colliders)
        {
            IGetHit h = c.GetComponent<IGetHit>();
            if (h != null && c.gameObject.CompareTag("Enemy")) 
                h.GetHit(_dmgExplosion / 2);
        }
    }
}
