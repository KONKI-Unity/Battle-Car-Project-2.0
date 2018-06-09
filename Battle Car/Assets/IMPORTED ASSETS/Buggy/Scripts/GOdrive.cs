using UnityEngine;
using System.Collections;

// goDrive is the lite version of the GO script
// used to control the cars in the ARK project.
// ARK = Arcade Racing Kit by New Creations!

[RequireComponent(typeof(Rigidbody))]
public class GOdrive : MonoBehaviour {
	[SerializeField] public float topSpeedMPH = 85.0f;			// Top speed in miles per hour.
	[SerializeField] public float accelTime = 3.0f;				// The time it takes to reach top speed.
	[SerializeField] public float traction = 0.7f;	
	
	[SerializeField] public Transform frontLeftWheelTransform;	// Transform  for front left wheel.
	[SerializeField] public Transform frontRightWheelTransform;	// Transform  for front right wheel.
	[SerializeField] public Transform rearLeftWheelTransform;	// Transform  for rear left wheel.
	[SerializeField] public Transform rearRightWheelTransform;	// Transform  for rear right wheel.

	[SerializeField] public float maximumSteeringAngle = 40.0f;	// How sharp of a turn can this vechicle make?
	
	[SerializeField] public float offTrackDragMultiplier = 2.0f;// When contacting terrian with 0ffTrack Tag increase Drag.
	
	private float thrust = 0.0f;			// Current thrust value.
	private float steer = 0.0f;				// Current steering value.
	private float currentMPH = 0.0f;		// Current speed in MPH.
	
	private bool isGrounded = true;			// Does the vehicle have traction?
	private bool isOffTrack = false;		// Has the vehicle left the track?
	
	private Rigidbody physicsBody;			// Rigidbody for controlling the physics of the vehicle.
	
	private WheelCollider wheelCollider, wheelColliderFrontRight, wheelColliderFrontLeft, wheelColliderRearRight, wheelColliderRearLeft;
	private GameObject steerRightFront, steerLeftFront, steerOneWheel;

	private Vector3 myDrag, myLocalVelocity, myGlobalVelocity, myWheelRayCastLocation, myWheelRayCastLocationFront, myWheelRayCastLocationRear;

	private float myFrontWheelRadius;		// radius of the front wheels in meters
	private float myRearWheelRadius;		// radius of the rear wheel in meters
	private float wheelRotationFront;
	//private float wheelRotationRear;      // Will use this in a future update.
	private float steerAngle;

	private float tempSpeedVariable, myCalculatedThrust, myAirMomentum, thrust_myAirMomentum,thrust_CurrentVelocity;
	private bool buildingMomentum, buildingReverseMomentum;
	private float accelerationTime, accel;

	private RaycastHit hit;
	 
	
	void Start () 
	{ 
		physicsBody = GetComponent<Rigidbody>();
		physicsBody.centerOfMass = Vector3.up * -0.25f;

		wheelColliderFrontLeft = transform.Find("WheelColliders/FrontLeft").GetComponent<WheelCollider>();
		myFrontWheelRadius = wheelColliderFrontLeft.radius;

		wheelColliderFrontRight = transform.Find("WheelColliders/FrontRight").GetComponent<WheelCollider>();

		wheelColliderRearLeft = transform.Find("WheelColliders/RearLeft").GetComponent<WheelCollider>();
		myRearWheelRadius = wheelColliderRearLeft.radius;

		wheelColliderRearRight = transform.Find("WheelColliders/RearRight").GetComponent<WheelCollider>();
	}
	
	public float Thrust { get { return thrust; }  set { thrust = Mathf.Clamp(value, -1.0f, 1.0f); } }
	
	public float Steering { get { return steer; } set { steer = Mathf.Clamp(value, -1.0f, 1.0f); }  }
	
	public float MPH { 	get { return currentMPH; } }
	
	public bool IsGrounded { get { return isGrounded; } }
	
	public bool IsOffRoad { get { return isOffTrack; } }
 
