using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleShBarrel : BarrelBase
{
    [SerializeField] private int shotCount = 3;
    private Coroutine shotCoroutine;
    public override void Fire()
    {
        if (timer > 0) return;
        if (shotCoroutine != null)
        {
            StopCoroutine(shotCoroutine);
        }
        shotCoroutine = StartCoroutine(ShotBullet());
        timer = attackCoolDown;
    }

    private IEnumerator ShotBullet()
    {
        for (int i = 1; i <= shotCount; i++)
        {
            Quaternion baseAngle = barrelParent.transform.rotation * Quaternion.Euler(0, 0, 180);
            BulletBase bullet = PoolingManager.Spawn<BulletBase>(bulletPrefab, transform.position, baseAngle, objectPool.transform);
            bullet.Init(-barrel.transform.up, bulletTime, damage);
            yield return new WaitForSeconds(0.1f);
        }
    }    
}
