using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof ( WeaponManager))]
public class turret : NetworkBehaviour {

	private Transform target;
	public float range = 100f;
	public float fireRate = 1f;
	private float fireCountdown = 0f;
	public int damage = 6;


	private const string PLAYER_TAG = "Player";

	public Transform partToRotate;
	public float turnSpeed = 10f;

	public PlayerWeapon weapon;
	public GameObject line_of_sight;

	private PlayerWeapon currentWeapon;
	private WeaponManager weaponManager;

	[SerializeField]
	private LayerMask mask;



	// Use this for initialization
	void Start ()  {
		InvokeRepeating("UpdateTarget", 0f,0.5f);
		weaponManager = GetComponent<WeaponManager>();
		
	}
	void UpdateTarget()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Player");
		float shortestDistance = Mathf.Infinity;
		GameObject nearestEnemy = null;

		foreach (GameObject enemy in enemies) 
		{
			float distanceToEnemy = Vector3.Distance (transform.position, enemy.transform.position);
			if (distanceToEnemy < shortestDistance) 
			{
				shortestDistance = distanceToEnemy;
				nearestEnemy = enemy;
			}

		}
		if (nearestEnemy != null && shortestDistance <= range) 
		{
			target = nearestEnemy.transform;
		}
		else
		{
			target = null;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (PauseMenu.IsOn)
			return;
		if (target == null) 
		{
			return;
		}
		// target lock on
		Vector3 dir = target.position - transform.position;
		Quaternion lookRotation = Quaternion.LookRotation (dir);
		Vector3 rotation = Quaternion.Lerp (partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
		partToRotate.rotation = Quaternion.Euler(0f,rotation.y,0f);

		//fire
		if (fireCountdown <= 0f) 
		{
			Shoot ();
			fireCountdown = 1f / fireRate;
		}
		fireCountdown -= Time.deltaTime;
		
	}



	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position, range);
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
		if (weaponManager.GetCurrentGraphics() == null)
			return;
		for(int i = 0; i < weaponManager.GetCurrentGraphics().muzzleFlashs.Length; i++)
		{

			weaponManager.GetCurrentGraphics ().muzzleFlashs [i].Play ();
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
		GameObject InsImapctEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, partToRotate.rotation);
		Destroy(InsImapctEffect, 2f);
		//Quaternion.LookRotation(_normal)
	}


	[Client]
	void Shoot()
	{
		//When shooting on local, call on server the OnShoot
		CmdOnShoot();

		RaycastHit _hit;
		if (Physics.Raycast(partToRotate.transform.position, partToRotate.transform.forward, out _hit, range, mask))
		{
			if (_hit.collider.gameObject.tag == PLAYER_TAG)
			{
				Debug.Log("we hit"  + _hit.collider.gameObject.name);
				Transform hit_gameObject = _hit.collider.gameObject.transform.parent.parent;
				CmdPlayerShot(hit_gameObject.name, damage, transform.name);


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
