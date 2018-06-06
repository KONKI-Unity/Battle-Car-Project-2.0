using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PickUp : MonoBehaviour {

    public GameObject inventoryPanel;
    public GameObject[] inventoryIcons;

    [SerializeField]
    private string[] weaponNames;

    private string WEAPON_TAG = "WEAPON_TAG";
    void OnCollisionEnter(Collision collision)
    { 
       
        GameObject i;
        if (collision.gameObject.tag == WEAPON_TAG)
        {
            Debug.Log(collision.gameObject.name);
            switch(collision.gameObject.name)
            {
                case"M4A1":
                    Debug.Log("picked M4A1");
                    break;

            }

            i = Instantiate(inventoryIcons[0]);
            i.transform.SetParent(inventoryPanel.transform);
        }

    }



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
