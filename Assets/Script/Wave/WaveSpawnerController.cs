using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnerController : MonoBehaviour
{
    [SerializeField] private List<WaveInfoSO> waves;
    [SerializeField] private float timeSpawnDelay = 1f;
    private bool canSpawn = true;
    private WaveInfoSO currentWave;
    private Coroutine spawnNewWave;

    private List<EnemyInWaveInfo> BuildWaveData(int wave)
    {
        WaveInfoSO templateWave;
        float multiplier = 1f;
        bool even = (wave % 2 == 0);
        int last = Mathf.Max(0, waves.Count - 1);

        if (wave <= 2)
        {
            templateWave = waves[0];
            multiplier = even ? 1.5f : 1f;
        }
        else if (wave <= 4)
        {
            templateWave = waves[1];
            multiplier = even ? 1.5f : 1f;
        }
        else if (wave == 5)
        {
            templateWave = waves[last];
            multiplier = 1f;
        }
        else
        {
            templateWave = waves[last];
            int steps = wave - 5;
            multiplier = Mathf.Pow(1.1f, steps);
        }

        List<EnemyInWaveInfo> listEnemiesSpawn = new List<EnemyInWaveInfo>(templateWave.enemies.Count);
        foreach (var e in templateWave.enemies)
        {
            EnemyInWaveInfo enemySpawnInfo = new EnemyInWaveInfo
            {
                enemyType = e.enemyType,
                numOfEnemy = e.numOfEnemy
            };

            if (e.enemyType != EnemyType.Boss)
            {
                enemySpawnInfo.numOfEnemy = Mathf.Max(0, Mathf.CeilToInt(enemySpawnInfo.numOfEnemy * multiplier));
            }

            listEnemiesSpawn.Add(enemySpawnInfo);
        }

        if (wave >= 10 && wave % 10 == 0)
        {
            int extraBoss = wave / 10;

            int bossIndex = listEnemiesSpawn.FindIndex(x => x.enemyType == EnemyType.Boss);
            if (bossIndex >= 0)
            {
                listEnemiesSpawn[bossIndex] = new EnemyInWaveInfo
                {
                    enemyType = listEnemiesSpawn[bossIndex].enemyType,
                    numOfEnemy = listEnemiesSpawn[bossIndex].numOfEnemy + extraBoss
                };
            }
            else
            {
                listEnemiesSpawn.Add(new EnemyInWaveInfo
                {
                    enemyType = EnemyType.Boss,
                    numOfEnemy = extraBoss
                });
            }
        }

        listEnemiesSpawn.RemoveAll(x => x.numOfEnemy <= 0);
        return listEnemiesSpawn;
    }

    public void SetUpWaveData()
    {
        int wave = GamePlayManager.Instance != null ? GamePlayManager.Instance.CurrentWave : 1;
        List<EnemyInWaveInfo> enemiesWorking = BuildWaveData(wave);

        int total = 0;
        foreach (EnemyInWaveInfo e in enemiesWorking) total += e.numOfEnemy;
        if (GamePlayManager.Instance != null && GamePlayManager.Instance.EnemyManager != null)
        {
            GamePlayManager.Instance.EnemyManager.SetEnemyCountForWave(total, wave);
            GamePlayManager.Instance.EnemyManager.GetScaledInfo();
        }

        GameUIController.Instance.DetailsInCurWave(total, wave);
        StartSpawnNewWave(enemiesWorking);
    }


    private void StartSpawnNewWave(List<EnemyInWaveInfo> workingEnemies)
    {
        if (spawnNewWave != null)
            StopCoroutine(spawnNewWave);
        spawnNewWave = StartCoroutine(SpawnEnemyInNewWave(workingEnemies));
    }

    private IEnumerator SpawnEnemyInNewWave(List<EnemyInWaveInfo> enemiesWorking)
    {
        while (enemiesWorking.Count > 0)
        {
            int index = Random.Range(0, enemiesWorking.Count);

            GamePlayManager.Instance.EnemyManager.SpawnEnemy(enemiesWorking[index].enemyType);

            enemiesWorking[index].numOfEnemy -= 1;

            if (enemiesWorking[index].numOfEnemy <= 0)
                enemiesWorking.RemoveAt(index);

            yield return new WaitForSeconds(timeSpawnDelay);
        }
    }
}