	public void Move(float h, float v) { thrust = v; steer = h; }
	
	
	void Update() { 

		if ( Physics.Raycast(frontLeftWheelTransform.position, -frontLeftWheelTransform.up, out hit,  wheelColliderFrontLeft.suspensionDistance + wheelColliderFrontLeft.radius ) ) {
			frontLeftWheelTransform.position = hit.point + (frontLeftWheelTransform.up * wheelColliderFrontLeft.radius); 
		} else { frontLeftWheelTransform.position = transform.Find("WheelColliders/FrontLeft").transform.position; }


		if ( Physics.Raycast(frontRightWheelTransform.position, -frontRightWheelTransform.up, out hit,  wheelColliderFrontRight.suspensionDistance + wheelColliderFrontRight.radius ) ) {
			frontRightWheelTransform.position = hit.point + (frontRightWheelTransform.up * wheelColliderFrontRight.radius); 
		} else { frontRightWheelTransform.position = transform.Find("WheelColliders/FrontRight").transform.position; }

		if ( Physics.Raycast(rearLeftWheelTransform.position, -rearLeftWheelTransform.up, out hit,  wheelColliderRearLeft.suspensionDistance + wheelColliderRearLeft.radius ) ) {
			rearLeftWheelTransform.position = hit.point + (rearLeftWheelTransform.up * wheelColliderRearLeft.radius); 
		} else { rearLeftWheelTransform.position = transform.Find("WheelColliders/RearLeft").transform.position; }
		
		
		if ( Physics.Raycast(rearRightWheelTransform.position, -rearRightWheelTransform.up, out hit,  wheelColliderRearRight.suspensionDistance + wheelColliderRearRight.radius ) ) {
			rearRightWheelTransform.position = hit.point + (rearRightWheelTransform.up * wheelColliderRearRight.radius);  
		} else { rearRightWheelTransform.position = transform.Find("WheelColliders/RearRight").transform.position; }

	}

