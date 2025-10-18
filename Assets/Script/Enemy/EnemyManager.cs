using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject objectPool;
    [SerializeField] private List<GameObject> spawnGates = new List<GameObject>();
    [SerializeField] private List<EnemyBase> enemiesPrefabs = new List<EnemyBase>();
    [SerializeField] private List<EnemyInfoSO> enemyInfos = new List<EnemyInfoSO>();

    private int _waveNumber = 1;
    private int _numOfEnemyInCurWave;
    private List<EnemyInfoSO> _cloneEnemyList;

    public void Init()
    {
        _cloneEnemyList = new List<EnemyInfoSO>();
        foreach (EnemyInfoSO info in enemyInfos)
        {
            EnemyInfoSO cloneInfo = ScriptableObject.CreateInstance<EnemyInfoSO>();
            cloneInfo.EnemyType = info.EnemyType;
            cloneInfo.HP = info.HP;
            cloneInfo.Armor = info.Armor;
            cloneInfo.DmgExplosion = info.DmgExplosion;
            cloneInfo.Speed = info.Speed;
            cloneInfo.Dmg = info.Dmg;
            cloneInfo.RateOfFire = info.RateOfFire;
            _cloneEnemyList.Add(cloneInfo);
        }
        ObserverManager<GameState>.AddRegisterEvent(GameState.OnEnemyDie, OnEnemyDie);
    }

    public void SetEnemyCountForWave(int total, int waveNumber)
    {
        _numOfEnemyInCurWave = Mathf.Max(0, total);
        _waveNumber = Mathf.Max(1, waveNumber);
    }

    public void SpawnEnemy(EnemyType enemyType)
    {
        EnemyInfoSO enemyInfo = _cloneEnemyList.Find(x => x.EnemyType == enemyType);
        EnemyBase enemyPrefab = enemiesPrefabs.Find(x => x.EnemyType == enemyType);
        if (enemyInfo == null)
        {
            Debug.LogError("EnemyInfo not found for type: " + enemyType);
            return;
        }
        else if (enemyPrefab == null)
        {
            Debug.LogError("EnemyPrefab not found for type: " + enemyType);
            return;
        }

        Vector2 spawnPos = RandomPosSpawn();
        EnemyBase newEnemy = PoolingManager.Spawn<EnemyBase>(enemyPrefab, spawnPos, Quaternion.identity, objectPool != null ? objectPool.transform : null);
        newEnemy.Init(enemyInfo.HP, enemyInfo.Armor, enemyInfo.DmgExplosion, enemyInfo.Speed, enemyInfo.Dmg, enemyInfo.RateOfFire
        );
    }

    public void GetScaledInfo()
    {
        foreach (EnemyInfoSO info in _cloneEnemyList)
        {
            if (info.EnemyType == EnemyType.Boom && _waveNumber <= 1)
                continue;
            else if (info.EnemyType == EnemyType.Shooter && _waveNumber <= 3)
                continue;
            else if (info.EnemyType == EnemyType.Boss && _waveNumber <= 5)
                continue;
            info.HP += info.HP * 0.1f * (_waveNumber - 1);
            info.Dmg += info.Dmg * 0.1f * (_waveNumber - 1);
            info.Armor += info.Armor * 0.1f * (_waveNumber - 1);
            info.DmgExplosion += info.DmgExplosion * 0.1f * (_waveNumber - 1);
        }
    }

    public void OnEnemyDie(object param)
    {
        _numOfEnemyInCurWave--;
        if (_numOfEnemyInCurWave <= 0)
        {
            GamePlayManager.Instance.EndWave();
        }
    }    

    public Vector2 RandomPosSpawn()
    {
        return spawnGates[Random.Range(0, spawnGates.Count)].gameObject.transform.position;
    }

    private void OnDisable()
    {
        ObserverManager<GameState>.RemoveAddListener(GameState.OnEnemyDie, OnEnemyDie);
    }
}
