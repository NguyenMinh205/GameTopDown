using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "ScriptableObjects/Buff")]
public class BuffSO : ScriptableObject
{
    public BuffType id;
    public string buffName;
    [TextArea(1, 5)]
    public string description;
    public Sprite icon;
    public int price;
}

public enum BuffType
{
    Buff1,
    Buff2,
    Buff3
}
