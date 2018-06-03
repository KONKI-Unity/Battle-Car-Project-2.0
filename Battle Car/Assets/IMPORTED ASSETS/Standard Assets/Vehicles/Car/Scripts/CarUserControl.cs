using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        private Animator animator;
        [SerializeField]
        private GameObject[] wheels = new GameObject[4];
        [SerializeField]
        private GameObject CarObject;

        void Start()
        {

            animator = GetComponent<Animator>();

        }

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }
        private void Update()
        {
            if (wheels.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    //Debug.Log("Speed : " + m_Car.CurrentSpeed * Time.deltaTime);
                    wheels[i].transform.Rotate(0, -m_Car.CurrentSpeed*100 / (60 * 360 * Time.deltaTime), 0);
                }
            }
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
            
                
            //Debug.Log("Speed : " + m_Car.CurrentSpeed);
            animator.SetFloat("ForwardVelocity", h);
           
        }
    }
}
