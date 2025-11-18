using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private List<RewardDetail> rewardDetails = new List<RewardDetail>();
    [SerializeField] private List<RewardSO> weaponRewards = new List<RewardSO>();
    [SerializeField] private List<RewardSO> statRewards = new List<RewardSO>();

    List<RewardSO> currentRewards = new List<RewardSO>();

    private const int numOfRewards = 3;

    public void GenerateReward()
    {
        currentRewards.Clear();

        if (rewardDetails == null || rewardDetails.Count == 0)
        {
            Debug.LogError("Danh sách rewardDetails rỗng hoặc null!");
            return;
        }

        List<RewardSO> sourceRewards = GamePlayManager.Instance.CurrentWave == 1 ? weaponRewards : statRewards;
        if (sourceRewards == null || sourceRewards.Count == 0)
        {
            Debug.LogError($"Danh sách {(GamePlayManager.Instance.CurrentWave == 1 ? "weaponRewards" : "statRewards")} rỗng hoặc null!");
            return;
        }

        List<int> indices = Enumerable.Range(0, sourceRewards.Count).ToList();
        Shuffle(indices);

        for (int i = 0; i < numOfRewards; i++)
        {
            int originalIndex = indices[i];
            currentRewards.Add(sourceRewards[originalIndex]);
        }

        for (int i = 0; i < rewardDetails.Count; i++)
        {
            if (i < numOfRewards)
            {
                RewardSO selectedReward = currentRewards[i];

                rewardDetails[i].rewardIcon.sprite = selectedReward.icon;
                rewardDetails[i].rewardName.text = selectedReward.rewardName;
                rewardDetails[i].rewardDes.text = selectedReward.rewardDescription;
                rewardDetails[i].rewardSlot.SetActive(true);
                rewardDetails[i].rewardSlot.transform.DOScale(Vector3.one, 0.3f).From(Vector3.zero).SetEase(Ease.OutBack).SetDelay(0.5f);
            }
            else
            {
                rewardDetails[i].rewardSlot.SetActive(false);
            }
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void ChooseReward(int index)
    {
        if (!GamePlayManager.Instance.IsChoosingReward) return;
        GamePlayManager.Instance.IsChoosingReward = false;
        RewardSO chosenReward;
        chosenReward = currentRewards[index];
        ApplyRewardToPlayer(chosenReward);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            GamePlayManager.Instance.EndChooseReward();
        });
    }

    public void ApplyRewardToPlayer(RewardSO reward)
    {
        PlayerController player = GamePlayManager.Instance.PlayerController;
        switch (reward.rewardType)
        {
            case RewardType.HPReward:
                GamePlayManager.Instance.PlayerController.MaxHP += GamePlayManager.Instance.PlayerController.MaxHP * reward.valueBuff;
                GamePlayManager.Instance.PlayerController.CurHP += GamePlayManager.Instance.PlayerController.MaxHP * reward.valueBuff;
                GameUIController.Instance?.UpdateHPBar(GamePlayManager.Instance.PlayerController.CurHP, GamePlayManager.Instance.PlayerController.MaxHP);
                break;
            case RewardType.ShieldReward:
                GamePlayManager.Instance.PlayerController.MaxArmor += GamePlayManager.Instance.PlayerController.MaxArmor * reward.valueBuff;
                GamePlayManager.Instance.PlayerController.CurArmor += GamePlayManager.Instance.PlayerController.MaxArmor * reward.valueBuff;
                GameUIController.Instance?.UpdateShieldBar(GamePlayManager.Instance.PlayerController.CurArmor, GamePlayManager.Instance.PlayerController.MaxArmor);
                break;
            case RewardType.AttackReward:
                GamePlayManager.Instance.PlayerController.AttackStat += GamePlayManager.Instance.PlayerController.AttackStat * reward.valueBuff;
                break;
            case RewardType.RateOfFireReward:
                BarrelBase barrel = GamePlayManager.Instance.PlayerController.BarrelController.CurTypeOfBarrel;
                if (barrel != null)
                {
                    barrel.AttackCoolDown -= barrel.AttackCoolDown * reward.valueBuff;
                }    
                break;
            case RewardType.SpeedTankReward:
                GamePlayManager.Instance.PlayerController.Speed += GamePlayManager.Instance.PlayerController.Speed * reward.valueBuff;
                break;
            case RewardType.WeaponReward:
                GamePlayManager.Instance.PlayerController.ChangeBarrel(reward.rewardName);
                break;
            default:
                Debug.LogWarning("Unknown reward type: " + reward.rewardType);
                break;
        }
    }
}

[System.Serializable]
public class RewardDetail
{
    public GameObject rewardSlot;
    public Image rewardIcon;
    public TextMeshProUGUI rewardName;
    public TextMeshProUGUI rewardDes;
    public Button chooseButton;
}
