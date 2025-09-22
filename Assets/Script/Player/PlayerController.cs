using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>, IGetHit
{
    [SerializeField] private float HP;
    [SerializeField] private float armor;
    [SerializeField] private float speed;
    [SerializeField] private float speedRotate;

    private Rigidbody2D playerRB;
    private float horizontal;
    private float vertical;

    public void Init()
    {

    }    

    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Move();
        Rotate();
    }

    private void Move()
    {
        if (vertical > 0)
        {
            this.playerRB.velocity = this.transform.up * -speed;
        } 
        else if (vertical < 0)
        {
            this.playerRB.velocity = this.transform.up * speed;
        } 
        else
        {
            this.playerRB.velocity = Vector2.zero;
        }    
    }   
    
    private void Rotate()
    {
        if (horizontal > 0)
        {
            this.transform.Rotate(new Vector3(0, 0, -speedRotate * Time.deltaTime));
        }
        else if (horizontal < 0)
        {
            this.transform.Rotate(new Vector3(0, 0, speedRotate * Time.deltaTime));
        }
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
            PoolingManager.Despawn(this.gameObject);
        }
    }
}
