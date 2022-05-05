﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (GUILayout.Button("Run") && Application.isPlaying)
            builder.Run();

        if (GUILayout.Button("Reset"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}


public class PlaneBuilder : MonoBehaviour
{
    public BlockRenderer planePrefab;
    public List<BlockRenderer> blockRenderers;

    private List<BlockRenderer> planes;

    [Header("Settings")]
    public float planeWidth = 10;
    public float planeHeight = 5;
    public bool visualize;

    private void Awake()
    {
        planes = new List<BlockRenderer>();
    }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        blockRenderers.Clear();
        blockRenderers.AddRange(
            FindObjectsOfType<BlockRenderer>()
                .Except(planes)
        );

        foreach (var plane in planes)
            Destroy(plane.gameObject);

        planes.Clear();
    }

    public void Run()
    {
        Setup();

        var blocks = blockRenderers
            .OrderByDescending(x => x.block.width * x.block.height)
            .ToList();

        // Reset block used status
        foreach (var value in blocks)
            value.block.fit.used = false;

        while (true)
        {
            var root = new Block {
                width = planeWidth,
                height = planeHeight
            };

            var packer = new Packer(root);
            packer.Fit(blocks.Select(x => x.block));

            var used = blocks
                .Where(value => value.block.fit.used)
                .ToList();

            if (used.Count < 1)
                return;

            var plane = Instantiate(planePrefab);
            plane.block = root;
            plane.spriteRenderer.transform.localScale =
                new Vector2(planeWidth, planeHeight);
            plane.transform.position +=
                Vector3.down * (planeHeight + 1) * planes.Count;
            planes.Add(plane);

            StartCoroutine(TransformBlocks(plane, used));

            blocks = blocks
                .Except(used)
                .OrderByDescending(x => x.block.width * x.block.height)
                .ToList();
        }
    }

    private IEnumerator TransformBlocks(BlockRenderer plane, IEnumerable<BlockRenderer> blocks)
    {
        var planeWidth = plane.block.width;
        var planeHeight = plane.block.height;

        foreach (var render in blocks)
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

            pos.Scale(new Vector2(planeWidth, planeHeight));

            render.transform.position = plane.transform.TransformPoint(pos);
            render.transform.rotation = Quaternion.Euler(0, 0, rotation);

            if (visualize)
                yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }
}
