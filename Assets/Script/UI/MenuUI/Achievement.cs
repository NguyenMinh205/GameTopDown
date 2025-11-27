using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "ScriptableObjects/Achievement")]
public class Achievement : ScriptableObject
{
    public string ID;
    public string Title;
    public string Description;
    public AchievementType Type;
    public int TargetValue;
    public int CoinReward;
    public bool Achieved;

    public string GetProgress(int currentValue)
    {
        return $"{currentValue}/{TargetValue}";
    }

    public bool IsUnlocked()
    {
        return DataManager.Instance.GameData.IsAchievementUnlocked(ID);
    }
}

public enum AchievementType
{
    ReachWave,
    KillsInSingleGame,
    TotalKillsLifetime,
    TotalCoinsSpent,
    TotalGamesPlayed,
}