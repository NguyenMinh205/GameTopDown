using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BarrelBase : MonoBehaviour
{
    protected GameObject barrel;
    [SerializeField] protected GameObject barrelParent;
    [SerializeField] protected GameObject objectPool;
    [SerializeField] protected BulletBase bulletPrefab;
    [SerializeField] protected float bulletSpeed, bulletTime, attackCoolDown, damage;
    protected float timer;

    public float AttackCoolDown { get { return attackCoolDown; } set { attackCoolDown = value; } }

    public float Damage { get { return damage; } set { damage = value; } }

    public void Init(GameObject barrel)
    {
        this.barrel = barrel;
        timer = 0;
    }

    private void FixedUpdate()
    {
        timer = Mathf.Max(0f, timer - Time.deltaTime);
    }


    public abstract void Fire();
}
