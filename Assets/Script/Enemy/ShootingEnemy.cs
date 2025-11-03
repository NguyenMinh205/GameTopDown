using System.Collections;
using UnityEngine;

public abstract class ShootingEnemy : EnemyBase
{
    [Header("Shooting Settings")]
    [SerializeField] protected Transform barrel;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected float attackRange = 5f;
    [SerializeField] protected float bulletLifetime = 2f;
    [SerializeField] protected float burstDelay = 0.2f;

    protected bool _inAttackRange = false;
    protected bool _wasInAttackRange = false;
    protected float _shootTimer = 0f;

    public override void Init(float hp, float armor, float dmgExplosion, float speed, float dmg, float rateOfFire, int coinVal)
    {
        base.Init(hp, armor, dmgExplosion, speed, dmg, rateOfFire, coinVal);
        _shootTimer = 0f;
        _inAttackRange = false;
        _wasInAttackRange = false;
    }

    protected override void Update()
    {
        base.Update();

        if (GamePlayManager.Instance.IsChoosingReward || GamePlayManager.Instance.IsGamePaused || _player == null)
            return;

        float distSqr = (transform.position - _player.position).sqrMagnitude;
        float rangeSqr = attackRange * attackRange;
        _wasInAttackRange = _inAttackRange;
        _inAttackRange = distSqr <= rangeSqr;

        if (_wasInAttackRange && !_inAttackRange)
        {
            _shootTimer = 0f;
        }

        if (_inAttackRange)
        {
            _movement = Vector2.zero;
            RotateBarrelSmooth();
            _shootTimer += Time.deltaTime;
            TryShoot();
        }
        else
        {
            FollowPlayer();
            RotateBarrelSmooth();
        }
    }

    private void TryShoot()
    {
        if (_shootTimer >= _rateOfFire)
        {
            StartCoroutine(ShootBurst());
            _shootTimer = 0f;
        }
    }

    protected abstract IEnumerator ShootBurst();

    protected void ShootSingle()
    {
        if (_player == null || barrel == null || bulletPrefab == null) return;

        Vector2 direction = (_player.position - barrel.position).normalized;
        Quaternion angle = barrel.transform.rotation * Quaternion.Euler(0, 0, 180);

        GameObject bulletObj = PoolingManager.Spawn(bulletPrefab, barrel.position, angle, GamePlayManager.Instance.EnemyManager.ObjectPool.transform);
        BulletBase bullet = bulletObj.GetComponent<BulletBase>();
        if (bullet != null)
        {
            bullet.Init(direction, bulletLifetime);
        }
    }

    private void RotateBarrelSmooth()
    {
        if (barrel == null || _player == null) return;

        Vector3 dir = _player.position - barrel.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
        barrel.rotation = Quaternion.Euler(0, 0, angle);
    }
}