using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyInfoSO", menuName = "ScriptableObjects/Enemy")]
public class EnemyInfoSO : ScriptableObject
{
    public EnemyType EnemyType;
    public float HP;
    public float Armor;
    public float Speed;
    public float DmgExplosion;
    public float RateOfFire;
    public float Dmg;
}
