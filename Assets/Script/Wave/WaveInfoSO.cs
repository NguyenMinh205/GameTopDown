using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveSO", menuName = "ScriptableObjects/Wave")]
public class WaveInfoSO : ScriptableObject
{
    public List<EnemyInWaveInfo> enemies;
}

[System.Serializable]
public class EnemyInWaveInfo
{
    public EnemyType enemyType;
    public int numOfEnemy;
}
