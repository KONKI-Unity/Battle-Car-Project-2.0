
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform turboFill;

    [SerializeField]
    Text healthBarText;

    [SerializeField]
    Text ammoText;

    [SerializeField]
    Text armorText;

    [SerializeField]
    Text healthText;


    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject scoreboard;

    private Player player;
    private CarUserControl controller;
    private WeaponManager weaponManager;

  
    public void SetPlayer(Player _player)
    {
        player = _player;
        controller = player.GetComponent<CarUserControl>();
        weaponManager = player.GetComponent<WeaponManager>();

    }

    private void Start()
    {
        PauseMenu.IsOn = false;
    }

    void Update()
    {
        if (controller != null)
            SetFuelAmount(controller.GetTurboAmount(), controller.GetMaxTurbo());

        SetAmmoAmount(weaponManager.GetCurrentWeapon().bullets);
        SetHealthAmount(player.GetCurrentHealth());
        SetArmorAmount(player.GetCurrentArmor());
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(true);

        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }

    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;
    }


    void SetFuelAmount(float _amount, float _max)
    {
        
        float turboPercentage = (_amount / 100);
        turboFill.localScale = new Vector3(1f, turboPercentage, 1f);


    }



    void SetHealthAmount(int _amount)
    {
        healthBarText.text = _amount.ToString();
    }

    void SetAmmoAmount(int _amount)
    {
        ammoText.text = _amount.ToString();
    }

    void SetArmorAmount(int _amount)
    {
        armorText.text = _amount.ToString();
    }

}
