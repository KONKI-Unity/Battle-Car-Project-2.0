using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


[RequireComponent(typeof ( WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{

    private const string PLAYER_TAG = "Player";
    public PlayerWeapon weapon;
    public GameObject line_of_sight;




    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    
    [SerializeField]
    private LayerMask mask;

    void Start()
    {

        weaponManager = GetComponent<WeaponManager>();

    }

    void Update()
    {

        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.IsOn)
            return;

        if(currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if (Input.GetButtonDown("Reload"))
            {
                weaponManager.Reload();
                return;
            }
        }

        

        if(currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
                
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1/currentWeapon.fireRate);
            }
            else if( Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }


    //Is called on the server when a player shoots
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    //Is called on all clients to do shoot effects
    [ClientRpc]
    void RpcDoShootEffect()
    {
        for(int i = 0; i < weaponManager.GetCurrentGraphics().muzzleFlashs.Length; i++)
        {
            weaponManager.GetCurrentGraphics().muzzleFlashs[i].Play();
        }
    }

    //Is called when on the server when hit something
    //Spawn in hit impacts particles
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoImpactEffect(_pos, _normal);
    }

    //Is called on all clients to do impact effects
    [ClientRpc]
    void RpcDoImpactEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject InsImapctEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(InsImapctEffect, 2f);
    }


    [Client]
    void Shoot()
    {

        if (!isLocalPlayer && weaponManager == false)
        {
            return;
        }

        if(currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }

        currentWeapon.bullets--;

        //When shooting on local, call on server the OnShoot
        CmdOnShoot();

        RaycastHit _hit;
        if (Physics.Raycast(line_of_sight.transform.position, line_of_sight.transform.forward, out _hit, weapon.range, mask))
        {

            Debug.Log(_hit.collider.gameObject.name);
            if (_hit.collider.gameObject.tag == PLAYER_TAG)
            {
                Transform hit_gameObject = _hit.collider.gameObject.transform.parent.parent;
                CmdPlayerShot(hit_gameObject.name, currentWeapon.damage, transform.name);


                //DebugLogs
                Debug.Log(hit_gameObject.name + "got hit");
                Player _player = GameManager.GetPlayer(hit_gameObject.name);
                Debug.Log(_player.name + " has been shot.");
            }

            //We hit something, call OnHit method on the server
            CmdOnHit(_hit.point, _hit.normal);
        }

    }

    [Command]
    void CmdPlayerShot(string _playerID, int _damage, string _sourceID)
    {

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage, _sourceID);
        Debug.Log(_playerID + " has been shot.");
        int debugPlayerHitHealth = _player.GetCurrentHealth();
        Debug.Log(_playerID + " has now " + debugPlayerHitHealth.ToString());

    }

}