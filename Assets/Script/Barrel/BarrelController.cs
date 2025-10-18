using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : MonoBehaviour
{
    [SerializeField] private BarrelBase curTypeOfBarrel;
    [SerializeField] private GameObject barrel;

    private void Start()
    {
        curTypeOfBarrel.Init(barrel);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            curTypeOfBarrel.Fire();
        } 
    }

    public void ChangeTypeOfBarrel(BarrelBase newBarrel)
    {
        this.curTypeOfBarrel = newBarrel;
        curTypeOfBarrel.Init(barrel);
    }    
}
