
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform turboFill;

    private CarUserControl controller;

    public void SetController (CarUserControl _controller)
    {
        controller = _controller;
        
    }

    void Update()
    {
        if (controller != null)
            SetFuelAmount(controller.GetTurboAmount(), controller.GetMaxTurbo());
    }


    void SetFuelAmount(float _amount, float _max)
    {
        
        float turboPercentage = (_amount / 100);
        turboFill.localScale = new Vector3(1f, turboPercentage, 1f);


    }

}
