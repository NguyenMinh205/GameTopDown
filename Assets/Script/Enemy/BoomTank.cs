using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomTank : EnemyBase
{
    [SerializeField] private float explodeRadius = 0.7f;

    protected override void Update()
    {

        FollowPlayer();
    }

    protected void Explode(IGetHit getHit)
    {
        getHit.GetHit(_dmgExplosion);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explodeRadius);
        foreach (Collider2D c in colliders)
        {
            IGetHit h = c.GetComponent<IGetHit>();
            if (h != null && c.gameObject.CompareTag("Enemy"))
                h.GetHit(_dmgExplosion / 4);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IGetHit isCanGetHit = collision.gameObject.GetComponent<IGetHit>();

        if (isCanGetHit == null || collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        Explode(isCanGetHit);
        Die();
    }
}
