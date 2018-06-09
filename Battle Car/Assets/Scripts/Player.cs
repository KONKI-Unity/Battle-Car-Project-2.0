using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private int maxArmor = 100;
    

    [SerializeField]
    private int maxTurbo = 100;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    private int currentTurbo = 100;

    [SyncVar]
    public string username = "John Smith";

    public int kills;
    public int deaths;

    [SyncVar]    
    private int currentHealth;

    [SyncVar]    
    private int currentArmor;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    


    private GameObject uiInstance;

    public void Setup()
    {
        //Switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(false);
            uiInstance = GetComponent<PlayerSetup>().playerUIInstance;
            uiInstance.SetActive(true);
        }
        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();

    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {

        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        

        SetDefaults();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetCurrentArmor()
    {
        return currentArmor;
    }

    public int GetMaxTurbo()
    {
        return maxTurbo;
    }
    

    [ClientRpc]
    public void RpcTakeDamage(int _amount, string _sourceID)
    {
        if (isDead)
            return;

        if(currentArmor > 0)
        {
            currentArmor -= _amount;
            if(currentArmor< 0)
            {
                currentHealth += currentArmor;
                currentArmor = 0;
            }
        }
        else
        {
            currentHealth -= _amount;
        }
        

        Debug.Log(transform.name + " now has " + currentHealth + " health and " + currentArmor + " armor !");

        if (currentHealth <= 0)
        {
            
            Die(_sourceID);
        }
    }

    private void Die(string _sourceID)
    {
        isDead = true;
        deaths++;

        Player sourcePlayer = GameManager.GetPlayer(_sourceID);
        if(_sourceID != null && _sourceID != "")
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username, "killed");
        }

        //Disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Disable components
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }


        //Disable Collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        //Spawn death effects
        GameObject deathEffectIns = (GameObject) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(deathEffectIns, 3f);

        //Switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            //GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
            uiInstance.SetActive(false);
        }

        Debug.Log(transform.name + " is DEAD!");

        StartCoroutine(Respawn());
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(99999, "");
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        Setup();

        Debug.Log(transform.name + " respawned.");
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;
        currentArmor = maxArmor / 2;
        //Set Components active
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        //Enable Colliders
        Collider _col = GetComponent<Collider>();
        if (_col == null)
        {
            _col = GetComponentInChildren<Collider>();
        }
        if (_col != null)
            _col.enabled = true;
        

        //Create Spawn Effect
        GameObject spawnEffectIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(spawnEffectIns, 3f);
    }

}