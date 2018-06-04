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



        private Player player;
        private float turboAmount = 1f;

        private float maxTurbo = 100f;

        [SerializeField]
        private float nitroBurnSpeed = 1f;
        [SerializeField]
        private float nitroRegenSpeed = 0.3f;

        public float GetTurboAmount()
        {
            return turboAmount;
        }

        public float GetMaxTurbo()
        {
            return maxTurbo;
        }


        void Start()
        {
            maxTurbo = player.GetMaxTurbo();
            turboAmount = 0f;

            animator = GetComponent<Animator>();
            if (animator == null)
                Debug.Log("No Animator found");

        }

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }
        private void Update()
        {
            

            //Wheels Spinning
            if (wheels.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    //Debug.Log("Speed : " + m_Car.CurrentSpeed * Time.deltaTime);
                    wheels[i].transform.Rotate(0, -m_Car.CurrentSpeed*100 / (60 * 360 * Time.deltaTime), 0);
                }
            }

            bool turbo = Input.GetKey(KeyCode.C);
            if (turbo && turboAmount > 0.5f)
            {
                Debug.Log("Turbooo !!");
                turboAmount -= nitroBurnSpeed * Time.deltaTime;
            }
            else
            {
                turboAmount += nitroRegenSpeed * Time.deltaTime;
            }

            turboAmount = Mathf.Clamp(turboAmount, 0f, maxTurbo);
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            

#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake, Input.GetKey(KeyCode.C));
#else
            m_Car.Move(h, v, v, 0f, turbo);
#endif


            //Debug.Log("Speed : " + m_Car.CurrentSpeed);
            if (animator != null)
                animator.SetFloat("ForwardVelocity", h);
           
        }
    }
}
