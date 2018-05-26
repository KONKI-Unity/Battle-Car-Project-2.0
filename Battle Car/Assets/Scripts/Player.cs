using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]    //Sync Var in every Clients of the game
    private int currentHealth;

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void Awake()
    {
        SetDefaults();
    }


    //Inflict dammages locally
    public void TakeDamage(int _amount)
    {
        currentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");
    }

    public void SetDefaults()
    {
        currentHealth = maxHealth;
    }

}