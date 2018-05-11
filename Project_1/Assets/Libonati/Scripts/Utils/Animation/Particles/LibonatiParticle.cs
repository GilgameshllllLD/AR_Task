using UnityEngine;
using System.Collections;

public class LibonatiParticle : MonoBehaviour {
	private float _size;
	private Gradient _color;
	private Vector3 _vel;
	private SpriteRenderer ren;

	private Vector3 newPos;
	private float timeAlive = 0;

	[HideInInspector]
	public LibonatiParticleSystem system;

	public float lifeTimeInSeconds = 10;

	// Use this for initialization
	void Awake () {
		ren = GetComponent<SpriteRenderer> ();
		transform.LookAt (Camera.main.transform.position, Vector3.up);
	}
	
	// Update is called once per frame
	void Update () {
		newPos = transform.position;
		newPos += _vel * Time.deltaTime;
		transform.position = newPos;

		timeAlive += Time.deltaTime;
		setColor(timeAlive / lifeTimeInSeconds);

		if (timeAlive >= lifeTimeInSeconds) {
			die();
		}
	}

	public float size {
		get {
			return _size;
		}
		set {
			_size = value;
			transform.localScale = Vector3.one * value;
		}
	}
	public Gradient color{
		get{
			return _color;
		}
		set{
			_color = value;
			setColor(0);
		}
	}
	public Vector3 velocity{
		get{
			return _vel;
		}
		set{
			_vel = value;
		}
	}

	private void die(){
		system.particleDeath ((LibonatiParticle)this);
		Destroy (gameObject);
	}

	private void setColor(float d){
		if (ren != null) {
			ren.color = _color.Evaluate (d);
		}
	}
}
