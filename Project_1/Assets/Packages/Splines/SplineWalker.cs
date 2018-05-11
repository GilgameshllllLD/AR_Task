using UnityEngine;

public class SplineWalker : MonoBehaviour {

	public BezierSpline spline;

	public float duration;

	public bool lookForward;
	public Vector3 rotationOffset = Vector3.zero;
	public bool destroyOnFinish = false;
	public float loopDelay = 0.0f;
	public AnimationCurve alphaCurve;

	public SplineWalkerMode mode;
	public bool Paused;

	bool HasAnimCurve { get { return alphaCurve != null && alphaCurve.length > 0; } }

	[Range(0,1)]
	public float startOffset = 0;

	private float progress;
	private bool goingForward = true;

	SpriteRenderer spriteRenderer;
	public float delaying = 0.0f;
	private float currentDelay = 0;
	Color originalColor;

    public bool AutoStepNext = false;

	private void Start(){
		progress = startOffset;

		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer)
			originalColor = spriteRenderer.color;
		if (HasAnimCurve)
			SetAlpha(alphaCurve.Evaluate(0));

		currentDelay = delaying;
		UpdateFromProgress();
	}
	public void restart(){
		currentDelay = delaying;
		progress = startOffset;
		transform.position = spline.GetPoint(progress);
		Paused = false;
	}
	public void goToTime(float seconds){
		progress = seconds;
	}

	void SetAlpha(float alpha) {
		Color c = originalColor;
		c.a = Mathf.Clamp01(alpha);
		if (spriteRenderer)
			spriteRenderer.color = c;
	}

	private void Update () {
		if (Paused)
			return;

		if (currentDelay > 0) {
			SetAlpha(0);
			currentDelay -= Time.deltaTime;
			if (currentDelay <= 0) {
				currentDelay = 0;
				SetAlpha(HasAnimCurve ? alphaCurve.Evaluate(0) : 1.0f);
			}
			return;
		}

		bool hideThisFrame = false;
		if (goingForward) {
			progress += Time.deltaTime / duration;
			if (progress > 1f) {
				switch (mode) {
				case SplineWalkerMode.Once:
					progress = 1f;
					break;
				case SplineWalkerMode.Loop:
					progress -= 1f;
					currentDelay = loopDelay;
					hideThisFrame = true;
					break;
				case SplineWalkerMode.PingPong:
					progress = 2f - progress;
					goingForward = false;
					break;
				default:
					Debug.LogWarning ("unknown SplineWalkerMode " + mode);
					break;
				}

				if (destroyOnFinish)
					Destroy(gameObject);

                // Use in Demo 5 to Step from the Plane Taxi to Walkin Through the Terminal.
                if (AutoStepNext)
                {
                    Debug.Log("Auto Next Step.");
                    GameLogic.Instance.stepScenario();
                }

			}
		}
		else {
			progress -= Time.deltaTime / duration;
			if (progress < 0f) {
				progress = -progress;
				goingForward = true;
			}
		}

		UpdateFromProgress(hideThisFrame);
	}

	void UpdateFromProgress(bool hideThisFrame = false) {

		Vector3 position = spline.GetPoint(progress);
		transform.position = position;
		if (lookForward) {
			if (goingForward) {
				transform.LookAt (position + spline.GetDirection (progress));
			} else {
				transform.LookAt (position - spline.GetDirection (progress));
			}
			transform.Rotate (rotationOffset);
		}

		else if (hideThisFrame)
			SetAlpha(0);
		else if (HasAnimCurve)
			SetAlpha(alphaCurve.Evaluate(progress));
		else
			SetAlpha(1.0f);
	}
}
