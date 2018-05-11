using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LibonatiParticleSystem : MonoBehaviour {
	public enum SpawnType
	{
		SPHERE,
		TRIANGLE
	}
	private List<LibonatiParticle> particles;
	//private float lastTickTime = 0;
	private float timeSinceSpawn = 0;
	private Transform particleHolder;


	//SPAWN
	public LibonatiParticle particleTemplate;
	public SpawnType spawnType;
	public float spawnRateInSeconds = 5;
	public float lifetimeInSeconds = 10;

	//SIZE
	public float startSize = 1;
	public float startSizeRandomRange = 0;
	
	//COLOR
	public Gradient[] startColor = new Gradient[1];

	//MOVEMENT
	public float startSpeed = 0;

	// Use this for initialization
	void Awake () {
		particles = new List<LibonatiParticle> ();
		particleHolder = new GameObject (name + ": ParticleHolder").transform;
	}
	
	// Update is called once per frame
	void Update () {
		timeSinceSpawn += Time.deltaTime;
		if (timeSinceSpawn >= 1 / spawnRateInSeconds) {
			timeSinceSpawn = 0;
			spawnParticle();
		}
		//lastTickTime = Time.time;
	}

	private void OnEnable(){
		timeSinceSpawn = 0;
		//lastTickTime = Time.time;
	}
	private void OnDisable(){

	}

	public void spawnParticle(){
		LibonatiParticle particle = (LibonatiParticle)Instantiate (particleTemplate, getParticleSpawnPoint(), Quaternion.identity);
		particle.size = startSize + Random.Range (-startSizeRandomRange, startSizeRandomRange);
		particle.color = startColor[Random.Range(0,startColor.Length)];
		particle.velocity = getParticleVelocity ();
		particle.lifeTimeInSeconds = lifetimeInSeconds;
		particle.system = (LibonatiParticleSystem)this;
		particle.transform.parent = particleHolder.transform;
		particles.Add (particle);
	}
	public void particleDeath(LibonatiParticle particle){
		particles.Remove(particle);
	}
	private Vector3 getParticleSpawnPoint(){
		return transform.position;
	}
	private Vector3 getParticleVelocity(){
		Vector3 dir = new Vector3(0,0,0);
		switch (spawnType) {
		case SpawnType.SPHERE:
			dir = new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f)).normalized;
			break;
		case SpawnType.TRIANGLE:
			dir = transform.TransformDirection(new Vector3(Random.Range(-1f,1f),Random.Range(.5f,1f),0)).normalized;
			break;
		}
		return dir * startSpeed;
	}
	
}
