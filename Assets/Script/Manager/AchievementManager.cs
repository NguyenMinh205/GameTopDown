using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementManager : Singleton<AchievementManager>
{
    [SerializeField] private List<Achievement> allAchievements;

    protected override void Awake()
    {
        base.KeepActive(true);
        base.Awake();
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        ObserverManager<AchievementEvents>.AddRegisterEvent(AchievementEvents.OnEnemyKilled, OnEnemyKilled);
        ObserverManager<AchievementEvents>.AddRegisterEvent(AchievementEvents.OnWaveReached, OnWaveReached);
        ObserverManager<AchievementEvents>.AddRegisterEvent(AchievementEvents.OnCoinsSpent, OnCoinsSpent);
        ObserverManager<AchievementEvents>.AddRegisterEvent(AchievementEvents.OnGameStarted, OnGameStarted);
        ObserverManager<AchievementEvents>.AddRegisterEvent(AchievementEvents.OnGameEnded, OnGameEnded);
    }

    private void OnEnemyKilled(object param)
    {
        int killCount = (int)param;
        DataManager.Instance.GameData.TotalKillsLifetime += killCount;
        DataManager.Instance.GameData.KillsInCurrentGame += killCount;

        CheckAchievements(AchievementType.KillsInSingleGame, DataManager.Instance.GameData.KillsInCurrentGame);
        CheckAchievements(AchievementType.TotalKillsLifetime, DataManager.Instance.GameData.TotalKillsLifetime);
    }

    private void OnWaveReached(object param)
    {
        int wave = (int)param;
        CheckAchievements(AchievementType.ReachWave, wave);
    }

    private void OnCoinsSpent(object param)
    {
        int amount = (int)param;
        DataManager.Instance.GameData.TotalCoinsSpent += amount;
        CheckAchievements(AchievementType.TotalCoinsSpent, DataManager.Instance.GameData.TotalCoinsSpent);
    }

    private void OnGameStarted(object param)
    {
        DataManager.Instance.GameData.ResetKillsInCurrentGame();
    }

    private void OnGameEnded(object param)
    {
        DataManager.Instance.GameData.TotalGamesPlayed++;
        CheckAchievements(AchievementType.TotalGamesPlayed, DataManager.Instance.GameData.TotalGamesPlayed);
    }

    private void CheckAchievements(AchievementType type, int currentValue)
    {
        foreach (var ach in allAchievements.Where(a => a.Type == type && !a.IsUnlocked()))
        {
            if (currentValue >= ach.TargetValue)
            {
                UnlockAchievement(ach);
            }
        }
    }

    public void UnlockAchievement(Achievement ach)
    {
        if (ach.IsUnlocked()) return;

        ach.Achieved = true;
        DataManager.Instance.GameData.UnlockAchievement(ach.ID);
        DataManager.Instance.GameData.Coin += ach.CoinReward;

        ObserverManager<AchievementEvents>.PostEvent(AchievementEvents.AchievementUnlocked, ach);

        Debug.Log($"Unlocked: {ach.Title} +{ach.CoinReward} coins!");
    }

    public List<Achievement> GetAllAchievements()
    {
        foreach (var ach in allAchievements)
        {
            ach.Achieved = DataManager.Instance.GameData.IsAchievementUnlocked(ach.ID);
        }
        return allAchievements;
    }
}

public enum AchievementEvents
{
    OnEnemyKilled,
    OnWaveReached,
    OnCoinsSpent,
    OnGameStarted,
    OnGameEnded,
    AchievementUnlocked
}