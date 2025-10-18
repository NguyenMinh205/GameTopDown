using System.Collections;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>, IGetHit
{
    [Header("Stats")]
    [SerializeField] private float maxHP;
    [SerializeField] private float armor;       // Max Armor
    [SerializeField] private float attackStat;
    [SerializeField] private float speed;
    [SerializeField] private float speedRotate;

    private float curHP;
    private float curArmor;

    public float MaxHP => maxHP;
    public float MaxArmor => armor;
    public float CurHP => curHP;
    public float CurArmor => curArmor;
    public float AttackStat { get => attackStat; set => attackStat = value; }

    private Rigidbody2D playerRB;

    [Header("Shield Regen")]
    [SerializeField] private float shieldRegenDelay = 5f;
    [SerializeField] private float shieldRegenPercentPerSec = 0.05f;
    [SerializeField] private float regenTick = 0.1f;

    private Coroutine delayCo, regenCo;
    private int regenVersion = 0;

    public void Init()
    {
        playerRB = GetComponent<Rigidbody2D>(); 
        this.maxHP = DataManager.Instance.GameData.Hp; 
        this.armor = DataManager.Instance.GameData.Shield; 
        this.attackStat = DataManager.Instance.GameData.AttackStat; 
        curHP = maxHP; 
        curArmor = armor;
        regenVersion = 0;
        ScheduleRegen();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
            this.transform.Rotate(0, 0, speedRotate * Time.deltaTime);
        else if (Input.GetKey(KeyCode.D))
            this.transform.Rotate(0, 0, -speedRotate * Time.deltaTime);

        if (Input.GetKey(KeyCode.W))
            this.playerRB.velocity = this.transform.up * speed;
        else if (Input.GetKey(KeyCode.S))
            this.playerRB.velocity = -this.transform.up * speed;
        else
            this.playerRB.velocity = Vector2.zero;
    }

    public void GetHit(float dmg)
    {
        CancelRegen();

        float remain = dmg;

        if (curArmor > 0f)
        {
            float absorbed = Mathf.Min(curArmor, remain);
            curArmor -= absorbed;
            remain -= absorbed;
        }

        if (remain > 0f)
            curHP -= remain;

        GameUIController.Instance?.UpdateBar();

        if (this.curHP <= 0f)
        {
            PoolingManager.Despawn(this.gameObject);
            return;
        }

        ScheduleRegen();
    }

    private void ScheduleRegen()
    {
        regenVersion++;
        if (delayCo != null) StopCoroutine(delayCo);
        delayCo = StartCoroutine(DelayThenStartRegen(regenVersion));
    }

    private void CancelRegen()
    {
        regenVersion++;
        if (delayCo != null) { StopCoroutine(delayCo); delayCo = null; }
        if (regenCo != null) { StopCoroutine(regenCo); regenCo = null; }
    }

    private IEnumerator DelayThenStartRegen(int ver)
    {
        yield return new WaitForSeconds(shieldRegenDelay);

        if (ver != regenVersion) yield break;

        if (curHP <= 0f || curArmor >= armor) yield break;

        regenCo = StartCoroutine(RegenTickLoop(ver));
    }

    private IEnumerator RegenTickLoop(int ver)
    {
        var wait = new WaitForSeconds(regenTick);
        float perTick = armor * shieldRegenPercentPerSec * regenTick;

        while (true)
        {
            if (ver != regenVersion) break;
            if (curHP <= 0f) break;
            if (curArmor >= armor) break;

            curArmor = Mathf.Min(armor, curArmor + perTick);
            GameUIController.Instance?.UpdateShieldBar(curArmor, armor);

            yield return wait;
        }

        regenCo = null;
    }
}
