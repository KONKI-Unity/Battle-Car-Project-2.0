
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;

    public bool isReloading = false;

    [SerializeField]
    private GameObject weaponGFX;
    [SerializeField]
    private string weaponLayerName = "weapon";


    void Start () {
        EquipWeapon(primaryWeapon);
	}

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;

        GameObject _weaponIns = (GameObject) Instantiate(_weapon.graphics,weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();

        if (currentGraphics == null)
            Debug.Log("No WeaponGraphics component on the weapon object :" + _weaponIns.name);

        //LAYER WEAPON IS 12 -- Do Not Change it
        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns, 12);
    }

    public void Reload()
    {
        if (isReloading)
            return;
        StartCoroutine(reloadSequence());
    }

    private IEnumerator reloadSequence()
    {
        Debug.Log("RELOOOOAD");
        isReloading = true;
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        currentWeapon.bullets = currentWeapon.maxBullets;
        isReloading = false;
    }

	
}
