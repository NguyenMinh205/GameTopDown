using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController; 

    public void Init()
    {
        playerController.Init();
    }

    private void Start()
    {
        Init();
    }
}
