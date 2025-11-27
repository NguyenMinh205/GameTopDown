using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    public TextMeshProUGUI CoinText => coinText;

    [Header("Buff 1 Info")]
    [SerializeField] private TextMeshProUGUI numOfBuff1;
    [SerializeField] private TextMeshProUGUI nameBuff1;
    [SerializeField] private TextMeshProUGUI descriptionBuff1;
    [SerializeField] private TextMeshProUGUI priceBuff1;
    [SerializeField] private Image iconBuff1;
    [SerializeField] private Button buyBuff1Btn;
    private int buff1Price = 100;

    [Header("Buff 2 Info")]
    [SerializeField] private TextMeshProUGUI numOfBuff2;
    [SerializeField] private TextMeshProUGUI nameBuff2;
    [SerializeField] private TextMeshProUGUI descriptionBuff2;
    [SerializeField] private TextMeshProUGUI priceBuff2;
    [SerializeField] private Image iconBuff2;
    [SerializeField] private Button buyBuff2Btn;
    private int buff2Price = 200;

    [Header("Buff 3 Info")]
    [SerializeField] private TextMeshProUGUI numOfBuff3;
    [SerializeField] private TextMeshProUGUI nameBuff3;
    [SerializeField] private TextMeshProUGUI descriptionBuff3;
    [SerializeField] private TextMeshProUGUI priceBuff3;
    [SerializeField] private Image iconBuff3;
    [SerializeField] private Button buyBuff3Btn;
    private int buff3Price = 300;

    [SerializeField] private List<BuffInfoSO> buffs;

    private readonly Dictionary<BuffType, BuffInfoSO> _buffMap = new Dictionary<BuffType, BuffInfoSO>();

    private void Start()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            BuffInfoSO buff = buffs[i];
            _buffMap[buff.id] = buff;
        }

        if (_buffMap.TryGetValue(BuffType.Buff1, out BuffInfoSO b1))
        {
            if (iconBuff1) iconBuff1.sprite = b1.icon;
            if (nameBuff1) nameBuff1.text = b1.buffName;
            if (descriptionBuff1) descriptionBuff1.text = b1.description;
            if (priceBuff1) priceBuff1.text = b1.price.ToString();
        }
        if (_buffMap.TryGetValue(BuffType.Buff2, out BuffInfoSO b2))
        {
            if (iconBuff2) iconBuff2.sprite = b2.icon;
            if (nameBuff2) nameBuff2.text = b2.buffName;
            if (descriptionBuff2) descriptionBuff2.text = b2.description;
            if (priceBuff2) priceBuff2.text = b2.price.ToString();
        }
        if (_buffMap.TryGetValue(BuffType.Buff3, out BuffInfoSO b3))
        {
            if (iconBuff3) iconBuff3.sprite = b3.icon;
            if (nameBuff3) nameBuff3.text = b3.buffName;
            if (descriptionBuff3) descriptionBuff3.text = b3.description;
            if (priceBuff3) priceBuff3.text = b3.price.ToString();
        }

        if (buyBuff1Btn) buyBuff1Btn.onClick.AddListener(() => Purchase(buff1Price, OnBuyBuff1Success, numOfBuff1));
        if (buyBuff2Btn) buyBuff2Btn.onClick.AddListener(() => Purchase(buff2Price, OnBuyBuff2Success, numOfBuff2));
        if (buyBuff3Btn) buyBuff3Btn.onClick.AddListener(() => Purchase(buff3Price, OnBuyBuff3Success, numOfBuff3));
    }

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        GameData gd = DataManager.Instance.GameData;

        if (coinText) coinText.text = gd.Coin.ToString();
        if (numOfBuff1) numOfBuff1.text = gd.NumOfBuff1.ToString();
        if (numOfBuff2) numOfBuff2.text = gd.NumOfBuff2.ToString();
        if (numOfBuff3) numOfBuff3.text = gd.NumOfBuff3.ToString();

        CheckAcceptBuy();
    }

    private void OnBuyBuff1Success() { DataManager.Instance.GameData.NumOfBuff1++; }
    private void OnBuyBuff2Success() { DataManager.Instance.GameData.NumOfBuff2++; }
    private void OnBuyBuff3Success() { DataManager.Instance.GameData.NumOfBuff3++; }

    private void Purchase(int price, Action onSuccess, TextMeshProUGUI countLabel)
    {
        GameData gd = DataManager.Instance.GameData;

        if (gd.Coin >= price)
        {
            gd.Coin -= price;
            onSuccess?.Invoke();

            if (coinText) coinText.text = gd.Coin.ToString();

            if (countLabel != null)
            {
                int currentCount = 0;
                int.TryParse(countLabel.text, out currentCount);
                int newCount = currentCount + 1;
                countLabel.text = newCount.ToString();
            }

            ObserverManager<AchievementEvents>.PostEvent(AchievementEvents.OnCoinsSpent, price);
            AudioManager.Instance.PlayUseCoinSound();
        }
        else
        {
            AudioManager.Instance.PlayErrorSound();
        }

        CheckAcceptBuy();
    }

    public void CheckAcceptBuy()
    {
        int coin = DataManager.Instance.GameData.Coin;

        priceBuff1.color = (coin < buff1Price) ? Color.red : Color.black;
        priceBuff2.color = (coin < buff2Price) ? Color.red : Color.black;
        priceBuff3.color = (coin < buff3Price) ? Color.red : Color.black;

        buyBuff1Btn.interactable = (coin >= buff1Price);
        buyBuff2Btn.interactable = (coin >= buff2Price);
        buyBuff3Btn.interactable = (coin >= buff3Price);
    }
}
