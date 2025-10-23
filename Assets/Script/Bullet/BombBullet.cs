using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBullet : BulletBase
{
    [SerializeField] private float explosionRadius = 1f;
    protected override void Hit(GameObject target)
    {
        Debug.Log("Check 1");
        base.Hit(target);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        Debug.Log("check2");
        foreach (Collider2D c in colliders)
        {
            Debug.LogError("check");
            IGetHit h = c.GetComponent<IGetHit>();
            if (h != null && c.gameObject.CompareTag("Enemy") && c.gameObject != target)
            {
                Debug.Log("Bomb hit: " + c.gameObject.name);
                h.GetHit(damage / 2);
            }
        }
    }
}
