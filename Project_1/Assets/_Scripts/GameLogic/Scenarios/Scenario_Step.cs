using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ZoomType {
	WholeCity,
	Area,
	FollowObjectNamed
}

public class Scenario_Step : MonoBehaviour {

	public ZoomType zoomType;
	public string zoomToArea;
	public float zoomDelay = 0;
	public string followObjectName;
	public Transform[] unparentObjects;
	public float enableNextAfterSeconds = 5.0f;

	public enum TimeOfDay {
		Unset,
		Day,
		Night
	}

	public TimeOfDay timeOfDay = TimeOfDay.Unset;
	public float timeOfDayDelay = 0.0f;

	public LayerMask layerNotToMask;

	[HideInInspector]
	public Scenario scenario;

	public virtual string friendlyName {
		get {
			return name;
		}
	}
	protected virtual void Awake(){
	}
	protected virtual void OnDestroy(){
	}
	public virtual void leaveStep() {
		foreach (Transform t in unparentObjects) {
            if (t != null)
                t.SetParent (transform, false);
		}
	}
	public virtual void enterStep() {
		foreach (Transform t in unparentObjects) {
            if (t!=null)
			t.SetParent (null, false);
		}
	}
	public virtual void leaveScenario() {

	}
	public virtual void enterScenario() {

	}
	public override string ToString ()
	{
		return friendlyName;
	}

	public void AdvanceLocallyToNextStep() {
		GameLogic.Instance.stepScenario ();
	}
}

#if false && UNITY_EDITOR
[CustomEditor(typeof(Scenario_Step))]
public class Scenario_StepEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        var step = (Scenario_Step)target;
		step.zoomType = (ZoomType)EditorGUILayout.EnumPopup("Zoom Type", step.zoomType);
		switch (step.zoomType) {
		case ZoomType.Area:
			var zoomTo = FindObjectOfType<ZoomToScenario>();
			if (zoomTo && zoomTo.transform.childCount > 0) {
				var names = zoomTo.AreaNames;
				var index = names.IndexOf(step.zoomToArea);
				if (index == -1) index = 0;
				int newIndex = EditorGUILayout.Popup("Zoom To Area", index, names.ToArray(), new GUILayoutOption[] {});
				step.zoomToArea = names[newIndex];
			} else {
				step.zoomToArea = EditorGUILayout.TextField("Zoom To Area", step.zoomToArea);
			}
			break;
		case ZoomType.FollowObjectNamed:
			step.followObjectName = EditorGUILayout.TextField("Follow Object Named", step.followObjectName);
			break;
		}
		step.timeOfDay = (Scenario_Step.TimeOfDay)EditorGUILayout.EnumPopup("Time of Day", step.timeOfDay, new GUILayoutOption[] {});
		if (step.timeOfDay != Scenario_Step.TimeOfDay.Unset)
			step.timeOfDayDelay = EditorGUILayout.FloatField("Time of Day Delay", step.timeOfDayDelay);
    }
}
#endif
