using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITankDetail : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hpTxt;
    [SerializeField] private TextMeshProUGUI _shieldTxt;
    [SerializeField] private TextMeshProUGUI _attackStatTxt;

    [Header("Upgrade Max HP")]
    [SerializeField] private Button _upgradeHP;
    [SerializeField] private TextMeshProUGUI _priceUpHPTxt;

    [Header("Upgrade Max Shield")]
    [SerializeField] private TextMeshProUGUI _priceUpShieldTxt;
    [SerializeField] private Button _upgradeShield;

    [Header("Upgrade Attack Stat")]
    [SerializeField] private Button _upgradeAttackStat;
    [SerializeField] private TextMeshProUGUI _priceUpAttackStatTxt;

    [Header("Choose Skin UI")]
    [SerializeField] private Button _leftBtn;
    [SerializeField] private Button _rightBtn;
    [SerializeField] private Image _skinPreview;
    [SerializeField] private List<SkinTank> skinTanks = new List<SkinTank>();

    private void Awake()
    {
        if (_upgradeHP) _upgradeHP.onClick.AddListener(UpgradeHP);
        if (_upgradeShield) _upgradeShield.onClick.AddListener(UpgradeShield);
        if (_upgradeAttackStat) _upgradeAttackStat.onClick.AddListener(UpgradeAttackStat);

        if (_leftBtn) _leftBtn.onClick.AddListener(() => ChangeSkin(-1));
        if (_rightBtn) _rightBtn.onClick.AddListener(() => ChangeSkin(+1));
    }

    private void OnEnable()
    {
        CheckAcceptBuy();
    }

    private void ChangeSkin(int delta)
    {
        if (skinTanks == null || skinTanks.Count == 0) return;
        var gd = DataManager.Instance.GameData;
        gd.SkinIndex = (gd.SkinIndex + delta + skinTanks.Count) % skinTanks.Count;
        ApplySkin(gd.SkinIndex);
    }

    private void ApplySkin(int index)
    {
        if (skinTanks == null || skinTanks.Count == 0) return;
        index = Mathf.Clamp(index, 0, skinTanks.Count - 1);
        SkinTank skin = skinTanks[index];
        if (_skinPreview) _skinPreview.sprite = skin.tank;
    }

    public void UpgradeHP()
    {
        if (DataManager.Instance.GameData.Coin >= DataManager.Instance.GameData.PriceUpHP)
        {
            DataManager.Instance.GameData.Coin -= DataManager.Instance.GameData.PriceUpHP;
            DataManager.Instance.GameData.Hp += DataManager.Instance.GameData.HpUpgradeValue;
            DataManager.Instance.GameData.HpUpgradeValue += DataManager.Instance.GameData.HpUpgradeIncreaseValue;
            DataManager.Instance.GameData.PriceUpHP += DataManager.Instance.GameData.DefaultUpgradePrice;
            _hpTxt.text = DataManager.Instance.GameData.Hp.ToString();
            _priceUpHPTxt.text = DataManager.Instance.GameData.PriceUpHP.ToString();
            AudioManager.Instance.PlayUseCoinSound();
        }
        else
        {
            AudioManager.Instance.PlayErrorSound();
        }
        CheckAcceptBuy();
    }

    public void UpgradeShield()
    {
        if (DataManager.Instance.GameData.Coin >= int.Parse(_priceUpShieldTxt.text))
        {
            DataManager.Instance.GameData.Coin -= int.Parse(_priceUpShieldTxt.text);
            DataManager.Instance.GameData.Shield += DataManager.Instance.GameData.ShieldUpgradeValue;
            DataManager.Instance.GameData.ShieldUpgradeValue += DataManager.Instance.GameData.ShieldUpgradeIncreaseValue;
            DataManager.Instance.GameData.PriceUpShield += DataManager.Instance.GameData.DefaultUpgradePrice;
            _shieldTxt.text = DataManager.Instance.GameData.Shield.ToString();
            _priceUpShieldTxt.text = DataManager.Instance.GameData.PriceUpShield.ToString();
            AudioManager.Instance.PlayUseCoinSound();
        }
        else
        {
            AudioManager.Instance.PlayErrorSound();
        }
        CheckAcceptBuy();
    }

    public void UpgradeAttackStat()
    {
        if (DataManager.Instance.GameData.Coin >= int.Parse(_priceUpAttackStatTxt.text))
        {
            DataManager.Instance.GameData.Coin -= int.Parse(_priceUpAttackStatTxt.text);
            DataManager.Instance.GameData.AttackStat += DataManager.Instance.GameData.AttackStatUpgradeValue;
            DataManager.Instance.GameData.AttackStatUpgradeValue += DataManager.Instance.GameData.AttackStatUpgradeIncreaseValue;
            DataManager.Instance.GameData.PriceUpAttackStat += DataManager.Instance.GameData.DefaultUpgradePrice;
            _attackStatTxt.text = DataManager.Instance.GameData.AttackStat.ToString();
            _priceUpAttackStatTxt.text = DataManager.Instance.GameData.PriceUpAttackStat.ToString();
            AudioManager.Instance.PlayUseCoinSound();
        }
        else
        {
            AudioManager.Instance.PlayErrorSound();
        }
        CheckAcceptBuy();
    }

    public void CheckAcceptBuy()
    {
        GameData gd = DataManager.Instance.GameData;

        _priceUpHPTxt.color = gd.Coin < gd.PriceUpHP ? Color.red : Color.black;
        _priceUpShieldTxt.color = gd.Coin < gd.PriceUpShield ? Color.red : Color.black;
        _priceUpAttackStatTxt.color = gd.Coin < gd.PriceUpAttackStat ? Color.red : Color.black;

        _upgradeHP.interactable = gd.Coin >= gd.PriceUpHP;
        _upgradeShield.interactable = gd.Coin >= gd.PriceUpShield;
        _upgradeAttackStat.interactable = gd.Coin >= gd.PriceUpAttackStat;
    }
}

[System.Serializable]
public class SkinTank
{
    public Sprite tank;
    public Sprite body;
    public Sprite barrel;
}
