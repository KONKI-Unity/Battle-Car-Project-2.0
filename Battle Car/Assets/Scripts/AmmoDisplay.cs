
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{ 
    [SerializeField]
    private Text ammoCount;

    [SerializeField]
    private Player player;

    private void Start()
    { 
        ammoCount.text = player.GetComponent<WeaponManager>().GetCurrentWeapon().bullets.ToString();
    }


    void Update()
    {
        int ammo = player.GetComponent<WeaponManager>().GetCurrentWeapon().bullets;
        if (ammo > 0)
        {
            ammoCount.color = new Color32(255, 255, 255, 255);
        } else if (player.GetComponent<WeaponManager>().isReloading)
        {
            ammoCount.color = new Color32(202, 41, 41, 255);
        }

        if(ammo < 10)
            ammoCount.text = "0" + ammo.ToString();
        else
            ammoCount.text = ammo.ToString();
        
    }

}