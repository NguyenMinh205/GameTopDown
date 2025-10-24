using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBullet : BulletBase
{
    [SerializeField] private float explosionRadius = 1f;
    protected override void Hit(GameObject target)
    {
        base.Hit(target);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D c in colliders)
        {
            Debug.LogError("check");
            IGetHit h = c.GetComponent<IGetHit>();
            if (h != null && c.gameObject.CompareTag("Enemy") && c.gameObject != target)
            {
                h.GetHit(GamePlayManager.Instance.PlayerController.AttackStat / 2);
            }
        }
    }
}
