using UnityEngine;

[SelectionBase]
[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class BlockRenderer : MonoBehaviour
{
    public Color color;
    public Block block;

    [HideInInspector] public BoxCollider2D collider2D;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        collider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.color = color;
    }

    private void Update()
    {
        if (Application.isPlaying || !transform.hasChanged)
            return;

        if (collider2D == null)
        {
            collider2D = GetComponent<BoxCollider2D>();
            return;
        }

        block.width = collider2D.size.x;
        block.height = collider2D.size.y;
    }

    private void OnValidate()
    {
        GetComponentInChildren<SpriteRenderer>().color = color;
        GetComponent<BoxCollider2D>().size = new Vector2(block.width, block.height);
    }
}
