using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reward", menuName = "ScriptableObjects/Reward")]
public class RewardSO : ScriptableObject
{
    public RewardType rewardType;
    public Sprite icon;
    public string rewardName;
    [TextArea(1, 3)]
    public string rewardDescription;
    public float valueBuff;
}

public enum RewardType
{
    HPReward,
    ShieldReward,
    AttackReward,
    WeaponReward,
}
