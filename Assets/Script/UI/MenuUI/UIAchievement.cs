using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAchievement : MonoBehaviour
{
    [SerializeField] private AchievementObjectUI achievementObjectPrefab;
    [SerializeField] private Transform contentTransform;

    public void Init()
    {
        foreach (Achievement achievement in AchievementManager.Instance.GetAllAchievements())
        {
            AchievementObjectUI achievementObj = PoolingManager.Spawn(achievementObjectPrefab, this.transform.position, Quaternion.identity, contentTransform);
            achievementObj.Init(achievement);
        }
    }

    public void ClearAllAchievementUI()
    {
        foreach (Transform child in contentTransform)
        {
            PoolingManager.Despawn(child.gameObject);
        }
    }
}
