using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUIController : Singleton<GameUIController>
{
    [Header("Player Bar")]
    [SerializeField] private Image _hpBar;
    [SerializeField] private TextMeshProUGUI _hpVal;

    [SerializeField] private Image _shieldBar;
    [SerializeField] private TextMeshProUGUI _shieldVal;

    [Header("Popups")]
    [SerializeField] private GameObject _settingPopupParent;
    [SerializeField] private GameObject _endGamePopupParent;
    [SerializeField] private GameObject _rewardPopupParent;
    [SerializeField] private GameObject _instructionPopupParent;
    [SerializeField] private GameObject _settingPopup;
    [SerializeField] private GameObject _endGamePopUp;
    [SerializeField] private GameObject _rewardPopUp;
    [SerializeField] private GameObject _instructionPopUp;

    [Header("Detail Wave")]
    [SerializeField] private GameObject _waveNotificationParent;
    [SerializeField] private GameObject _waveNotification;
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _numOfCurWaveText;
    [SerializeField] private TextMeshProUGUI _numOfEnemyKilledText;
    [SerializeField] private TextMeshProUGUI _numOfEnemyInCurWaveText;

    [Header("Buff")]
    [SerializeField] private TextMeshProUGUI _numOfBuff1;
    [SerializeField] private TextMeshProUGUI _numOfBuff2;
    [SerializeField] private TextMeshProUGUI _numOfBuff3;

    [SerializeField] private Image _cooldownBuff1;
    [SerializeField] private Image _cooldownBuff2;
    [SerializeField] private Image _cooldownBuff3;

    [Header("Instruction")]
    [SerializeField] private Image _instructionImage;
    [SerializeField] private Button _skipButton;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private TextMeshProUGUI _confirmText;
    [SerializeField] private List<Sprite> _instructionSprites;
    private int currentInstructionStep = 0;

    [Header("EndGame")]
    [SerializeField] private TextMeshProUGUI _endGameCurWaveText;
    [SerializeField] private TextMeshProUGUI _endGameHighestWaveText;
    [SerializeField] private TextMeshProUGUI _endGameNumOfEnemyKilledText;
    [SerializeField] private TextMeshProUGUI _endGameCoinText;
    private int coinStartGame = 0;

    [Header("Config")]
    [SerializeField] private float _waveNotifScaleInDuration = 0.35f;
    [SerializeField] private float _waveNotifHoldDuration = 0.65f;
    [SerializeField] private float _waveNotifScaleOutDuration = 0.35f;

    private PlayerController curPlayer;
    private Tween _waveTween;
    private Coroutine cooldownCoroutine;

    public void SetupStartGame()
    {
        curPlayer = GamePlayManager.Instance.PlayerController;
        coinStartGame = DataManager.Instance.GameData.Coin;
        UpdateBar();
        UpdateCoin(DataManager.Instance.GameData.Coin);
        UpdateNumOfBuff(DataManager.Instance.GameData.NumOfBuff1, BuffType.Buff1);
        UpdateNumOfBuff(DataManager.Instance.GameData.NumOfBuff2, BuffType.Buff2);
        UpdateNumOfBuff(DataManager.Instance.GameData.NumOfBuff3, BuffType.Buff3);
    }    

    public void ShowWaveText(int wave)
    {
        _waveText.text = $"WAVE {wave}";
    }

    public void UpdateBar()
    {
        UpdateHPBar(curPlayer.CurHP, curPlayer.MaxHP);
        UpdateShieldBar(curPlayer.CurArmor, curPlayer.MaxArmor);
    }

    public void UpdateHPBar(float currentHP, float maxHP)
    {
        _hpBar.fillAmount = currentHP / maxHP;
        _hpVal.text = $"{Math.Round(currentHP, 1)}/{Math.Round(maxHP, 1)}";
    }

    public void UpdateShieldBar(float currentShield, float maxShield)
    {
        _shieldBar.fillAmount = currentShield / maxShield;
        _shieldVal.text = $"{Math.Round(currentShield, 1)}/{Math.Round(maxShield, 1)}";
    }

    public void UpdateCoin(int coin)
    {
        _coinText.text = coin.ToString();
    }

    public void UpdateNumOfBuff(int buff, BuffType buffType)
    {
        if (buffType == BuffType.Buff1)
        {
            _numOfBuff1.text = buff.ToString();
        }
        else if (buffType == BuffType.Buff2)
        {
            _numOfBuff2.text = buff.ToString();
        }
        else if (buffType == BuffType.Buff3)
        {
            _numOfBuff3.text = buff.ToString();
        }
    }

    public void StartBuffCooldownUI(BuffType buffType, float cooldown)
    {
        Image cooldownImage = GetCooldownImage(buffType);
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        if (cooldownImage != null)
        {
            cooldownCoroutine = StartCoroutine(UpdateCooldown(cooldownImage, cooldown));
        }
    }

    private Image GetCooldownImage(BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.Buff1: return _cooldownBuff1;
            case BuffType.Buff2: return _cooldownBuff2;
            case BuffType.Buff3: return _cooldownBuff3;
            default: return null;
        }
    }

    private IEnumerator UpdateCooldown(Image cooldownImage, float cooldown)
    {
        float elapsedTime = 0f;
        cooldownImage.fillAmount = 1f;

        while (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;
            cooldownImage.fillAmount = 1f - (elapsedTime / cooldown);
            yield return null;
        }

        cooldownImage.fillAmount = 0f;
    }

    public void DetailsInCurWave(int numOfEnemyInCurWave, int curWave)
    {
        _numOfCurWaveText.text = curWave.ToString();
        UpdateNumOfEnemyInCurWave(numOfEnemyInCurWave);
    }

    public void UpdateNumOfEnemyInCurWave(int numOfEnemyInCurWave)
    {
        _numOfEnemyInCurWaveText.text = numOfEnemyInCurWave.ToString();
    }

    public void UpdateNumOfEnemyKilled(int numOfEnemyKilled)
    {
        _numOfEnemyKilledText.text = numOfEnemyKilled.ToString();
    }
    
    public void ShowInstruction(bool val)
    {
        if (val)
        {
            _instructionPopupParent.SetActive(true);
            _instructionPopUp.transform.DOScale(Vector3.one, 0.25f)
                .From(Vector3.zero)
                .SetEase(Ease.OutBack);
        }
        else
        {
            _instructionPopUp.transform.DOScale(Vector3.zero, 0.25f)
                .SetEase(Ease.InBack)
                .OnComplete(() => _instructionPopupParent.SetActive(false));
        }
    }

    public void SetupInstructionDetail()
    {
        currentInstructionStep = 0;
        UpdateInstructionDetail();
        _skipButton.gameObject.SetActive(true);
        _confirmButton.gameObject.SetActive(true);
        _confirmText.text = "Next!";
        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(() => { currentInstructionStep++; UpdateInstructionDetail(); } );
    }

    public void UpdateInstructionDetail()
    {
        _instructionImage.sprite = _instructionSprites[currentInstructionStep];
        if (currentInstructionStep == _instructionSprites.Count - 1)
        {
            _confirmText.text = "I'm understand!";
            _skipButton.gameObject.SetActive(false);
            _confirmButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.AddListener(() => { GamePlayManager.Instance.FinishInstruction(); });
        }
    }

    public void ShowEndGameDetails(int curWave, int numOfEnemyKilled)
    {
        if (DataManager.Instance.GameData.HighestWave < curWave)
        {
            DataManager.Instance.GameData.HighestWave = curWave;
        }
        _endGameCurWaveText.text = $"Waves Survived: {curWave}";
        _endGameHighestWaveText.text = $"Highest Wave: {DataManager.Instance.GameData.HighestWave}";
        _endGameNumOfEnemyKilledText.text = $"Tanks Destroyed: {numOfEnemyKilled}";

        int coinEarned = DataManager.Instance.GameData.Coin - coinStartGame;
        _endGameCoinText.text = $"Coins Collected: {coinEarned}";

    }

    public void ShowWaveNotification()
    {
        _waveNotificationParent.SetActive(true);
        _waveNotification.transform.localScale = new Vector3(1, 0, 1);

        _waveTween?.Kill();

        _waveTween = DOTween.Sequence()
                .Append(_waveNotification.transform.DOScale(1f, _waveNotifScaleInDuration).SetEase(Ease.OutBack))
                .AppendInterval(_waveNotifHoldDuration)
                .Append(_waveNotification.transform.DOScale(0f, _waveNotifScaleOutDuration).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    _waveNotificationParent.SetActive(false);
                    GamePlayManager.Instance.WaveSpawnerController.SetUpWaveData();
                });
    }

    public void ShowSetting(bool val)
    {
        if (val)
        {
            _settingPopupParent.SetActive(true);
            _settingPopup.transform.DOScale(Vector3.one, 0.25f)
                .From(Vector3.zero)
                .SetEase(Ease.OutBack);
        }
        else
        {
            _settingPopup.transform.DOScale(Vector3.zero, 0.25f)
                .SetEase(Ease.InBack)
                .OnComplete(() => _settingPopupParent.SetActive(false));
        }
    }

    public void ShowEndGamePopup(bool val)
    {
        if (val)
        {
            _endGamePopupParent.SetActive(true);
            _endGamePopUp.transform.DOScale(Vector3.one, 0.25f)
                .From(Vector3.zero)
                .SetEase(Ease.OutBack);
        }
        else
        {
            _endGamePopUp.transform.DOScale(Vector3.zero, 0.25f)
                .SetEase(Ease.InBack)
                .OnComplete(() => _endGamePopupParent.SetActive(false));
        }
    }

    public void ShowRewardPopup(bool val)
    {
        if (val)
        {
            _rewardPopupParent.SetActive(true);
            _rewardPopUp.transform.DOScale(Vector3.one, 0.25f)
                .From(Vector3.zero)
                .SetEase(Ease.OutBack);
        }
        else
        {
            _rewardPopUp.transform.DOScale(Vector3.zero, 0.25f)
                .SetEase(Ease.InBack)
                .OnComplete(() => _rewardPopupParent.SetActive(false));
        }
    }
}
