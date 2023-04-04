using System;
using UnityEngine;

namespace Entity
{
    public class LootSpawnPoint : MonoBehaviour
    {
        private bool occupied = false;
        public bool Occupied => occupied;

        private void Start()
        {
            LootManager.RegisterSpawnPoint(this);
        }

        public void SetOccupant(Loot loot)
        {
            occupied = true;
            loot.OnLooted += OnLooted;
        }
        private void OnLooted(object sender, EventArgs e)
        {
            (sender as Loot).OnLooted -= OnLooted;
            occupied = false;
        }
    }
}
