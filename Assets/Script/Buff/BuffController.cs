using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    [SerializeField] private float buff1Cooldown = 7f;
    [SerializeField] private float buff2Cooldown = 10f;
    [SerializeField] private float buff3Cooldown = 15f;

    private bool isBuff1OnCooldown = false;
    private bool isBuff2OnCooldown = false;
    private bool isBuff3OnCooldown = false;

    private int _numOfBuff1;
    public int NumOfBuff1 => _numOfBuff1;
    private int _numOfBuff2;
    public int NumOfBuff2 => _numOfBuff2;
    private int _numOfBuff3;
    public int NumOfBuff3 => _numOfBuff3;

    private Coroutine cooldownCoroutine1;
    private Coroutine cooldownCoroutine2;
    private Coroutine cooldownCoroutine3;
    private Coroutine resetBuff2Coroutine;

    public void Init()
    {
        _numOfBuff1 = DataManager.Instance.GameData.NumOfBuff1;
        _numOfBuff2 = DataManager.Instance.GameData.NumOfBuff2;
        _numOfBuff3 = DataManager.Instance.GameData.NumOfBuff3;
    }

    public void UseBuff1()
    {
        if (isBuff1OnCooldown || _numOfBuff1 <= 0) return;

        AudioManager.Instance.PlayUseBuffSound();
        float maxHP = GamePlayManager.Instance.PlayerController.MaxHP;
        GamePlayManager.Instance.PlayerController.Heal(maxHP * 0.15f);
        _numOfBuff1--;
        DataManager.Instance.GameData.NumOfBuff1 = _numOfBuff1;
        GameUIController.Instance.UpdateNumOfBuff(_numOfBuff1, BuffType.Buff1);

        if (cooldownCoroutine1 != null)
        {
            StopCoroutine(cooldownCoroutine1);
        }
        cooldownCoroutine1 = StartCoroutine(CooldownBuff(BuffType.Buff1));
    }

    public void UseBuff2()
    {
        if (isBuff2OnCooldown || _numOfBuff2 <= 0) return;

        AudioManager.Instance.PlayUseBuffSound();
        BarrelBase barrel = GamePlayManager.Instance.PlayerController.BarrelController.CurTypeOfBarrel;
        if (barrel != null)
        {
            float originalDamage = GamePlayManager.Instance.PlayerController.AttackStat;
            float originalCooldown = barrel.AttackCoolDown;
            barrel.AttackCoolDown *= 0.5f;
            _numOfBuff2--;
            DataManager.Instance.GameData.NumOfBuff2 = _numOfBuff2;
            GameUIController.Instance.UpdateNumOfBuff(_numOfBuff2, BuffType.Buff2);

            if (cooldownCoroutine2 != null)
            {
                StopCoroutine(cooldownCoroutine2);
            }
            if (resetBuff2Coroutine != null)
            {
                StopCoroutine(resetBuff2Coroutine);
            }
            cooldownCoroutine2 = StartCoroutine(CooldownBuff(BuffType.Buff2));
            resetBuff2Coroutine = StartCoroutine(ResetBuff2(barrel, originalDamage, originalCooldown));
        }
    }

    private IEnumerator ResetBuff2(BarrelBase barrel, float originalDamage, float originalCooldown)
    {
        yield return new WaitForSeconds(5f);
        GamePlayManager.Instance.PlayerController.AttackStat = originalDamage;
        barrel.AttackCoolDown = originalCooldown;
        resetBuff2Coroutine = null;
    }

    public void UseBuff3()
    {
        if (isBuff3OnCooldown || _numOfBuff3 <= 0) return;

        AudioManager.Instance.PlayUseBuffSound();
        StartCoroutine(Invincibility(5f));
        _numOfBuff3--;
        DataManager.Instance.GameData.NumOfBuff3 = _numOfBuff3;
        GameUIController.Instance.UpdateNumOfBuff(_numOfBuff3, BuffType.Buff3);

        if (cooldownCoroutine3 != null)
        {
            StopCoroutine(cooldownCoroutine3);
        }
        cooldownCoroutine3 = StartCoroutine(CooldownBuff(BuffType.Buff3));
    }

    private IEnumerator Invincibility(float duration)
    {
        GamePlayManager.Instance.PlayerController.SetInvincible(true);
        yield return new WaitForSeconds(duration);
        GamePlayManager.Instance.PlayerController.SetInvincible(false);
    }

    private IEnumerator CooldownBuff(BuffType type)
    {
        float cooldown = GetCooldown(type);
        SetCooldownFlag(type, true);
        GameUIController.Instance.StartBuffCooldownUI(type, cooldown);
        yield return new WaitForSeconds(cooldown);
        SetCooldownFlag(type, false);
        switch (type)
        {
            case BuffType.Buff1: cooldownCoroutine1 = null; break;
            case BuffType.Buff2: cooldownCoroutine2 = null; break;
            case BuffType.Buff3: cooldownCoroutine3 = null; break;
        }
    }

    private float GetCooldown(BuffType type)
    {
        switch (type)
        {
            case BuffType.Buff1: return buff1Cooldown;
            case BuffType.Buff2: return buff2Cooldown;
            case BuffType.Buff3: return buff3Cooldown;
            default: return 0f;
        }
    }

    private void SetCooldownFlag(BuffType type, bool value)
    {
        switch (type)
        {
            case BuffType.Buff1: isBuff1OnCooldown = value; break;
            case BuffType.Buff2: isBuff2OnCooldown = value; break;
            case BuffType.Buff3: isBuff3OnCooldown = value; break;
        }
    }

    public void StopAllCoroutine()
    {
        if (cooldownCoroutine1 != null)
        {
            StopCoroutine(cooldownCoroutine1);
            cooldownCoroutine1 = null;
        }
        if (cooldownCoroutine2 != null)
        {
            StopCoroutine(cooldownCoroutine2);
            cooldownCoroutine2 = null;
        }
        if (cooldownCoroutine3 != null)
        {
            StopCoroutine(cooldownCoroutine3);
            cooldownCoroutine3 = null;
        }
        if (resetBuff2Coroutine != null)
        {
            StopCoroutine(resetBuff2Coroutine);
            resetBuff2Coroutine = null;
        }
    }    
}