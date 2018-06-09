using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{

    public string name = "gun";



    public int damage = 11;
    public float range = 100f;

    public float fireRate = 0f;

    public int maxBullets = 30;
    [HideInInspector]
    public int bullets;
    
    public int reloadTime = 4;

    public GameObject graphics;

    public PlayerWeapon()
    {
        bullets = maxBullets;
    }

}