using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class IntroUIController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;


    private void Start()
    {
        playButton.transform.localScale = Vector3.zero;
        quitButton.transform.localScale = Vector3.zero;
        playButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        playButton.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBounce);
        quitButton.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBounce);
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
