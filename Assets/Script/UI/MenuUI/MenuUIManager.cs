using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject btnPlay;
    [SerializeField] private GameObject btnExit;
    [SerializeField] private GameObject btnSetting;
    [SerializeField] private GameObject btnShop;

    [Header("Popup")]
    [SerializeField] private GameObject popupSetting;
    [SerializeField] private GameObject popupShop;

    [Header("Popup Manager")]
    [SerializeField] private UISetting settingManager;
    public UISetting SettingManager => settingManager;
    [SerializeField] private UIShop shopManager;
    public UIShop ShopManager => shopManager;

    private void Start()
    {
        btnPlay.GetComponent<Button>().onClick.AddListener(OnClickPlay);
        btnExit.GetComponent<Button>().onClick.AddListener(OnClickExit);
        btnSetting.GetComponent<Button>().onClick.AddListener(OnClickSetting);
        btnShop.GetComponent<Button>().onClick.AddListener(OnClickShop);
        popupSetting.SetActive(false);
        popupShop.SetActive(false);
    }

    public void OnClickPlay()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void OnClickExit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OnClickSetting()
    {
        popupSetting.SetActive(true);
        //settingManager.Init();
    }

    public void OnClickShop()
    {
        popupShop.SetActive(true);
        //shopManager.Init();
    }
}
