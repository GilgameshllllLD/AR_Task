using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BezierSpline))]
public class SplineParticleSpawner : MonoBehaviour {

	public Sprite[] sprites;
	public int numParticles = 20;
	public int duration = 10;
	public float loopDelay = 0.0f;
	public float spawnDelayInSeconds = 1;
	public float burstSpeed = 0.3f;
	public AnimationCurve alphaCurve;

	public bool ui3d = true;
	public UI_3D.UI_3D_Mode ui3dMode = UI_3D.UI_3D_Mode.OrientY;

	int particleI = 0;
	Coroutine coro;

	public GameObject MakeParticle(GameObject prefab, float duration, SplineWalkerMode mode=SplineWalkerMode.Loop, bool paused=false) {
		var gameObject = Instantiate<GameObject> (prefab);
		return _makeParticle (gameObject, duration, mode, paused);
	}

	public GameObject MakeParticle(Sprite sprite, float duration, SplineWalkerMode mode=SplineWalkerMode.Loop) {
		var gameObject = new GameObject ();
		var spriteRenderer = gameObject.AddComponent<SpriteRenderer> ();
		spriteRenderer.sprite = sprite;
		return _makeParticle (gameObject, duration, mode);
	}

	public GameObject _makeParticle(GameObject gameObject, float duration, SplineWalkerMode mode=SplineWalkerMode.Loop, bool paused=false) {
		gameObject.transform.SetParent(transform, false);

		if (ui3d && !gameObject.GetComponent<UI_3D>()) {
			var billboard = gameObject.AddComponent<UI_3D>();
			billboard.mode = ui3dMode;
		}

		var spline = GetComponent<BezierSpline>();

		var splineWalker = gameObject.AddComponent<SplineWalker>();
		splineWalker.duration = duration;
		splineWalker.spline = spline;
		splineWalker.mode = mode;
		splineWalker.Paused = paused;
		if (mode == SplineWalkerMode.Loop)
			splineWalker.loopDelay = loopDelay;
		if (alphaCurve != null && alphaCurve.keys.Length > 0)
			splineWalker.alphaCurve = alphaCurve;
		//splineWalker.startOffset = (float)i / (float)numParticles;

		//gameObject.hideFlags = HideFlags.HideAndDontSave;
		gameObject.layer = LayerMask.NameToLayer("UI");
		gameObject.transform.SetParent(transform, false);

		return gameObject;
	}

	// let any looping particles finish, and then destroy them
	public void DestroyAllAfterLooping() {
		if (coro != null) {
			StopCoroutine (coro);
			coro = null;
		}

		var count = 0;
		foreach (var splineWalker in GetComponentsInChildren<SplineWalker>()) {
			splineWalker.mode = SplineWalkerMode.Once;
			splineWalker.destroyOnFinish = true;
			count += 1;
		}

		Debug.Log ("destroyOnFinish set " + count);
	}

	public void SpawnNewBurst() {
		DestroyAllParticles();
		particleI = 0;
		coro = StartCoroutine(spawnBursts());
	}

	public void DestroyAllParticles() {
		if (coro != null) {
			StopCoroutine(coro);
			coro = null;
		}

		var children = new List<GameObject> ();
		foreach (Transform child in transform)
			children.Add (child.gameObject);
		children.ForEach(child => Destroy(child.gameObject));
	}

	void OnDisable() {
		DestroyAllParticles();
	}

	IEnumerator spawnBursts() {
		yield return new WaitForSeconds(spawnDelayInSeconds);

		Debug.Log("spawning " + numParticles + " from " + gameObject);
		for (var i = 0; i < numParticles; ++i) {
			MakeParticle(nextSprite(), duration);
			particleI++;
			yield return new WaitForSeconds(burstSpeed);
		}
	}

	Sprite nextSprite() { return sprites [Random.Range (0, sprites.Length)]; }

	private void spawn(){
		if (particleI++ < numParticles) {
			MakeParticle (nextSprite(), duration);
		} else {
			CancelInvoke ("spawn");
		}
	}

}
