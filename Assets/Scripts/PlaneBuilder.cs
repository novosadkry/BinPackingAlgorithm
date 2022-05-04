﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaneBuilder))]
internal class PlaneBuilderEditor : Editor
{
    private PlaneBuilder builder;

    private void Awake()
    {
        builder = target as PlaneBuilder;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Load Blocks"))
            builder.LoadBlocks();

        if (GUILayout.Button("Run"))
            builder.Run();
    }
}

public class PlaneBuilder : MonoBehaviour
{
    public BlockRenderer planeRenderer;
    public List<BlockRenderer> blockRenderers;

    [Header("Settings")]
    public bool visualize;

    public void LoadBlocks()
    {
        blockRenderers.Clear();

        foreach (var render in FindObjectsOfType<BlockRenderer>())
        {
            if (render != planeRenderer)
                blockRenderers.Add(render);
        }

        blockRenderers = blockRenderers
            .OrderByDescending(x => x.block.width * x.block.height)
            .ToList();
    }

    public void Run()
    {
        var packer = new Packer(planeRenderer.block);
        packer.Fit(blockRenderers.Select(x => x.block));

        StartCoroutine(TransformBlocks());
    }

    private IEnumerator TransformBlocks()
    {
        var plane = planeRenderer.transform;

        var planeWidth = planeRenderer.block.width;
        var planeHeight = planeRenderer.block.height;
        var planeCollider = planeRenderer.collider2D;

        foreach (var render in blockRenderers)
        {
            var node = render.block.fit;
            if (!node.used) continue;

            var x = node.position.x;
            var y = node.position.y;

            var width = render.block.width;
            var height = render.block.height;
            var rotation = render.block.rotation;

            var pos = new Vector2(
                       (x / planeWidth  - 0.5f) + (width  / planeWidth  * 0.5f),
                1.0f - (y / planeHeight + 0.5f) - (height / planeHeight * 0.5f)
            );

            pos.Scale(planeCollider.size);

            render.transform.position = plane.TransformPoint(pos);
            render.transform.rotation = Quaternion.Euler(0, 0, rotation);

            if (visualize)
                yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }
}
