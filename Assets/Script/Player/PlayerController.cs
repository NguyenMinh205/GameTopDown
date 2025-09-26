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

    public void Init()
    {

    }    

    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Rotate(new Vector3(0, 0, speedRotate * Time.deltaTime));
        } 
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.Rotate(new Vector3(0, 0, -speedRotate * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.W))
        {
            this.playerRB.velocity = this.transform.up * speed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.playerRB.velocity = this.transform.up * -speed;
        }
        else
        {
            this.playerRB.velocity = Vector2.zero;
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
