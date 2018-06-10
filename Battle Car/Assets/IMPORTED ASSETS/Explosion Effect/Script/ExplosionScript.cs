using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {

	[Space(20)]
	[Header("Audio sources")]

	//Audiosources
	public AudioSource explosionAudio;
	public AudioSource shockwaveAudio;
	
	[Header("Point light gameobject")]
	//Light gameobject
	public Light lightObject;
	
	[Header("Light options")]

	//Light values
	public float lightIntensity = 8f;
	public float lightRange = 100f;
	public float lightFlashDuration = 0.035f;
	public float shockwaveDelay = 0.55f;
	
	[Header("Explosionforce options")]

	//Explosionforce values
	public float radius = 5.0F;
	public float power = 10.0F;

	[Header("All particle systems")]

	//All particle systems
	public ParticleSystem debrisParticles;
	public ParticleSystem middleSmokeParticles2;
	public ParticleSystem shockwaveParticles;
	public ParticleSystem shockwaveAirParticles;
	public ParticleSystem middleSmokeParticles;
	public ParticleSystem fireParticles;


	void Start () {

		//Make sure light is not showing at start
		lightObject.intensity = 0;
		lightObject.range = lightRange;
	}
	
	//Plays the shockwave audio after a set amount of time
	IEnumerator ShockWaveDelay () {
		
		//How long after the main explosion the audio should be played
		yield return new WaitForSeconds (shockwaveDelay);

		//Play audio
		shockwaveAudio.Play ();
	}
	
	//Plays all particle systems and starts audio and lightflash
	IEnumerator ExplosionDuration () {

		//Light and audio
		lightObject.intensity = lightIntensity;
		explosionAudio.Play ();

		//Particle systems
		debrisParticles.GetComponent<ParticleSystem> ().Play ();
		middleSmokeParticles2.GetComponent<ParticleSystem> ().Play ();
		shockwaveAirParticles.GetComponent<ParticleSystem> ().Play ();
		shockwaveParticles.GetComponent<ParticleSystem> ().Play ();
		middleSmokeParticles.GetComponent<ParticleSystem> ().Play ();
		fireParticles.GetComponent<ParticleSystem> ().Play ();
			
		//How long the lightflash will be visible
		yield return new WaitForSeconds (lightFlashDuration);
		
		lightObject.intensity = 0;
		StartCoroutine (ShockWaveDelay ());
	}

	void Update () {
		
		//Press left click to start the explosion
		if (Input.GetMouseButtonDown (0)) {
			StartCoroutine (ExplosionDuration ());

			//Applies explosion force to nearby rigidbodies on left click
			Vector3 explosionPos = transform.position;
			Collider[] colliders = Physics.OverlapSphere (explosionPos, radius);
			foreach (Collider hit in colliders) {
				Rigidbody rb = hit.GetComponent<Rigidbody> ();
			
			if (rb != null)
				rb.AddExplosionForce (power, explosionPos, radius, 3.0F);
		}}
	}
}