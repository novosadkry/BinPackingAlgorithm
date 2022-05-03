using System;
using UnityEngine;

[Serializable]
public class PackerNode
{
    public PackerNode right;
    public PackerNode down;

    public float width;
    public float height;
    public Vector2 position;

    public bool used;
}
