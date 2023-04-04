using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new LootTableEntry", menuName = "ScriptableObjects/Entity/LootTableEntry", order = 1)]
public class LootTableEntry : ScriptableObject
{
    public GameObject ItemPrefab;

    public int MaximumCount = 1;
    public float SecondsBetweenSpawns = 0;
}