using UnityEngine;

public class LoopingBG : MonoBehaviour
{
    [SerializeField] private float loopingSpeed;
    [SerializeField] private SpriteRenderer bgRenderer;
    private float bgWidth;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        bgWidth = bgRenderer.bounds.size.x;
    }

    private void Update()
    {
        transform.position += Vector3.right * loopingSpeed * Time.deltaTime;

        if (transform.position.x - startPos.x >= bgWidth/2)
        {
            transform.position = startPos;
        }
    }
}
