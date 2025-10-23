using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IGetHit
{
    protected float _hp;
    protected float _armor;
    protected float _dmgExplosion = 10f;
    protected float _speed;
    protected float _damage;
    protected float _rateOfFire;
    protected int _coinValue;

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Transform _player;

    [SerializeField] private EnemyType _enemyType;
    public EnemyType EnemyType => _enemyType;

    public void Init(float hp, float armor, float dmpExplosion, float speed, float dmg, float rateOfFire, int coinVal)
    {
        this._hp = hp;
        this._armor = armor;
        this._dmgExplosion = dmpExplosion;
        this._speed = speed;
        this._damage = dmg;
        this._rateOfFire = rateOfFire;
        this._coinValue = coinVal;
        _rb = GetComponent<Rigidbody2D>();
        _player = PlayerController.Instance.transform;
    }

    protected virtual void Update()
    {
        FollowPlayer();
    }

    protected virtual void FixedUpdate()
    {
        if (_rb != null)
            _rb.velocity = _movement * _speed;
    }

    public virtual void FollowPlayer()
    {
        if (_player == null)
        {
            _movement = Vector2.zero;
            return;
        }

        this._movement = (_player.position - this.transform.position).normalized;
        RotateTowards();
    }

    public void RotateTowards()
    {
        Vector3 direction = (_player.transform.position - this.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected void RotateBarrelTowards(Transform t)
    {
        Vector3 dir = (_player.transform.position - t.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
        t.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void GetHit(float dmg)
    {
        if (dmg - _armor > 0)
        {
            this._hp -= dmg - _armor;
            _armor = 0;
        }
        else
        {
            _armor -= dmg;
        }

        if (this._hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Enemy die");
        PoolingManager.Despawn(this.gameObject);
        DataManager.Instance.GameData.Coin += _coinValue;
        GameUIController.Instance.UpdateCoin(DataManager.Instance.GameData.Coin);
        ObserverManager<GameState>.PostEvent(GameState.OnEnemyDie, this);
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

    protected virtual void Explode(IGetHit getHit)
    {
        getHit.GetHit(_dmgExplosion);
    }
}

public enum EnemyType
{
    Boom,
    Shooter,
    Boss,
}
