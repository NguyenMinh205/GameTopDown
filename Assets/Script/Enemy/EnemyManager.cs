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
    [SerializeField] private ParticleSystem appearParticle;

    private const float PARTICLE_DESPAWN_BUFFER = 0.05f;

    private int _waveNumber = 1;
    private int _numOfEnemyInCurWave;
    private int _numOfEnemyKilled = 0;
    private List<EnemyBase> _activeEnemies = new List<EnemyBase>();
    public int NumOfEnemyInCurWave
    {
        get { return _numOfEnemyInCurWave; }
        set
        {
            _numOfEnemyInCurWave = value;
            GameUIController.Instance.UpdateNumOfEnemyInCurWave(_numOfEnemyInCurWave);
        }
    }

    public int NumOfEnemyKilled
    {
        get { return _numOfEnemyKilled; }
        set
        {
            _numOfEnemyKilled = value;
            GameUIController.Instance.UpdateNumOfEnemyKilled(_numOfEnemyKilled);
        }
    }
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
            cloneInfo.CoinValue = info.CoinValue;
            _cloneEnemyList.Add(cloneInfo);
        }
        ObserverManager<GameState>.AddRegisterEvent(GameState.OnEnemyDie, OnEnemyDie);
        NumOfEnemyInCurWave = 0;
        NumOfEnemyKilled = 0;
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

        SpawnAppearEffect(spawnPos);

        newEnemy.Init(enemyInfo.HP, enemyInfo.Armor, enemyInfo.DmgExplosion, enemyInfo.Speed, enemyInfo.Dmg, enemyInfo.RateOfFire, enemyInfo.CoinValue);
        _activeEnemies.Add(newEnemy);
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
        _activeEnemies.Remove(param as EnemyBase);
        NumOfEnemyInCurWave--;
        NumOfEnemyKilled++;
        if (_numOfEnemyInCurWave <= 0)
        {
            GamePlayManager.Instance.EndWave();
        }
    }

    public Vector2 RandomPosSpawn()
    {
        return spawnGates[Random.Range(0, spawnGates.Count)].gameObject.transform.position;
    }

    public void ClearAllEnemy()
    {
        foreach (EnemyBase enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                PoolingManager.Despawn(enemy.gameObject);
            }
        }
    }

    private void SpawnAppearEffect(Vector2 pos)
    {
        if (appearParticle == null)
            return;

        ParticleSystem ps = PoolingManager.Spawn<ParticleSystem>(appearParticle, pos, Quaternion.identity, objectPool != null ? objectPool.transform : null);
        if (ps == null)
            return;

        ps.Play();

        StartCoroutine(DespawnParticleAfter(ps));
    }

    private IEnumerator DespawnParticleAfter(ParticleSystem ps)
    {
        if (ps == null)
            yield break;

        var main = ps.main;

        if (main.loop)
        {
            float fallback = 2f;
            yield return new WaitForSeconds(fallback + PARTICLE_DESPAWN_BUFFER);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            ReturnParticleToPool(ps);
            yield break;
        }

        float duration = main.duration;
        float maxStartLifetime = main.startLifetime.constantMax;
        float waitTime = duration + maxStartLifetime + PARTICLE_DESPAWN_BUFFER;

        yield return new WaitForSeconds(waitTime);

        ReturnParticleToPool(ps);
    }

    private void ReturnParticleToPool(ParticleSystem ps)
    {
        if (ps == null) return;
        PoolingManager.Despawn(ps.gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        ObserverManager<GameState>.RemoveAddListener(GameState.OnEnemyDie, OnEnemyDie);
    }
}
