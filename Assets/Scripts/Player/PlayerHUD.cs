using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Text spdText;
    
    //hp stuff
    public ResourceUI HealthBar;
    private GameObject floaterObj;

    //messages
    
    private void Start()
    {
        HealthBar.SetMaxValues(PlayerController.HP_MAX, PlayerController.ARMOR_MAX);
    }
    private void Update()
    {
    }

    //public
    public void UpdateSpdText(float spd)
    {
        spdText.text = spd.ToString("N2");
    }
    public void UpdateHealth(float HP)
    {
        HealthBar.Value1 = HP;
    }
    public void UpdateArmor(float ARMOR)
    {
        HealthBar.Value2 = ARMOR;
    }
    public void CreateHealthFloater(float amount)
    {
        if( amount >= 1 || amount <= -1)
            HealthBar.CreateFloater1((int)amount);
    }
    public void CreateArmorFloater(float amount)
    {
        if (amount >= 1 || amount <= -1)
            HealthBar.CreateFloater2((int)amount);
    }
    public GameObject DrawPrefab(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        return obj;
    }
}
