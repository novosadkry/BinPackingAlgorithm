using UnityEngine;

[SelectionBase]
public class BlockRenderer : MonoBehaviour
{
    public Block block;
    public bool scaleSprite;

    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnValidate()
    {
        transform.rotation = Quaternion.Euler(0, 0, block.rotation);

        if (scaleSprite)
        {
            GetComponentInChildren<SpriteRenderer>().transform.localScale =
                new Vector2(block.width, block.height);
        }
    }
}
