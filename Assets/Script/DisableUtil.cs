using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableUtil : MonoBehaviour
{
    public void DeSpawnObject()
    {
        PoolingManager.Despawn(this.gameObject);
    }

    public void DisableObject()
    {
        this.gameObject.SetActive(false);
    }
}
