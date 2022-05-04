using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class BlockRenderer : MonoBehaviour
{
    public Color color;
    public Block block;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _spriteRenderer.color = color;
    }

    private void Update()
    {
        if (Application.isPlaying || !transform.hasChanged)
            return;

        block.width = transform.localScale.x;
        block.height = transform.localScale.y;
    }

    private void OnValidate()
    {
        GetComponent<SpriteRenderer>().color = color;
        transform.localScale = new Vector3(block.width, block.height, 1);
    }
}
