using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public Stickman stickmanPrefab;

    private readonly Dictionary<StickmanColor, Queue<Stickman>> stickmanPool = new();
    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (StickmanColor color in Enum.GetValues(typeof(StickmanColor)))
        {
            var stickmanQueue = new Queue<Stickman>();

            for (var i = 0; i < 16; i++)
            {
                var stickman = Instantiate(stickmanPrefab);
                stickman.Color = color;
                stickman.name = color + " " + i;
                stickman.gameObject.SetActive(false);

                stickman.transform.parent = transform; // Parent the Stickman under the PoolManager object

                stickmanQueue.Enqueue(stickman);
            }

            stickmanPool[color] = stickmanQueue;
        }
    }

    public Stickman GetStickmanFromPool(StickmanColor color)
    {
        if (!stickmanPool.ContainsKey(color)) stickmanPool[color] = new Queue<Stickman>();

        Stickman stickman = null;

        if (stickmanPool[color].Count > 0)
        {
            stickman = stickmanPool[color].Dequeue();
            stickman.gameObject.SetActive(true);
        }

        return stickman;
    }

    public void ReturnStickmanToPool(Stickman stickman)
    {
        var color = stickman.Color;

        if (!stickmanPool.ContainsKey(color)) stickmanPool[color] = new Queue<Stickman>();

        stickman.gameObject.SetActive(false);
        stickmanPool[color].Enqueue(stickman);
    }
}