using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawn : MonoBehaviour {

    public int respawnTime = 100;



    [SerializeField]
    private GameObject[] weaponParts;

    void OnCollisionEnter()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
        for (int i = 0; i < weaponParts.Length; i++){
            Debug.Log("test");
            if(weaponParts[i].GetComponent<BoxCollider>() != null)
                weaponParts[i].GetComponent<BoxCollider>().enabled = false;
            if (weaponParts[i].GetComponent<MeshRenderer>() != null)
                weaponParts[i].GetComponent<MeshRenderer>().enabled = false;

        }

        Invoke("Respawn", respawnTime);
    }


    void Respawn()
    {
        this.GetComponent<BoxCollider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
        for (int i = 0; i < weaponParts.Length; i++)
        {

            if (weaponParts[i].GetComponent<BoxCollider>() != null)
                weaponParts[i].GetComponent<BoxCollider>().enabled = true;
            if (weaponParts[i].GetComponent<MeshRenderer>() != null)
                weaponParts[i].GetComponent<MeshRenderer>().enabled = true;

        }
    }


}
