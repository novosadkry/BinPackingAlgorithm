using System.Collections.Generic;
using UnityEngine;

public class Packer
{
    private PackerNode root;

    public Packer(Block plane)
    {
        root = new PackerNode {
            width = plane.width,
            height = plane.height
        };
    }

    /// <summary>
    /// Performs bin-packing algorithm
    /// </summary>
    public void Fit(IEnumerable<Block> blocks)
    {
        foreach (var block in blocks)
        {
            var node = FindNode(root, block);
            if (node != null)
                block.fit = SplitNode(node, block);
        }
    }

    /// <summary>
    /// Finds node which can fit the block
    /// </summary>
    private static PackerNode FindNode(PackerNode node, Block block)
    {
        // Search deeper if this block is already occupied
        if (node.used)
        {
            // Try right-side first
            var right = FindNode(node.right, block);
            if (right != null)
                return right;

            // then try going down
            var down = FindNode(node.down, block);
            return down;
        }

        // Check if this node can fit the block
        if (block.width <= node.width && block.height <= node.height)
            return node;

        // Rotate block to fit
        if (block.height <= node.width && block.width <= node.height)
        {
            (block.height, block.width) = (block.width, block.height);
            return node;
        }

        // Return null if there is no space
        return null;
    }

    /// <summary>
    /// Splits the area of this node by the block's dimensions
    /// </summary>
    private static PackerNode SplitNode(PackerNode node, Block block)
    {
        node.used = true;

        node.down = new PackerNode {
            position = new Vector2(
                node.position.x,
                node.position.y + block.height
            ),
            width = node.width,
            height = node.height - block.height,
        };

        node.right = new PackerNode {
            position = new Vector2(
                node.position.x + block.width,
                node.position.y
            ),
            width = node.width - block.width,
            height = block.height,
        };

        return node;
    }
}
