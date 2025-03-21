using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] float HP, armor;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject objectPool;
    //[SerializeField] private float spawnRange; 
    [SerializeField] private float spawnTime;
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private List<GameObject> spawnGates = new List<GameObject>();

    private void Start()
    {
        StartCoroutine("SpawnEnemies");
    }

    public void SpawnEnemy()
    {
        Vector2 spawnPos = RandomPosSpawn();
        EnemyController newEnemy = PoolingManager.Spawn<EnemyController>(enemyPrefab, spawnPos, Quaternion.identity, objectPool.transform);
        newEnemy.Init(HP, armor);
    }    

    public Vector2 RandomPosSpawn ()
    { 
        return spawnGates[Random.Range(0, spawnGates.Count - 1)].gameObject.transform.position;
    }    

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnTime);
        } 
    }    
}
