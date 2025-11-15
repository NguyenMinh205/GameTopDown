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

    protected Rigidbody2D _rb;
    protected Vector2 _movement;
    protected Transform _player;

    [SerializeField] private EnemyType _enemyType;
    public EnemyType EnemyType => _enemyType;
    [SerializeField] private Animator _animator;

    [Header("Obstacle Avoidance Settings")]
    [SerializeField] private float avoidanceAngle = 25f; // Góc xoay tránh (45 độ)
    [SerializeField] private float avoidanceDuration = 0.5f; // Thời gian đi theo hướng tránh (0.5s)
    [SerializeField] private LayerMask obstacleLayer; // Layer cho obstacle (border + enemy khác)

    private bool _isAvoiding = false;
    private Vector2 _avoidanceDirection;
    private Coroutine _avoidanceCoroutine;

    public virtual void Init(float hp, float armor, float dmpExplosion, float speed, float dmg, float rateOfFire, int coinVal)
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

    protected virtual void FixedUpdate()
    {
        if (GamePlayManager.Instance.IsChoosingReward || GamePlayManager.Instance.IsGamePaused)
        {
            _rb.velocity = Vector2.zero;
            return;
        }

        if (_rb != null)
        {
            _rb.velocity = _movement * _speed;
        }
    }

    public virtual void FollowPlayer()
    {
        if (_player == null)
        {
            _movement = Vector2.zero;
            return;
        }

        Vector2 toPlayer = (_player.position - transform.position).normalized;

        if (_isAvoiding)
        {
            _movement = _avoidanceDirection;
        }
        else
        {
            _movement = toPlayer;
        }

        RotateTowards();
    }

    public void RotateTowards()
    {
        if (_player == null) return;

        float angle = Mathf.Atan2(_movement.y, _movement.x) * Mathf.Rad2Deg + 90f;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayer) == 0 ||
            collision.gameObject == gameObject ||
            collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (!_isAvoiding)
        {
            _avoidanceCoroutine = null;
            _avoidanceCoroutine = StartCoroutine(AvoidanceCoroutine());
        }
    }

    private IEnumerator AvoidanceCoroutine()
    {
        _isAvoiding = true;

        Vector2 forward = transform.up;
        Vector2 toPlayer = (_player.position - transform.position).normalized;
        float angleToPlayer = Vector2.SignedAngle(forward, toPlayer);
        float rotationDirection = -Mathf.Sign(angleToPlayer);

        _avoidanceDirection = Quaternion.Euler(0, 0, avoidanceAngle * rotationDirection) * forward.normalized;

        yield return new WaitForSeconds(avoidanceDuration);

        _isAvoiding = false;
    }

    public void GetHit(float dmg)
    {
        _animator.SetTrigger("IsHit");
        if (dmg - _armor > 0)
        {
            _hp -= dmg - _armor;
            _armor = 0;
        }
        else
        {
            _armor -= dmg;
        }

        if (_hp <= 0) Die();
    }

    public void Die()
    {
        Debug.Log("Enemy die");
        if (_avoidanceCoroutine != null)
        {
            StopCoroutine(_avoidanceCoroutine);
            _avoidanceCoroutine = null;
        }
        PoolingManager.Despawn(gameObject);
        AudioManager.Instance.PlayEnemyDieSound();
        GamePlayManager.Instance.SpawnExplosionTankAnim(transform.position);
        DataManager.Instance.GameData.Coin += _coinValue;
        GameUIController.Instance.UpdateCoin(DataManager.Instance.GameData.Coin);
        ObserverManager<GameState>.PostEvent(GameState.OnEnemyDie, this);
    }
}

public enum EnemyType { Boom, Shooter, Boss }