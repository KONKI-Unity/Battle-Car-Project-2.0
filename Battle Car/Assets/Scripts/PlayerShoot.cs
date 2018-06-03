using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


[RequireComponent(typeof ( WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{

    private const string PLAYER_TAG = "Player";
    public PlayerWeapon weapon;
    public GameObject line_of_sight;
    LineRenderer line;



    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    
    [SerializeField]
    private LayerMask mask;

    void Start()
    {
        //DebugRays
        line = gameObject.GetComponentInChildren<LineRenderer>();
        line.enabled = false;


        weaponManager = GetComponent<WeaponManager>();

    }

    void Update()
    {

        currentWeapon = weaponManager.GetCurrentWeapon();
        if (Input.GetButtonDown("Fire1"))
        {

            StopCoroutine("FireLaser");
            StartCoroutine("FireLaser");
            Shoot();
        }
    }

    IEnumerator FireLaser()
    {
        line.enabled = true;
        while (Input.GetButtonDown("Fire1"))
        {
            Ray ray = new Ray(transform.position, transform.forward);


            line.SetPosition(0, ray.origin);
            line.SetPosition(1, ray.GetPoint(weapon.range));
            yield return null;
        }
        line.enabled = false;
    }

    [Client]
    void Shoot()
    {
        RaycastHit _hit;
        if (Physics.Raycast(line_of_sight.transform.position, line_of_sight.transform.forward, out _hit, weapon.range, mask))
        {
            Debug.Log(_hit.collider.gameObject.name);
            if (_hit.collider.gameObject.tag == PLAYER_TAG)
            {
                Transform hit_gameObject = _hit.collider.gameObject.transform.parent.parent;
                CmdPlayerShot(hit_gameObject.name, currentWeapon.damage);


                //DebugLogs
                Debug.Log(hit_gameObject.name + "got hit");
                Player _player = GameManager.GetPlayer(hit_gameObject.name);
                Debug.Log(_player.name + " has been shot.");
            }
        }

    }

    [Command]
    void CmdPlayerShot(string _playerID, int _damage)
    {

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
        Debug.Log(_playerID + " has been shot.");
        int debugPlayerHitHealth = _player.GetCurrentHealth();
        Debug.Log(_playerID + " has now " + debugPlayerHitHealth.ToString());

    }

}