	void FixedUpdate()
	{ 
		// Check for vehicle on ground with traction (not is a jump).
			myWheelRayCastLocationFront = 0.5f*(frontRightWheelTransform.position + frontLeftWheelTransform.position) - new Vector3(0,0.5f,0.0f) * myFrontWheelRadius;
			myWheelRayCastLocationRear = 0.5f*(rearRightWheelTransform.position + rearLeftWheelTransform.position) - new Vector3(0,0.5f,0.0f) * myRearWheelRadius;
			if ( Physics.Raycast(myWheelRayCastLocationFront, -transform.up, out hit, 1.5f*myFrontWheelRadius) && Physics.Raycast(myWheelRayCastLocationRear, -transform.up, out hit, 1.5f*myRearWheelRadius)) isGrounded = true; else isGrounded = false;
			//Debug.DrawRay(myWheelRayCastLocationFront, -Vector3.up, Color.red);  Debug.DrawRay(myWheelRayCastLocationRear, -Vector3.up, Color.red);

		if(isGrounded){
			//isOffTrack = hit.collider.gameObject.CompareTag("OffTrack");
			ApplyThrust();  // only apply thrust if the wheels are touching the ground
			thrust_myAirMomentum = myCalculatedThrust ;
			} else { 
						myAirMomentum = Mathf.SmoothDamp(myAirMomentum, 0.0f, ref thrust_myAirMomentum, 0.25f);
						physicsBody.velocity += accel * transform.forward * myAirMomentum * (Time.deltaTime) ;
						physicsBody.velocity += 20f * -Vector3.up * (Time.deltaTime) ;
			}

		// Calculate and apply DRAG
		myLocalVelocity = transform.InverseTransformDirection(physicsBody.velocity);
		myDrag = Vector3.zero;
		//if(isOffTrack) {myDrag.z += myLocalVelocity.z * offTrackDragMultiplier; }

		float lateralDrag = Mathf.Lerp(1.0f, 5.0f, traction);
		myDrag.x += myLocalVelocity.x * lateralDrag;
		myDrag = transform.TransformDirection(myDrag);
		physicsBody.velocity -= myDrag * Time.deltaTime;
		myLocalVelocity = transform.InverseTransformDirection(physicsBody.velocity);
		
		currentMPH = myLocalVelocity.z * 2.237f; 

		ApplySteering();
		
		// Show wheels turning, if they should be!
		wheelRotationFront = (myLocalVelocity.z  / myFrontWheelRadius) * Time.deltaTime * Mathf.Rad2Deg;
		//wheelRotationRear = (myLocalVelocity.z  / myRearWheelRadius) * Time.deltaTime * Mathf.Rad2Deg;

		transform.Find("FrontLeftSteering/FrontLeftRotation").transform.Rotate(wheelRotationFront, 0.0f, 0.0f);
		transform.Find("FrontRightSteering/FrontRightRotation").transform.Rotate(wheelRotationFront, 0.0f, 0.0f);
		transform.Find("RearLeftSteering/RearLeftRotation").transform.Rotate(wheelRotationFront, 0.0f, 0.0f);
		transform.Find("RearRightSteering/RearRightRotation").transform.Rotate(wheelRotationFront, 0.0f, 0.0f);

	} // END void FixedUpdate()
	

	
	private void ApplyThrust()
	{	
		accelerationTime = accelTime;
		
		accelerationTime = Mathf.Max(0.01f, accelerationTime);
		float topSpeedReverse = 0.5f * topSpeedMPH;
		accel = topSpeedMPH ;

		if(currentMPH >= topSpeedMPH || currentMPH <= -topSpeedReverse) accel =  0.0f;

		if (thrust < 0.1f) buildingMomentum = false;
		if (thrust > 0.1f) buildingReverseMomentum = false;

		if((thrust < -0.15f) && (currentMPH > .9f * topSpeedReverse)) buildingReverseMomentum = true;

		if (currentMPH < -.9f*topSpeedReverse) {buildingReverseMomentum = false; }

		if((thrust > 0.15f) && (currentMPH < .9f * topSpeedMPH)) buildingMomentum = true;
		if (currentMPH > .9f*topSpeedMPH) {buildingMomentum = false; }

		if (buildingMomentum){
			myCalculatedThrust = Mathf.SmoothDamp(myCalculatedThrust, 1.0f, ref thrust_CurrentVelocity, accelerationTime);
		} else if (buildingReverseMomentum){
			myCalculatedThrust = Mathf.SmoothDamp(myCalculatedThrust, -1.0f, ref thrust_CurrentVelocity, accelerationTime);
		} else { myCalculatedThrust = thrust;  thrust_CurrentVelocity = 0;}

		physicsBody.velocity += accel * 2f * transform.forward * myCalculatedThrust * (Time.deltaTime) ;



	}
	
	private void ApplySteering()
	{
		steerAngle = steer * maximumSteeringAngle;

		var currentAngle = transform.Find("FrontRightSteering").transform.localRotation;
		var newAngle = Quaternion.Euler(0 , steerAngle, 0);
		transform.Find("FrontRightSteering").transform.localRotation= Quaternion.Lerp(currentAngle, newAngle, 3f * Time.deltaTime);
		currentAngle = transform.Find("FrontLeftSteering").transform.localRotation;
		transform.Find("FrontLeftSteering").transform.localRotation= Quaternion.Lerp(currentAngle, newAngle, 3f * Time.deltaTime);

		// Steering only works if the vehicle is on the ground.
		if(isGrounded && physicsBody.velocity.sqrMagnitude > 0.1f)
		{
			// If in reverse, then the steering must be reversed.
			myLocalVelocity = transform.InverseTransformDirection(physicsBody.velocity);
			steerAngle *= Mathf.Sign(myLocalVelocity.z);
			
			// Let the steering actually rotate the vehicle.
			Quaternion steerRot = Quaternion.Euler(0, steerAngle * Time.deltaTime * 2.0f, 0);
			physicsBody.MoveRotation(transform.rotation * steerRot);
			
		}
	}




}
