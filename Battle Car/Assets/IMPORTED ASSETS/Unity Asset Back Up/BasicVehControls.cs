using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicVehControls : MonoBehaviour
{

    public int bhp;
    public float torque;
    public int brakeTorque;

    public float[] gearRatio;
    public int currentGear;

    public WheelCollider FL;
    public WheelCollider FR;
    public WheelCollider RL;
    public WheelCollider RR;

    public float currentSpeed;
    public int maxSpeed;
    public int maxRevSpeed;

    public float SteerAngle;

    public float engineRPM;
    public float gearUpRPM;
    public float gearDownRPM;
    private GameObject COM;

	public bool handBraked;

	public List<AudioSource> CarSound;

	public float[] MinRpmTable = {50.0f, 75.0f, 112.0f, 166.9f, 222.4f, 278.3f, 333.5f, 388.2f, 435.5f, 483.3f, 538.4f, 594.3f, 643.6f, 692.8f, 741.9f, 790.0f};
	public float[] NormalRpmTable = {72.0f, 93.0f, 155.9f, 202.8f, 267.0f, 314.5f, 377.4f, 423.9f, 472.1f, 519.4f, 582.3f, 631.3f, 680.8f, 729.4f, 778.8f, 826.1f};
	public float[] MaxRpmTable = {92.0f, 136.0f, 182.9f, 247.4f, 294.3f, 357.5f, 403.6f, 452.5f, 499.3f, 562.5f, 612.3f, 661.6f, 708.8f, 758.9f, 806.0f, 1000.0f};
	public float[] PitchingTable = {0.12f, 0.12f, 0.12f, 0.12f, 0.11f, 0.10f, 0.09f, 0.08f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f};
	public float RangeDivider = 4f;
	public float soundRPM;

    void Start()
    {

        FL = GameObject.Find("FL.Col").GetComponent<WheelCollider>();
        FR = GameObject.Find("FR.Col").GetComponent<WheelCollider>();
        RL = GameObject.Find("RL.Col").GetComponent<WheelCollider>();
        RR = GameObject.Find("RR.Col").GetComponent<WheelCollider>();
		
        COM = GameObject.Find("Col");
        GetComponent<Rigidbody>().centerOfMass = new Vector3(COM.transform.localPosition.x * transform.localScale.x, COM.transform.localPosition.y * transform.localScale.y, COM.transform.localPosition.z * transform.localScale.z);		            

		for(int i =1; i<=16; ++i) 
		{
			CarSound.Add(GameObject.Find(string.Format("CarSound ({0})",i)).GetComponent<AudioSource>());
			CarSound[i-1].Play();
		}

    }
	

    void Update()
    {
		//Functions to access.
        Steer();
		AutoGears();
		Accelerate();
		carSounds ();

		//Defenitions.
        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        engineRPM = Mathf.Round((RL.rpm * gearRatio[currentGear]));
		soundRPM = Mathf.Round(engineRPM * (1000 / 420));
        torque = bhp * gearRatio[currentGear];     

        if (Input.GetButton("Jump"))
        {
            HandBrakes();
        }
		
		if (Input.GetKey(KeyCode.R)) {

			transform.position.Set(transform.position.x, transform.position.y + 5f, transform.position.z);
			transform.rotation.Set(0,0,0,0);
		}
    }


	//Function

    void Accelerate()
    {

        if (currentSpeed < maxSpeed && currentSpeed > maxRevSpeed && engineRPM <= gearUpRPM)
        {

            RL.motorTorque = torque * Input.GetAxis("Vertical");
            RR.motorTorque = torque * Input.GetAxis("Vertical");
            RL.brakeTorque = 0;
            RR.brakeTorque = 0;
        }
        else
        {

            RL.motorTorque = 0;
            RR.motorTorque = 0;
            RL.brakeTorque = brakeTorque;
            RR.brakeTorque = brakeTorque;
        }

		if (engineRPM > 0 && Input.GetAxis("Vertical") < 0 && engineRPM <= gearUpRPM)
		{
			
            FL.brakeTorque = brakeTorque;
            FR.brakeTorque = brakeTorque;
        }
        else
        {
            FL.brakeTorque = 0;
            FR.brakeTorque = 0;
        }
    }

    void Steer()
    {

        if (currentSpeed < 100)
        {
            SteerAngle = 13 - (currentSpeed / 10);
        }
        else
        {
            SteerAngle = 2;
        }

        FL.steerAngle = SteerAngle * Input.GetAxis("Horizontal");
        FR.steerAngle = SteerAngle * Input.GetAxis("Horizontal");
    }


    void AutoGears()
    {

        int AppropriateGear = currentGear;

        if (engineRPM >= gearUpRPM)
        {

            for (var i = 0; i < gearRatio.Length; i++)
            {
                if (RL.rpm * gearRatio[i] < gearUpRPM)
                {
                    AppropriateGear = i;
                    break;
                }
            }
            currentGear = AppropriateGear;
        }

        if (engineRPM <= gearDownRPM)
        {
            AppropriateGear = currentGear;
            for (var j = gearRatio.Length - 1; j >= 0; j--)
            {
                if (RL.rpm * gearRatio[j] > gearDownRPM)
                {
                    AppropriateGear = j;
                    break;
                }
            }
            currentGear = AppropriateGear;
        }
    }

    void HandBrakes()
    {

        RL.brakeTorque = brakeTorque;
        RR.brakeTorque = brakeTorque;
        FL.brakeTorque = brakeTorque;
        FR.brakeTorque = brakeTorque;
    }

	void carSounds()
	{

		for (int i = 0; i < 16; i++) {
			if (i == 0) {
				//Set CarSound[0]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[0].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[0].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[0].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[0].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[0].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[0].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[0].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 1) {
				//Set CarSound[1]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[1].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[1].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[1].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[1].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[1].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[1].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[1].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 2) {
				//Set CarSound[2]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[2].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[2].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[2].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[2].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[2].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[2].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[2].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 3) {
				//Set CarSound[3]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[3].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[3].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[3].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[3].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[3].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[3].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[3].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 4) {
				//Set CarSound[4]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[4].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[4].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[4].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[4].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[4].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[4].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[4].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 5) {
				//Set CarSound[5]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[5].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[5].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[5].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[5].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[5].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[5].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[5].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 6) {
				//Set CarSound[6]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[6].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[6].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[6].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[6].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[6].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[6].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[6].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 7) {
				//Set CarSound[7]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[7].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[7].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[7].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[7].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[7].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[7].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[7].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 8) {
				//Set CarSound[8]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[8].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[8].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[8].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[8].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[8].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[8].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[8].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 9) {
				//Set CarSound[9]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[9].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[9].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[9].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[9].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[9].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[9].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[9].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 10) {
				//Set CarSound[10]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[10].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[10].volume = ((ReducedRPM / Range) * 2f) - 1f;
					//CarSound[10].volume = 0.0f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[10].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[10].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[10].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[10].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[10].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 11) {
				//Set CarSound[11]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[11].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[11].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[11].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[11].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[11].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[11].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[11].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 12) {
				//Set CarSound[12]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[12].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[12].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[12].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[12].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[12].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[12].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[12].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 13) {
				//Set CarSound[13]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[13].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[13].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[13].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[13].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[13].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[13].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[13].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 14) {
				//Set CarSound[14]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[14].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[14].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[14].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[14].volume = 1f;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[14].pitch = 1f + PitchMath;
				} else if (soundRPM > MaxRpmTable[i]) {
					float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
					float ReducedRPM = soundRPM - MaxRpmTable[i];
					CarSound[14].volume = 1f - ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					//CarSound[14].pitch = 1f + PitchingTable[i] + PitchMath;
				}
			} else if (i == 15) {
				//Set CarSound[15]
				if (soundRPM < MinRpmTable[i]) {
					CarSound[15].volume = 0.0f;
				} else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
					float Range = NormalRpmTable[i] - MinRpmTable[i];
					float ReducedRPM = soundRPM - MinRpmTable[i];
					CarSound[15].volume = ReducedRPM / Range;
					float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
					CarSound[15].pitch = 1f - PitchingTable[i] + PitchMath;
				} else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
					float Range = MaxRpmTable[i] - NormalRpmTable[i];
					float ReducedRPM = soundRPM - NormalRpmTable[i];
					CarSound[15].volume = 1f;
					float PitchMath = (ReducedRPM * (PitchingTable[i] + 0.1f)) / Range;
					CarSound[15].pitch = 1f + PitchMath;
				}
			}
		}
	}
	}