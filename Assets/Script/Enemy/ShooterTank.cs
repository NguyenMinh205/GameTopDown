using System.Collections;
using UnityEngine;

public class ShooterTank : ShootingEnemy
{
    protected override IEnumerator ShootBurst()
    {
        ShootSingle();
        yield return null;
    }
}