using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    [SerializeField] protected float speed, lifeTime;
    protected Rigidbody2D rb;

    protected Vector2 movement = Vector2.zero;
    public void Init(Vector2 movement)
    {
        this.movement = movement;
    }

    public void Init(Vector2 movement, float lifeTime)
    {
        this.movement = movement;
        this.lifeTime = lifeTime;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        this.lifeTime -= Time.deltaTime;
        if (this.lifeTime < 0)
        {
            PoolingManager.Despawn(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = movement * speed;
    }

    protected virtual void Hit(GameObject target)
    {
        PoolingManager.Despawn(this.gameObject);
        IGetHit isCanGetHit = target.gameObject.GetComponent<IGetHit>();

        if (isCanGetHit != null)
        {
            target.GetComponent<IGetHit>().GetHit(GamePlayManager.Instance.PlayerController.AttackStat);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BulletBase>() != null) return;
        else if (collision.gameObject.CompareTag("Bound")) return;
        this.Hit(collision.gameObject);
    }
}
