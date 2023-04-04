using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Entity
{
    public class LootManager : MonoBehaviour
    {
        public static LootManager self;

        public LootTableEntry[] LootTable;

        List<LootSpawnPoint> spawnPoints;

        SpawnTracker[] trackers;

        private void Awake()
        {
            self = this;
            spawnPoints = new List<LootSpawnPoint>();
        }

        private void Start()
        {
            trackers = new SpawnTracker[LootTable.Length];
            for(int n = 0; n < LootTable.Length; n++)
            {
                trackers[n] = new SpawnTracker(LootTable[n]);
            }
        }

        public static void RegisterSpawnPoint(LootSpawnPoint spawnPoint)
        {
            self.spawnPoints.Add(spawnPoint);
        }

        private void Update()
        {
            foreach(SpawnTracker tracker in trackers)
            {
                if (tracker.ShouldRespawn())
                {
                    tracker.RespawnAt(PickSpawnPoint());
                }
            }
        }

        LootSpawnPoint PickSpawnPoint()
        {
            Vector3 playerPosition = Camera.main.transform.position;

            var orderedPoints = spawnPoints
                .Where(sp => sp.Occupied == false)
                .OrderBy(sp => Vector3.SqrMagnitude(playerPosition - sp.transform.position))
                .ToList();

            int chosenIndex = Random.Range(0, orderedPoints.Count / 2);

            return orderedPoints[orderedPoints.Count - 1 - chosenIndex];
        }

        class SpawnTracker
        {
            public float lastChange;
            public LootTableEntry lootTableEntry;

            List<Loot> SpawnedLoot;
            List<Loot> DespawnedLoot;

            public bool ShouldRespawn()
            {
                return (SpawnedLoot.Count < lootTableEntry.MaximumCount
                    && lastChange + lootTableEntry.SecondsBetweenSpawns < Time.time);
            }

            public void RespawnAt(LootSpawnPoint spawnPoint)
            {
                Loot newLoot;

                if (DespawnedLoot.Count > 0)
                {
                    newLoot = DespawnedLoot[0];
                    DespawnedLoot.RemoveAt(0);
                }
                else
                {
                    newLoot = CreateNewLoot();
                }

                SpawnObject(newLoot, spawnPoint);
            }

            void SpawnObject(Loot loot, LootSpawnPoint spawnPoint)
            {
                loot.transform.parent = spawnPoint.transform;
                loot.transform.localPosition = Vector3.zero;
                spawnPoint.SetOccupant(loot);
                loot.OnLooted += OnDespawn;
                loot.Respawn();
                SpawnedLoot.Add(loot);

                lastChange = Time.time;
            }

            private void OnDespawn(object sender, System.EventArgs e)
            {
                Loot loot = sender as Loot;
                loot.OnLooted -= OnDespawn;
                SpawnedLoot.Remove(loot);
                DespawnedLoot.Add(loot);

                lastChange = Time.time;
            }

            Loot CreateNewLoot()
            {
                GameObject g = Instantiate(lootTableEntry.ItemPrefab);
                g.name = lootTableEntry.name;

                var lootSpawn = g.GetComponent<Loot>();

                return lootSpawn;
            }

            public SpawnTracker(LootTableEntry _lootTableEntry)
            {
                lootTableEntry = _lootTableEntry;
                lastChange = Time.time;
                SpawnedLoot = new List<Loot>();
                DespawnedLoot = new List<Loot>();
            }
        }
    }
}
