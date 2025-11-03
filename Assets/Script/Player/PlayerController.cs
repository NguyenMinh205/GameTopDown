using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>, IGetHit
{
    [Header("Stats")]
    [SerializeField] private float maxHP;
    [SerializeField] private float maxArmor;
    [SerializeField] private float attackStat;
    [SerializeField] private float speed;
    [SerializeField] private float speedRotate;

    private float curHP;
    private float curArmor;

    public float MaxHP { get => maxHP; set => maxHP = value; }
    public float MaxArmor { get => maxArmor; set => maxArmor = value; }
    public float CurHP { get => curHP; set => curHP = value; }
    public float CurArmor { get => curArmor; set => curArmor = value; }
    public float AttackStat { get => attackStat; set => attackStat = value; }

    private Rigidbody2D playerRB;
    private bool isPaused = false;

    [Header("Shield Regen")]
    [SerializeField] private float shieldRegenDelay = 5f;
    [SerializeField] private float shieldRegenPercentPerSec = 0.05f;
    [SerializeField] private float regenTick = 0.1f;

    private Coroutine delayCo, regenCo;
    private int regenVersion = 0;

    [Header("Barrel")]
    [SerializeField] private BarrelBase defaultBarrel;
    [SerializeField] private BarrelBase tripleShotBarrel;
    [SerializeField] private BarrelBase bombBarrel;
    [SerializeField] private BarrelBase laserBarrel;
    [SerializeField] private BarrelController barrelController;
    public BarrelController BarrelController => barrelController;

    private bool isInvincible = false;

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    public void Init()
    {
        playerRB = GetComponent<Rigidbody2D>(); 
        this.maxHP = DataManager.Instance.GameData.Hp; 
        this.maxArmor = DataManager.Instance.GameData.Shield; 
        this.attackStat = DataManager.Instance.GameData.AttackStat; 
        curHP = maxHP; 
        curArmor = maxArmor;
        regenVersion = 0;
        ScheduleRegen();

        if (barrelController != null)
        {
            barrelController.ChangeTypeOfBarrel(defaultBarrel);
        } 
    }

    private void Update()
    {
        if (GamePlayManager.Instance.IsChoosingReward || GamePlayManager.Instance.IsGamePaused)
        {
            this.playerRB.velocity = Vector2.zero;
            return;
        }

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

    public void PauseRegenShield()
    {
        if (isPaused) return;
        isPaused = true;

        CancelRegen();
    }

    public void ResumeRegenShield()
    {
        if (!isPaused) return;
        isPaused = false;

        ScheduleRegen();
    }


    public void ChangeBarrel(string nameBarrel)
    {
        if (barrelController == null) return;
        if (nameBarrel.Equals("Multiple Shot"))
        {
            barrelController.ChangeTypeOfBarrel(tripleShotBarrel);
        }
        else if (nameBarrel.Equals("Explosive Shot"))
        {
            barrelController.ChangeTypeOfBarrel(bombBarrel);
        }
        else if (nameBarrel.Equals("Beam Cannon"))
        {
            barrelController.ChangeTypeOfBarrel(laserBarrel);
        }
        else
        {
            barrelController.ChangeTypeOfBarrel(defaultBarrel);
        }
    }

    public void Heal(float val)
    {
        curHP += val;
        if (curHP > maxHP) curHP = maxHP;
        GameUIController.Instance?.UpdateBar();
    }

    public void GetHit(float dmg)
    {
        if (isInvincible) return;

        AudioManager.Instance.PlayHitSound();
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
            GamePlayManager.Instance.BuffController.StopAllCoroutine();
            GamePlayManager.Instance.EndGame();
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

        if (curHP <= 0f || curArmor >= maxArmor) yield break;

        regenCo = StartCoroutine(RegenTickLoop(ver));
    }

    private IEnumerator RegenTickLoop(int ver)
    {
        var wait = new WaitForSeconds(regenTick);
        float perTick = maxArmor * shieldRegenPercentPerSec * regenTick;

        while (true)
        {
            if (ver != regenVersion) break;
            if (curHP <= 0f) break;
            if (curArmor >= maxArmor) break;

            curArmor = Mathf.Min(maxArmor, curArmor + perTick);
            GameUIController.Instance?.UpdateShieldBar(curArmor, maxArmor);

            yield return wait;
        }

        regenCo = null;
    }

    //public void ModifyAttackSpeed(float multiplier, float duration)
    //{
    //    if (barrelController != null && barrelController.CurTypeOfBarrel != null)
    //    {
    //        StartCoroutine(TempModifyAttackSpeed(multiplier, duration));
    //    }
    //}

    //private IEnumerator TempModifyAttackSpeed(float multiplier, float duration)
    //{
    //    float originalCooldown = barrelController.CurTypeOfBarrel.AttackCoolDown;
    //    barrelController.CurTypeOfBarrel.AttackCoolDown /= multiplier;
    //    yield return new WaitForSeconds(duration);
    //    barrelController.CurTypeOfBarrel.AttackCoolDown = originalCooldown;
    //}

    //public void ModifyDamage(float multiplier, float duration)
    //{
    //    if (barrelController != null && barrelController.CurTypeOfBarrel != null)
    //    {
    //        StartCoroutine(TempModifyDamage(multiplier, duration));
    //    }
    //}

    //private IEnumerator TempModifyDamage(float multiplier, float duration)
    //{
    //    float originalDamage = barrelController.CurTypeOfBarrel.Damage;
    //    float originalAttackStat = this.AttackStat;
    //    barrelController.CurTypeOfBarrel.Damage *= multiplier;
    //    this.AttackStat *= multiplier;
    //    yield return new WaitForSeconds(duration);
    //    barrelController.CurTypeOfBarrel.Damage = originalDamage;
    //    this.AttackStat = originalAttackStat;
    //}
}
