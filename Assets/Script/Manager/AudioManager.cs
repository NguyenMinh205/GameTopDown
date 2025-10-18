using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    public AudioSource MusicSource => musicSource;
    [SerializeField] private AudioSource soundSource;
    public AudioSource SoundSource => soundSource;

    [Header("Music")]
    [SerializeField] private AudioClip musicIntro;
    [SerializeField] private AudioClip musicInMenu;
    [SerializeField] private AudioClip musicInGame;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip useCoinSound;
    [SerializeField] private AudioClip upgradeSound;
    [SerializeField] private AudioClip popupSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip startWaveSound;
    [SerializeField] private AudioClip endGameSound;
    [SerializeField] private AudioClip errorSound;

    protected override void Awake()
    {
        base.KeepActive(true);
        base.Awake();
        if (musicSource == null || soundSource == null)
        {
            Debug.LogError("AudioSource not assigned in AudioManager!");
        }
    }

    private void Start()
    {
        SetMusicVolume(DataManager.Instance.GameData.VolumeMusic);
        SetSoundVolume(DataManager.Instance.GameData.VolumeSFX);
        PlayMusicIntro();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
            DataManager.Instance.GameData.VolumeMusic = volume;
        }
    }

    public void SetSoundVolume(float volume)
    {
        if (soundSource != null)
        {
            soundSource.volume = volume;
            DataManager.Instance.GameData.VolumeSFX = volume;
        }
    }

    public void ResetDefault()
    {
        if (musicSource != null)
            musicSource.volume = 0.5f;
        if (soundSource != null)
            soundSource.volume = 0.5f;
        SetMusicVolume(0.5f);
        SetSoundVolume(0.5f);
    }

    public void PlayMusicIntro()
    {
        if (musicSource != null && musicIntro != null)
        {
            PlayMusicGame(musicIntro);
        }
    }

    public void PlayMusicInMenu()
    {
        if (musicSource != null && musicInMenu != null)
        {
            PlayMusicGame(musicInMenu);
        }
    }

    public void PlayMusicInGame()
    {
        if (musicSource != null && musicInGame != null)
        {
            PlayMusicGame(musicInGame);
        }
    }

    public void PlayMusicGame(AudioClip clip)
    {
        musicSource.loop = true;
        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();
        musicSource.DOFade(DataManager.Instance.GameData.VolumeMusic, 0.5f).SetUpdate(true);
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.DOFade(0f, 0.5f).OnComplete(() =>
            {
                musicSource.Stop();
            }).SetUpdate(true);
        }
    }

    public void PlaySFX(AudioClip sound, bool repeat = false)
    {
        if (sound != null && soundSource != null)
        {
            if (repeat)
            {
                soundSource.loop = true;
                soundSource.clip = sound;
                soundSource.Play();
            }
            else
            {
                soundSource.loop = false;
                soundSource.PlayOneShot(sound, soundSource.volume);
            }
        }
    }

    public void StopSFX()
    {
        if (soundSource != null && soundSource.isPlaying)
        {
            soundSource.Stop();
            soundSource.loop = false;
            soundSource.clip = null;
        }
    }

    #region Play SFX Methods
    public void PlayButtonClick()
    {
        PlaySFX(buttonClick);
    }

    public void PlayUseCoinSound()
    {
        PlaySFX(useCoinSound);
    }

    public void PlayUpgradeSound()
    {
        PlaySFX(upgradeSound);
    }

    public void PlayPopupSound()
    {
        PlaySFX(popupSound);
    }

    public void PlayShootSound()
    {
        PlaySFX(shootSound);
    }

    public void PlayExplosionSound()
    {
        PlaySFX(explosionSound);
    }

    public void PlayStartWaveSound()
    {
        PlaySFX(startWaveSound);
    }

    public void PlayEndGameSound()
    {
        PlaySFX(endGameSound);
    }

    public void PlayErrorSound()
    {
        PlaySFX(errorSound);
    }
    #endregion
}
