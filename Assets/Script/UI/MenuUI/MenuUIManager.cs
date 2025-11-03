using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Menu;
using DG.Tweening;

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
        settingManager.gameObject.SetActive(false);
        shopManager.gameObject.SetActive(false);
        AudioManager.Instance.PlayMusicInMenu();
    }

    private void OnEnable()
    {
        AudioManager.Instance?.PlayMusicInMenu();
    }
    public void OnClickPlay()
    {
        AudioManager.Instance.PlayButtonClick();
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void OnClickExit()
    {
        AudioManager.Instance.PlayButtonClick();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OnClickSetting()
    {
        AudioManager.Instance.PlayPopupSound();
        settingManager.gameObject.SetActive(true);
        popupSetting.transform.DOKill();
        popupSetting.transform.localScale = Vector3.zero;

        popupSetting.transform.DOScale(Vector3.one, 0.25f)
            .SetEase(Ease.OutBack);
    }

    public void OnClickShop()
    {
        AudioManager.Instance.PlayPopupSound();
        shopManager.gameObject.SetActive(true);
        popupSetting.transform.DOKill();
        popupShop.transform.localScale = Vector3.zero;

        popupShop.transform.DOScale(Vector3.one, 0.25f)
            .SetEase(Ease.OutBack);
    }

    public void CloseSetting()
    {
        AudioManager.Instance.PlayButtonClick();
        popupSetting.transform.DOKill();
        popupSetting.transform.DOScale(Vector3.zero, 0.25f)
            .SetEase(Ease.InBack).OnComplete(() => settingManager.gameObject.SetActive(false));
    }

    public void CloseShop()
    {
        AudioManager.Instance.PlayButtonClick();
        popupSetting.transform.DOKill();
        popupShop.transform.DOScale(Vector3.zero, 0.25f)
            .SetEase(Ease.InBack).OnComplete(() => shopManager.gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        AudioManager.Instance.StopMusic();
    }
}
