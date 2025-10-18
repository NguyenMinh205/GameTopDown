using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : Singleton<DataManager>
{
    public GameData GameData { get; private set; }

    protected override void Awake()
    {
        base.KeepActive(true);
        base.Awake();
        GameData = new GameData();
        GameData.Load();
    }

    private void OnApplicationQuit()
    {
        GameData.Save();
    }
}

[System.Serializable]
public class GameData
{
    [SerializeField] private bool isFirstTimePlay;

    //Audio
    [SerializeField] private float volumeMusic;
    [SerializeField] private float volumeSFX;

    //Tank Stats
    [SerializeField] private float hp;
    [SerializeField] private float shield;
    [SerializeField] private float attackStat;
    [SerializeField] private int skinIndex;

    //Buffs
    [SerializeField] private int coin;
    [SerializeField] private int numOfBuff1;
    [SerializeField] private int numOfBuff2;
    [SerializeField] private int numOfBuff3;

    //Upgrade Price
    [SerializeField] private int _priceUpHP;
    [SerializeField] private int _priceUpShield;
    [SerializeField] private int _priceUpAttackStat;
    [SerializeField] private int _hpUpgradeValue;
    [SerializeField] private int _shieldUpgradeValue;
    [SerializeField] private int _attackStatUpgradeValue;

    #region GETTER/SETTER
    public bool IsFirstTimePlay { get => isFirstTimePlay; set => isFirstTimePlay = value; }
    public float VolumeMusic { get => volumeMusic; set => volumeMusic = value; }
    public float VolumeSFX { get => volumeSFX; set => volumeSFX = value; }
    public float Hp { get => hp; set => hp = value; }
    public float Shield { get => shield; set => shield = value; }
    public float AttackStat { get => attackStat; set => attackStat = value; }
    public int SkinIndex { get => skinIndex; set => skinIndex = value; }
    public int Coin { get => coin; set => coin = value; }
    public int NumOfBuff1 { get => numOfBuff1; set => numOfBuff1 = value; }
    public int NumOfBuff2 { get => numOfBuff2; set => numOfBuff2 = value; }
    public int NumOfBuff3 { get => numOfBuff3; set => numOfBuff3 = value; }
    public int PriceUpHP { get => _priceUpHP; set => _priceUpHP = value; }
    public int PriceUpShield { get => _priceUpShield; set => _priceUpShield = value; }
    public int PriceUpAttackStat { get => _priceUpAttackStat; set => _priceUpAttackStat = value; }
    public int HpUpgradeValue { get => _hpUpgradeValue; set => _hpUpgradeValue = value; }
    public int ShieldUpgradeValue { get => _shieldUpgradeValue; set => _shieldUpgradeValue = value; }
    public int AttackStatUpgradeValue { get => _attackStatUpgradeValue; set => _attackStatUpgradeValue = value; }
    #endregion

    #region CONST VALUE
    private const float defaultVolume = 0.5f;
    private const float defaultSound = 0.5f;
    private const float defaultHP = 100;
    private const float defaultShield = 50;
    private const float defaultAttack = 3;
    private const int defaultCoin = 100;
    private const int defaultNumOfBuff1 = 0;
    private const int defaultNumOfBuff2 = 0;
    private const int defaultNumOfBuff3 = 0;
    private const int defaultUpgradePrice = 100;
    private const int defaultUpgradeHpValue = 100;
    private const int defaultUpgradeShieldValue = 20;
    private const int defaultUpgradeAttackStatValue = 1;
    public int DefaultUpgradePrice => defaultUpgradePrice;
    private const int hpUpgradeIncreaseValue = 50;
    public int HpUpgradeIncreaseValue => hpUpgradeIncreaseValue;
    private const int shieldUpgradeIncreaseValue = 10;
    public int ShieldUpgradeIncreaseValue => shieldUpgradeIncreaseValue;
    private const int attackStatUpgradeIncreaseValue = 1;
    public int AttackStatUpgradeIncreaseValue => attackStatUpgradeIncreaseValue;
    #endregion

    #region SAVE/LOAD DATA
    public void Load()
    {
        isFirstTimePlay = PlayerPrefs.GetInt("isFirstTimePlay", 1) == 1 ? true : false;
        volumeMusic = PlayerPrefs.GetFloat("volumeMusic", defaultVolume);
        volumeSFX = PlayerPrefs.GetFloat("volumeSFX", defaultSound);
        hp = PlayerPrefs.GetFloat("hp", defaultHP);
        shield = PlayerPrefs.GetFloat("shield", defaultShield);
        attackStat = PlayerPrefs.GetFloat("attackStat", defaultAttack);
        coin = PlayerPrefs.GetInt("coin", defaultCoin);
        numOfBuff1 = PlayerPrefs.GetInt("numOfBuff1", defaultNumOfBuff1);
        numOfBuff2 = PlayerPrefs.GetInt("numOfBuff2", defaultNumOfBuff2);
        numOfBuff3 = PlayerPrefs.GetInt("numOfBuff3", defaultNumOfBuff3);
        _priceUpHP = PlayerPrefs.GetInt("priceUpHP", defaultUpgradePrice);
        _priceUpShield = PlayerPrefs.GetInt("priceUpShield", defaultUpgradePrice);
        _priceUpAttackStat = PlayerPrefs.GetInt("priceUpAttackStat", defaultUpgradePrice);
        _hpUpgradeValue = PlayerPrefs.GetInt("HpUpgradeValue", defaultUpgradeHpValue);
        _shieldUpgradeValue = PlayerPrefs.GetInt("ShieldUpgradeValue", defaultUpgradeShieldValue);
        _attackStatUpgradeValue = PlayerPrefs.GetInt("AttackStatUpgradeValue", defaultUpgradeAttackStatValue);
    }

    public void Save()
    {
        PlayerPrefs.SetInt("isFirstTimePlay", isFirstTimePlay ? 1 : 0);
        PlayerPrefs.SetFloat("volumeMusic", volumeMusic);
        PlayerPrefs.SetFloat("volumeSFX", volumeSFX);
        PlayerPrefs.SetFloat("hp", hp);
        PlayerPrefs.SetFloat("shield", shield);
        PlayerPrefs.SetFloat("attackStat", attackStat);
        PlayerPrefs.SetInt("coin", coin);
        PlayerPrefs.SetInt("numOfBuff1", numOfBuff1);
        PlayerPrefs.SetInt("numOfBuff2", numOfBuff2);
        PlayerPrefs.SetInt("numOfBuff3", numOfBuff3);
        PlayerPrefs.SetInt("priceUpHP", _priceUpHP);
        PlayerPrefs.SetInt("priceUpShield", _priceUpShield);
        PlayerPrefs.SetInt("priceUpAttackStat", _priceUpAttackStat);
        PlayerPrefs.SetInt("HpUpgradeValue", _hpUpgradeValue);
        PlayerPrefs.SetInt("ShieldUpgradeValue", _shieldUpgradeValue);
        PlayerPrefs.SetInt("AttackStatUpgradeValue", _attackStatUpgradeValue);
        PlayerPrefs.Save();
    }
    #endregion
}
