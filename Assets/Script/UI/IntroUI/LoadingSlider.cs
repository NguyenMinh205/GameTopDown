using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LoadingSlider : MonoBehaviour
{
    [SerializeField] private GameObject panelLoading;
    [SerializeField] private Button playButton;
    [SerializeField] private Slider slider;
    [SerializeField] private float duration;
    private const float maxValue = 100;
    private float value;


    void Start()
    {
        slider.maxValue = maxValue;
        DOTween.To(() => value, x => value = x, 100f, duration).OnUpdate(() =>
        {
            slider.value = value;
        }).OnComplete(() =>
        {
            panelLoading.SetActive(false);
            playButton.gameObject.SetActive(true);
            playButton.transform.DOScale(1.5f, 0.35f).SetEase(Ease.OutBounce);
        });
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }
}
