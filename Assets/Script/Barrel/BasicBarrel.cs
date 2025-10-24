using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBarrel : BarrelBase
{
    public override void Fire()
    {
        if (timer > 0 || GamePlayManager.Instance.IsChoosingReward) return;

        Quaternion angle = barrelParent.transform.rotation * Quaternion.Euler(0, 0, 180);

        BulletBase bullet = PoolingManager.Spawn<BulletBase>(bulletPrefab, transform.position, angle, objectPool.transform);
        bullet.Init(-barrel.transform.up, bulletTime);

        timer = attackCoolDown;
    }

    public void ChangeBullet (BulletBase bullet)
    {
        this.bulletPrefab = bullet;
    }
}
