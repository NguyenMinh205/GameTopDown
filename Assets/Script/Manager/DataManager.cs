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

public class GameData
{
    [SerializeField] private bool isFirstTimePlay;

    //Audio
    [SerializeField] private float volumeMusic;
    [SerializeField] private float volumeSFX;

    //Player Stats
    [SerializeField] private float hp;
    [SerializeField] private float shield;
    [SerializeField] private float attackStat;

    //Buffs
    [SerializeField] private int coin;
    [SerializeField] private int numOfBuff1;
    [SerializeField] private int numOfBuff2;
    [SerializeField] private int numOfBuff3;

    public bool IsFirstTimePlay { get => isFirstTimePlay; set => isFirstTimePlay = value; }
    public float VolumeMusic { get => volumeMusic; set => volumeMusic = value; }
    public float VolumeSFX { get => volumeSFX; set => volumeSFX = value; }
    public float Hp { get => hp; set => hp = value; }
    public float Shield { get => shield; set => shield = value; }
    public float AttackStat { get => attackStat; set => attackStat = value; }
    public int Coin { get => coin; set => coin = value; }
    public int NumOfBuff1 { get => numOfBuff1; set => numOfBuff1 = value; }
    public int NumOfBuff2 { get => numOfBuff2; set => numOfBuff2 = value; }
    public int NumOfBuff3 { get => numOfBuff3; set => numOfBuff3 = value; }

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
        PlayerPrefs.Save();
    }
    #endregion
}
