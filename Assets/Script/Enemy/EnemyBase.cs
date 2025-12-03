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
    [SerializeField] private float avoidanceAngle = 25f;
    [SerializeField] private float detectionDistance = 2f;
    [SerializeField] private float avoidanceSmoothing = 8f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private int numAvoidanceRays = 7;
    [SerializeField] private float separationRadius = 1.5f;
    [SerializeField] private float separationWeight = 1f;

    private bool _isAvoiding = false;
    private Vector2 _targetDirection;
    private float _raycastTimer = 0f;
    [SerializeField] private float raycastInterval = 0.1f;

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
        if (GamePlayManager.Instance.IsChoosingReward || GamePlayManager.Instance.IsGamePaused || GamePlayManager.Instance.IsInstructionShown)
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
        _targetDirection = toPlayer;

        _raycastTimer += Time.deltaTime;
        if (_raycastTimer >= raycastInterval)
        {
            _raycastTimer = 0f;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, detectionDistance, obstacleLayer);
            if (hit.collider != null && !_isAvoiding)
            {
                StartAvoidance(hit);
            }

            if (_isAvoiding)
            {
                if (!Physics2D.Raycast(transform.position, _targetDirection, detectionDistance, obstacleLayer))
                {
                    _isAvoiding = false;
                }
            }
            else
            {
                _targetDirection = toPlayer;
            }
        }

        Vector2 separationForce = CalculateSeparation();
        _targetDirection += separationForce * separationWeight;
        _targetDirection = _targetDirection.normalized;

        _movement = Vector2.Lerp(_movement, _targetDirection, avoidanceSmoothing * Time.deltaTime);

        RotateTowards();
    }

    private Vector2 CalculateSeparation()
    {
        Vector2 steer = Vector2.zero;
        int count = 0;

        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, separationRadius, obstacleLayer);
        foreach (Collider2D neighbor in neighbors)
        {
            if (neighbor.gameObject != gameObject && neighbor.CompareTag("Enemy"))
            {
                Vector2 diff = transform.position - neighbor.transform.position;
                steer += diff.normalized / diff.magnitude;
                count++;
            }
        }

        if (count > 0)
        {
            steer /= count;
        }

        return steer;
    }

    public void RotateTowards()
    {
        if (_player == null) return;

        float angle = Mathf.Atan2(_movement.y, _movement.x) * Mathf.Rad2Deg + 90f;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void StartAvoidance(RaycastHit2D forwardHit)
    {
        _isAvoiding = true;

        Vector2 forward = transform.up.normalized;
        Vector2 bestDir = forward;
        float bestScore = float.MinValue;

        int raySteps = (numAvoidanceRays - 1) / 2;
        for (int i = -raySteps; i <= raySteps; i++)
        {
            Vector2 dir = Quaternion.Euler(0, 0, avoidanceAngle * i) * forward;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionDistance * 2, obstacleLayer);
            float distance = hit.collider ? hit.distance : Mathf.Infinity;
            Vector2 toPlayer = (_player.position - transform.position).normalized;
            float score = Vector2.Dot(dir, toPlayer) * 1.2f + (distance / (detectionDistance * 2));
            if (score > bestScore)
            {
                bestScore = score;
                bestDir = dir;
            }
        }

        // Thêm repulsion dựa trên normal của forward hit để đẩy mạnh hơn khỏi biên/tường
        if (forwardHit.collider != null)
        {
            bestDir += forwardHit.normal * 0.8f; // Tăng lực đẩy để thoát kẹt nhanh hơn
        }

        _targetDirection = bestDir.normalized;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0 && collision.gameObject != gameObject && !collision.gameObject.CompareTag("Player"))
        {
            if (!_isAvoiding)
            {
                // Lấy normal từ collision để đẩy ngay lập tức
                Vector2 repulsion = collision.contacts[0].normal * 1f;
                _targetDirection += repulsion;
                _targetDirection.Normalize();
                StartAvoidance(new RaycastHit2D()); // Gọi avoidance với empty hit
            }
        }
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
        PoolingManager.Despawn(gameObject);
        AudioManager.Instance.PlayEnemyDieSound();
        GamePlayManager.Instance.SpawnExplosionTankAnim(transform.position);
        DataManager.Instance.GameData.Coin += _coinValue;
        GameUIController.Instance.UpdateCoin(DataManager.Instance.GameData.Coin);
        ObserverManager<GameState>.PostEvent(GameState.OnEnemyDie, this);
        ObserverManager<AchievementEvents>.PostEvent(AchievementEvents.OnEnemyKilled, 1);
    }
}

public enum EnemyType { Boom, Shooter, Boss }