using UnityEngine;
using System.Collections;
using SimpleJSON;

public enum AdminMessage
{
	CURRENT_STATE
}

public class AdminManager : MonoBehaviour {

#if UNITY_EDITOR
	public void Update() {
		// Arrows and numbers WITH shift change the scenario on the network
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			if (Input.GetKeyDown(KeyCode.Alpha0)) { setState(-1, -1); }
			if (Input.GetKeyDown(KeyCode.Alpha1)) { setState(0, 0); }
			if (Input.GetKeyDown(KeyCode.Alpha2)) { setState(1, 0); }
			if (Input.GetKeyDown(KeyCode.Alpha3)) { setState(2, 0); }
			if (Input.GetKeyDown(KeyCode.Alpha4)) { setState(3, 0); }

			if (Input.GetKeyDown(KeyCode.LeftArrow))
				step(-1);
			else if (Input.GetKeyDown(KeyCode.RightArrow))
				step(1);
		}
	}
#endif

	public void step(int delta=1) {
		// Advance to the next step
		int scenarioIndex, stepIndex;
		GameLogic.Instance.getNextStepDelta(delta, out scenarioIndex, out stepIndex);
		setState(scenarioIndex, stepIndex);
	}

	public void attract() { setState(GameLogic.ATTRACT_MODE, -1); }
	public void begin() { setState(0, 0); }

	private void setState(int scenario, int step) {
		var json = JSONNode.Parse ("{}");
		json ["type"] = AdminMessage.CURRENT_STATE.ToString();
		json ["scenario"] = scenario.ToString();
		json ["step"] = step.ToString();
		//Debug.Log ("sending " + json.ToString());
		GameLogic.Instance.sendMessage(json.ToString(), retain: true);
	}

}
