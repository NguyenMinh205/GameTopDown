using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Buff 1 Info")]
    [SerializeField] private TextMeshProUGUI numOfBuff1;
    [SerializeField] private TextMeshProUGUI nameBuff1;
    [SerializeField] private TextMeshProUGUI descriptionBuff1;
    [SerializeField] private TextMeshProUGUI priceBuff1;
    [SerializeField] private Image iconBuff1;
    [SerializeField] private Button buyBuff1Btn;
    private const int buff1Price = 100;

    [Header("Buff 2 Info")]
    [SerializeField] private TextMeshProUGUI numOfBuff2;
    [SerializeField] private TextMeshProUGUI nameBuff2;
    [SerializeField] private TextMeshProUGUI descriptionBuff2;
    [SerializeField] private TextMeshProUGUI priceBuff2;
    [SerializeField] private Image iconBuff2;
    [SerializeField] private Button buyBuff2Btn;
    private const int buff2Price = 200;

    [Header("Buff 3 Info")]
    [SerializeField] private TextMeshProUGUI numOfBuff3;
    [SerializeField] private TextMeshProUGUI nameBuff3;
    [SerializeField] private TextMeshProUGUI descriptionBuff3;
    [SerializeField] private TextMeshProUGUI priceBuff3;
    [SerializeField] private Image iconBuff3;
    [SerializeField] private Button buyBuff3Btn;
    private const int buff3Price = 300;

    private List<BuffSO> buffList = new List<BuffSO>();
    private void Start()
    {
        if (buyBuff1Btn != null)
            buyBuff1Btn.onClick.AddListener(PurchaseBuff1);
        if (buyBuff2Btn != null)
            buyBuff2Btn.onClick.AddListener(PurchaseBuff2);
        if (buyBuff3Btn != null)
            buyBuff3Btn.onClick.AddListener(PurchaseBuff3);
    }

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        coinText.text = DataManager.Instance.GameData.Coin.ToString();
        numOfBuff1.text = DataManager.Instance.GameData.NumOfBuff1.ToString();
        numOfBuff2.text = DataManager.Instance.GameData.NumOfBuff2.ToString();
        numOfBuff3.text = DataManager.Instance.GameData.NumOfBuff3.ToString();
        CheckAcceptBuy(priceBuff1);
        CheckAcceptBuy(priceBuff2);
        CheckAcceptBuy(priceBuff3);
    }    

    public void PurchaseBuff1()
    {
        if (DataManager.Instance.GameData.Coin >= buff1Price)
        {
            DataManager.Instance.GameData.Coin -= buff1Price;
            DataManager.Instance.GameData.NumOfBuff1 += 1;
            coinText.text = DataManager.Instance.GameData.Coin.ToString();
            numOfBuff1.text = DataManager.Instance.GameData.NumOfBuff1.ToString();
        }
        CheckAcceptBuy(priceBuff1);
    }    

    public void PurchaseBuff2()
    {
        if (DataManager.Instance.GameData.Coin >= buff2Price)
        {
            DataManager.Instance.GameData.Coin -= buff2Price;
            DataManager.Instance.GameData.NumOfBuff2 += 1;
            coinText.text = DataManager.Instance.GameData.Coin.ToString();
            numOfBuff2.text = DataManager.Instance.GameData.NumOfBuff2.ToString();
        }
        CheckAcceptBuy(priceBuff2);
    }    

    public void PurchaseBuff3()
    {
        if (DataManager.Instance.GameData.Coin >= buff3Price)
        {
            DataManager.Instance.GameData.Coin -= buff3Price;
            DataManager.Instance.GameData.NumOfBuff3 += 1;
            coinText.text = DataManager.Instance.GameData.Coin.ToString();
            numOfBuff3.text = DataManager.Instance.GameData.NumOfBuff3.ToString();
        }
        CheckAcceptBuy(priceBuff3);

    }

    public void CheckAcceptBuy(TextMeshProUGUI priceTxt)
    {
        if (DataManager.Instance.GameData.Coin < int.Parse(priceTxt.text))
        {
            priceTxt.color = Color.red;
        }
        else
        {
            priceTxt.color = Color.black;
        }
    }    

    public void OnDrawGizmosSelected()
    {
        buffList = Resources.LoadAll<BuffSO>("BuffSO").ToList();
        foreach (BuffSO buff in buffList)
        {
            if (buff.id == BuffType.Buff1)
            {
                iconBuff1.sprite = buff.icon;
                nameBuff1.text = buff.buffName;
                descriptionBuff1.text = buff.description;
                priceBuff1.text = buff1Price.ToString();
            }
            else if (buff.id == BuffType.Buff2)
            {
                iconBuff2.sprite = buff.icon;
                nameBuff2.text = buff.buffName;
                descriptionBuff2.text = buff.description;
                priceBuff2.text = buff2Price.ToString();
            }
            else if (buff.id == BuffType.Buff3)
            {
                iconBuff3.sprite = buff.icon;
                nameBuff3.text = buff.buffName;
                descriptionBuff3.text = buff.description;
                priceBuff3.text = buff3Price.ToString();
            }
        }
    }
}
