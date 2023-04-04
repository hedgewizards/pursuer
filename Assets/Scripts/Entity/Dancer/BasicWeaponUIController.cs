using Entity.Weapon;
using UnityEngine;
using UnityEngine.UI;

namespace Entity.Dancer
{
    public class BasicWeaponUIController : WeaponUIController
    {
        public Text ClipAmount;
        public Text SpareAmount;

        private Color initialClipColor;
        public Color NoClipAmmoColor = Color.red;
        private Color initialSpareColor;
        public Color NoSpareAmmoColor = Color.red;

        private void Awake()
        {
            initialClipColor = ClipAmount.color;
            initialSpareColor = SpareAmount.color;
        }

        private void Update()
        {
            if(initialized)
            {
                int clipAmmo = self.Table.CurrentClip;
                int spareAmmo = self.WeaponHolder.AmmoTank.GetAmmoCount(self.Table.AmmoType);

                ClipAmount.text = clipAmmo.ToString();
                ClipAmount.color = clipAmmo == 0 ? NoClipAmmoColor : initialClipColor;
                
                SpareAmount.text = spareAmmo.ToString();
                SpareAmount.color = spareAmmo == 0 ? NoSpareAmmoColor : initialSpareColor;
            }
        }
    }
}
