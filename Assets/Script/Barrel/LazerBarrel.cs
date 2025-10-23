using System.Collections;
using UnityEngine;

public class LazerBarrel : BarrelBase
{
    [Header("Lazer Config")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float lazerLength = 25f;
    [SerializeField] private float lazerDuration = 0.2f;
    [SerializeField] private LayerMask enemyLayer;

    private bool isFiring = false;

    public override void Fire()
    {
        if (timer > 0 || isFiring) return;

        StartCoroutine(ShootLazer());
        timer = attackCoolDown;
    }

    private IEnumerator ShootLazer()
    {
        isFiring = true;

        if (lineRenderer == null)
        {
            isFiring = false;
            yield break;
        }

        lineRenderer.enabled = true;
        Vector2 startPos = barrel.transform.position;
        Vector2 direction = - barrel.transform.up;
        Vector2 endPos = startPos + direction * lazerLength;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction, lazerLength, enemyLayer);
        foreach (RaycastHit2D hit in hits)
        {
            IGetHit getHit = hit.collider.GetComponent<IGetHit>();
            if (getHit != null)
            {
                getHit.GetHit(damage * PlayerController.Instance.AttackStat);
            }
        }

        yield return new WaitForSeconds(lazerDuration);

        lineRenderer.enabled = false;
        isFiring = false;
    }
}