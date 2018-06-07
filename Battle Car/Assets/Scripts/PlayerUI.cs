
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
    GameObject pauseMenu;

    [SerializeField]
    GameObject scoreboard;

    private CarUserControl controller;

    public void SetController (CarUserControl _controller)
    {
        controller = _controller;
        
    }

    private void Start()
    {
        PauseMenu.IsOn = false;
    }

    void Update()
    {
        if (controller != null)
            SetFuelAmount(controller.GetTurboAmount(), controller.GetMaxTurbo());
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
