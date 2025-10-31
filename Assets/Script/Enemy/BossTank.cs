using System.Collections;
using UnityEngine;

public class BossTank : ShootingEnemy
{
    private int numberOfBulletsInBurst = 3;
    public int NumberOfBulletsInBurst => numberOfBulletsInBurst;

    protected override IEnumerator ShootBurst()
    {
        for (int i = 0; i < numberOfBulletsInBurst; i++)
        {
            ShootSingle();
            if (i < numberOfBulletsInBurst - 1)
                yield return new WaitForSeconds(burstDelay);
        }
    }
}