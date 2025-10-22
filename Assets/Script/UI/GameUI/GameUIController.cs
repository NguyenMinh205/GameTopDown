using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : Singleton<GameUIController>
{
    [SerializeField] private Image _hpBar;
    [SerializeField] private TextMeshProUGUI _hpVal;

    [SerializeField] private Image _shieldBar;
    [SerializeField] private TextMeshProUGUI _shieldVal;

    [SerializeField] private GameObject settingPopup;
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _coinText;

    [SerializeField] private TextMeshProUGUI _numOfBuff1;
    [SerializeField] private TextMeshProUGUI _numOfBuff2;
    [SerializeField] private TextMeshProUGUI _numOfBuff3;

    [SerializeField] private Image _cooldownBuff1;
    [SerializeField] private Image _cooldownBuff2;
    [SerializeField] private Image _cooldownBuff3;

    private PlayerController curPlayer;

    private Coroutine cooldownCoroutine;

    public void SetupStartGame()
    {
        curPlayer = GamePlayManager.Instance.PlayerController;
        UpdateBar();
        UpdateCoin(DataManager.Instance.GameData.Coin);
        UpdateNumOfBuff(DataManager.Instance.GameData.NumOfBuff1, BuffType.Buff1);
        UpdateNumOfBuff(DataManager.Instance.GameData.NumOfBuff2, BuffType.Buff2);
        UpdateNumOfBuff(DataManager.Instance.GameData.NumOfBuff3, BuffType.Buff3);
    }    

    public void ShowWaveText(int wave)
    {
        _waveText.text = $"WAVE {wave}";
    }

    public void UpdateBar()
    {
        UpdateHPBar(curPlayer.CurHP, curPlayer.MaxHP);
        UpdateShieldBar(curPlayer.CurArmor, curPlayer.MaxArmor);
    }

    public void UpdateHPBar(float currentHP, float maxHP)
    {
        _hpBar.fillAmount = currentHP / maxHP;
        _hpVal.text = $"{currentHP}/{maxHP}";
    }

    public void UpdateShieldBar(float currentShield, float maxShield)
    {
        _shieldBar.fillAmount = currentShield / maxShield;
        _shieldVal.text = $"{currentShield}/{maxShield}";
    }

    public void UpdateCoin(int coin)
    {
        _coinText.text = coin.ToString();
    }

    public void UpdateNumOfBuff(int buff, BuffType buffType)
    {
        if (buffType == BuffType.Buff1)
        {
            _numOfBuff1.text = buff.ToString();
        }
        else if (buffType == BuffType.Buff2)
        {
            _numOfBuff2.text = buff.ToString();
        }
        else if (buffType == BuffType.Buff3)
        {
            _numOfBuff3.text = buff.ToString();
        }
    }

    public void StartBuffCooldownUI(BuffType buffType, float cooldown)
    {
        Image cooldownImage = GetCooldownImage(buffType);
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        if (cooldownImage != null)
        {
            cooldownCoroutine = StartCoroutine(UpdateCooldown(cooldownImage, cooldown));
        }
    }

    private Image GetCooldownImage(BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.Buff1: return _cooldownBuff1;
            case BuffType.Buff2: return _cooldownBuff2;
            case BuffType.Buff3: return _cooldownBuff3;
            default: return null;
        }
    }

    private IEnumerator UpdateCooldown(Image cooldownImage, float cooldown)
    {
        float elapsedTime = 0f;
        cooldownImage.fillAmount = 1f;

        while (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;
            cooldownImage.fillAmount = 1f - (elapsedTime / cooldown);
            yield return null;
        }

        cooldownImage.fillAmount = 0f;
    }

    public void OpenSetting(bool val)
    {
        settingPopup.SetActive(val);
    }
}
