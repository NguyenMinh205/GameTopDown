using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementObjectUI : MonoBehaviour
{
    [SerializeField] private Image _iconChecked;
    [SerializeField] private Sprite _trueIcon;
    [SerializeField] private Sprite _falseIcon;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    public void Init(Achievement achievement)
    {
        _titleText.text = achievement.Title;
        _descriptionText.text = achievement.Description;
        if (achievement.IsUnlocked())
        {
            _iconChecked.sprite = _trueIcon;
        }
        else
        {
            _iconChecked.sprite = _falseIcon;
        }
    }
}
