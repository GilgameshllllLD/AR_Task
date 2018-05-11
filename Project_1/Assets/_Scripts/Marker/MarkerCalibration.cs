using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MarkerCalibration : MonoBehaviour {
	public Transform frontPiece;
	public Transform backPiece;
	public Transform leftPiece;
	public Transform rightPiece;

	public Slider sensititivySlider;
	public Slider frontSliderX;
	public Slider frontSliderZ;
	public Slider leftSliderX;
	public Slider leftSliderZ;

	public Text frontTxt;
	public Text rightTxt;
	public Text toggleTxt;

	private float positionMod = 1;

	private Vector3[] startPositions;

	private Renderer[] markerGraphics;


	// Use this for initialization
	void Start () {
		markerGraphics = gameObject.GetComponentsInChildren<MeshRenderer> ();
		startPositions = new Vector3[4] {
			frontPiece.transform.localPosition,
			backPiece.transform.localPosition,
			leftPiece.transform.localPosition,
			rightPiece.transform.localPosition
		};
		frontSliderX.value = frontSliderZ.value = leftSliderX.value = leftSliderZ.value = 0;
		frontSliderX.onValueChanged.AddListener(delegate {onSliderChange();});
		frontSliderZ.onValueChanged.AddListener(delegate {onSliderChange();});
		leftSliderX.onValueChanged.AddListener(delegate {onSliderChange();});
		leftSliderZ.onValueChanged.AddListener(delegate {onSliderChange();});
		updateData ();
	}

	public void updateData(){
		string data = "Front \n";
		data += "X: " + frontPiece.transform.localPosition.x + "\n";
		data += "Y: " + frontPiece.transform.localPosition.z + "\n";
		data += "Z: " + frontPiece.transform.localPosition.y + "\n";
		data += "\nBack \n";
		data += "X: " + backPiece.transform.localPosition.x + "\n";
		data += "Y: " + backPiece.transform.localPosition.z + "\n";
		data += "Z: " + backPiece.transform.localPosition.y + "\n";
		frontTxt.text = data;

		data = "Right \n";
		data += "X: " + rightPiece.transform.localPosition.x + "\n";
		data += "Y: " + rightPiece.transform.localPosition.z + "\n";
		data += "Z: " + rightPiece.transform.localPosition.y + "\n";
		data += "\nLeft \n";
		data += "X: " + leftPiece.transform.localPosition.x + "\n";
		data += "Y: " + leftPiece.transform.localPosition.z + "\n";
		data += "Z: " + leftPiece.transform.localPosition.y + "\n";
		rightTxt.text = data;


		positionMod = sensititivySlider.value;
		frontPiece.transform.localPosition = startPositions [0] + new Vector3 (frontSliderX.value, 0, frontSliderZ.value) * positionMod;
		backPiece.transform.localPosition = startPositions [1] - new Vector3 (frontSliderX.value, 0, frontSliderZ.value) * positionMod;
		leftPiece.transform.localPosition = startPositions [2] + new Vector3 (leftSliderX.value, 0, leftSliderZ.value) * positionMod;
		rightPiece.transform.localPosition = startPositions [3] - new Vector3 (leftSliderX.value, 0, leftSliderZ.value) * positionMod;
	}

	private void onSliderChange(){
		updateData ();
	}
	bool isOn = true;
	public void toggleMarker(){
		isOn = !isOn;
		foreach (Renderer ren in markerGraphics) {
			ren.enabled = !ren.enabled;
		}
		if (isOn) {
			toggleTxt.text = "Toggle Off";
		} else {
			toggleTxt.text = "Toggle On";
		}
	}
	public void loadGame(){
		Libonati.loadNextScene ();
	}
}
