using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelTransform: MonoBehaviour
{
    [SerializeField] private Transform tankBody;
    [SerializeField] private Camera mainCamera;

    private void Start()
    {
        
    }

    void Update()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - tankBody.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        this.transform.rotation = Quaternion.Euler(0, 0, angle + 90); 
    }   
}
