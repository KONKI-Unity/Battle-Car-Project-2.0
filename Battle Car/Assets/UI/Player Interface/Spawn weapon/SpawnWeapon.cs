using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWeapon : MonoBehaviour {

	
    public GameObject weapon;
    int spawnNum = 1;

    void spawn()
    {
        for (int i = 0; i < spawnNum; i++)
        {
            Vector3 weaponPos = new Vector3(this.transform.position.x + Random.Range(0.1f, 0.1f),
                this.transform.position.y + Random.Range(0.5f, 0.5f),
                this.transform.position.z + Random.Range(0.1f, 0.1f));
            Instantiate(weapon, weaponPos, Quaternion.identity);
        }
    }
    // Use this for initialization
    void Start()
    {
        spawn();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

