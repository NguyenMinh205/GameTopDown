using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IGetHit
{
    private float HP; //Máu của enemy
    private float armor; //Giáp của enemy
    //[SerializeField] private float _detectTargetRadius; //Phạm vi phát hiện player
    private Rigidbody2D rb;
    private Vector2 _movement;

    [SerializeField] private Transform _player;
    [SerializeField] private float speed;
    [SerializeField] private float dmgExplosion;

    public void Init(float HP, float armor)
    {
        this.HP = HP;
        this.armor = armor;
    }    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _player = PlayerController.Instance.transform;
        //_player = PlayerController.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, _detectTargetRadius, _playerMask);
        //foreach (Collider2D collider in colliders)
        //{
        //    _player = collider.transform;
        //}
        //if (DetectObstacle())
        //{
        //    _player = null;
        //}     
    }

    private void FixedUpdate()
    {
        this.rb.velocity = _movement * speed;
    }

    //bool DetectObstacle()
    //{
    //    if (_player == null)
    //    {
    //        return true;
    //    }

    //    Vector2 target = _player.position - this.transform.position;
    //    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, target, target.magnitude);

    //    if (hit.collider == null)
    //        return false;
    //    if (hit.collider.CompareTag("Player"))
    //        return false;

    //    return true;
    //}    
    void FollowPlayer()
    {
        if (_player == null)
        {
            _movement = Vector2.zero;
            return;
        }

        if (_player != null)
        {
            this._movement = (_player.position - this.transform.position).normalized;
            Rotate();
            //if (Vector2.Distance(_player.position, this.transform.position) > _detectTargetRadius)
            //{
            //    _player = null;
            //}
        }
    }

    void Rotate()
    { 
        Vector3 direction = (_player.transform.position - this.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        this.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }    

    public void GetHit(float dmg)
    {
        if (dmg - armor > 0)
        {
            this.HP -= dmg - armor;
            armor = 0;
        }
        else
        {
            armor -= dmg;
        }

        if (this.HP <= 0)
        {
            Debug.Log("Enemy die");
            PoolingManager.Despawn(this.gameObject);
        }    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IGetHit isCanGetHit = collision.gameObject.GetComponent<IGetHit>();

        if (isCanGetHit == null || collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        isCanGetHit.GetHit(dmgExplosion);
        PoolingManager.Despawn(this.gameObject);
    }
}
