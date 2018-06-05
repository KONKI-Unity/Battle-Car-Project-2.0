
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform turboFill;

    [SerializeField]
    GameObject pauseMenu;

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

    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;
    }


    void SetFuelAmount(float _amount, float _max)
    {
        
        float turboPercentage = (_amount / 100);
        turboFill.localScale = new Vector3(1f, turboPercentage, 1f);


    }

}
