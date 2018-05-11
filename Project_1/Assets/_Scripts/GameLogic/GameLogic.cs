using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

public class GameLogic : Singleton<GameLogic> {
	public Camera arCamera;

	public delegate void OnStateDelegate(Scenario scenario);
	public OnStateDelegate OnState;

	//public CityManager cityTemplate;
	[HideInInspector]
	public CityManager city;
	public Network_MQTT networkTemplate;
	private Network_MQTT network;
	private GameObject ambientAnimation; // TODO: link publically

	private AR_Manager AR;

	public Scenario[] scenarioTemplates;
	private Scenario[] scenarios;

	[HideInInspector]
	public Scenario currentScenario;

	public StatueController statues;

	public int CurrentScenarioIndex {
		get {
			if (currentScenario != null) {
				return Array.IndexOf (scenarios, currentScenario);
			} else {
				return -1;
			}
		}
	}

	private string mqttTopic;
	private string mqttServerIP;
	private int mqttPort;

	public UnityEngine.UI.Text statusTxt;

	public bool adminEnabledInEditor = true; // If true, admin will always be on in the editor

	public const int ATTRACT_MODE = -1;

	public bool connectToServer = true;
    public int startAtScenario = 0;

	[HideInInspector]
	public LightingController lighting;


	void Awake() {
		try{
			AR = gameObject.GetComponent<AR_Manager> ();
			city = GameObject.FindObjectOfType<CityManager> ();
			lighting = FindObjectOfType<LightingController> ();

			var scenarioHolder = new GameObject ("Scenarios").transform;
			scenarioHolder.SetParent (city.transform, false);
			scenarios = new Scenario[scenarioTemplates.Length];
			int i = 0;
			foreach (Scenario sT in scenarioTemplates) {
				Scenario s = (Scenario)Instantiate (sT);
				s.gameObject.SetActive(false);
				s.name = sT.name;
				s.transform.SetParent(scenarioHolder,false);
				scenarios [i] = s;
				//Debug.Log(s.name + " in slot " + i  + " has " + s.stepTemplates.Length + " steps");
				i++;
			}
		}catch(Exception e){
			Debug.LogError("Couldn't Initialize GameLogic: " + e.Message);
		}
	}

	void Start () {
		if (connectToServer) {
			startNetworkConnection ();
		}

		foreach (Scenario scenario in scenarios)
			scenario.game = (GameLogic)this;

		ambientAnimation = GameObject.Find("AmbientAnimation");
	}
	private void startNetworkConnection(){
		try{
			mqttServerIP = PlayerPrefs.GetString ("mqtt_ip", "10.151.100.200");
			#if UNITY_EDITOR
			mqttServerIP = "104.236.171.74";
			#endif
			mqttTopic = PlayerPrefs.GetString ("mqtt_topic", "verizon_ar");
			mqttPort = int.Parse(PlayerPrefs.GetString ("mqtt_port", "1883"));

			Debug.Log("MQTT Topic: " + mqttTopic);
			Debug.Log("Server IP: " + mqttServerIP);

			if (network != null) {
				Destroy (network.gameObject);
			}
			network = (Network_MQTT)Instantiate (networkTemplate);
			network.transform.parent = transform;
			network.ipAddress = mqttServerIP;
			network.port = mqttPort;
			network.onRecieveMessage = onMessageRecieved;
			network.onConnectionChange = onNetworkChange ;
			network.connect ();
			network.subscribe (new string[1]{mqttTopic});

			sendMessage ("User Connected. IsAdmin: " + IsAdmin, "/debug");
		}catch(Exception e){
			Debug.LogError ("Network Connection Error: " + e.Message);
		}
	}
	private void onNetworkChange(bool connected){
		if (!connected) {
			startNetworkConnection ();
		}
	}

	// a public way to manually trigger the appearance of the 
	// admin's "next step" button. you can also just set your
	// Step's "enableNextAfterSeconds" value to something other
	// than -1
	public static void EnableAdminNextStepButton() {
		if (GameLogic.Instance.IsAdmin)
			FindObjectOfType<AdminUI>().EnableNextButton();
	}

	public bool IsAdmin { get { 
		return Application.isEditor ? adminEnabledInEditor : PlayerPrefs.GetInt("admin_enabled", 0) == 1;
	} }
	
	public void sendMessage(string message, string additionalTopic = "", bool retain=false){
		if (connectToServer) {
			network.sendMessage (mqttTopic + additionalTopic, message, retain: retain);
		}
	}

	public void onMessageRecieved(MQTT_Message MQTTMessage){
		string dataStr = MQTTMessage.message;
		JSONNode json;
		try {
			json = JSONNode.Parse(dataStr);
		} catch {
			Debug.Log("Couldn't decode JSON from " + dataStr);
			return;
		}

		var type = (AdminMessage)Enum.Parse(typeof(AdminMessage), json["type"].Value);
		switch (type) {
		case AdminMessage.CURRENT_STATE:
			var scenarioIndex = json["scenario"].AsInt;
			var stepIndex = json["step"].AsInt;
			//Debug.Log("received CURRENT_STATE (Scenario " + scenarioIndex + ", Step " + stepIndex + ")");
			setState(scenarioIndex, stepIndex);
			break;
		default:
			//Debug.Log("unknown AdminMessage: " + json);
			break;
		}
	}

	public void Update() {
		// Arrows and numbers WITHOUT shift change the scenario locally
		if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) {
			if (Input.GetKeyDown(KeyCode.LeftArrow))
				stepScenario(-1);
			else if (Input.GetKeyDown(KeyCode.RightArrow))
				stepScenario(1);

			if (Input.GetKeyDown(KeyCode.Alpha0)) { setState(-1, -1); }
			if (Input.GetKeyDown(KeyCode.Alpha1)) { setState(0, 0); }
			if (Input.GetKeyDown(KeyCode.Alpha2)) { setState(1, 0); }
			if (Input.GetKeyDown(KeyCode.Alpha3)) { setState(2, 0); }
			if (Input.GetKeyDown(KeyCode.Alpha4)) { setState(3, 0); }
			if (Input.GetKeyDown(KeyCode.Alpha5)) { setState(4, 0); }
		}
	}

	public void spawnCity() {
		if (city != null)
			Destroy (city.gameObject);
		city.transform.position = AR.getCitySpawnPoint ();
	}

	public void enterAttract() {
		setState(ATTRACT_MODE);
	}
	public void onEnterAttract(){

	}
    public void beginState(int scenarioIndex)
    {
        setState(scenarioIndex, 0);
    }
	public void setState(int scenarioIndex, int stepIndex=0) {
		if (scenarioIndex == CurrentScenarioIndex && currentScenario != null && stepIndex == currentScenario.currentStepIndex) {
			//Debug.Log ("Already on step");
			return;
		}
		// Invalid request; reset to attract mode
		if (scenarioIndex >= scenarios.Length) {
			scenarioIndex = ATTRACT_MODE;
		}

		if (scenarioIndex == ATTRACT_MODE) {
			if (statues != null) {
				statues.hideStatue ();
			}
			if (currentScenario) {
				stopScenario ();
				currentScenario = null;
			}
			scenarioIndex = 0;
			stepIndex = -1;
		} else {
			if (CurrentScenarioIndex != scenarioIndex || currentScenario.currentStepIndex != stepIndex) {
				if (statues != null) {
					statues.showStatue (scenarioIndex);
				}
				var scenario = scenarios[scenarioIndex];
				if (scenario != currentScenario) {
					if (currentScenario) {
						stopScenario ();
					}
					currentScenario = scenarios[scenarioIndex];
					currentScenario.gameObject.SetActive(true);
					currentScenario.startScenario(stepIndex);
				} else {
					currentScenario.setStepIndex(stepIndex);
				}
			}
		}

		statusTxt.text = currentScenario ? currentScenario.ToString() : "ATTRACT MODE";

		if (OnState != null)
			OnState(currentScenario);

		// Ambient animation only active when are in attract mode.
		if (ambientAnimation)
			ambientAnimation.SetActive(currentScenario == null || currentScenario.usesAmbientAnimation);
	}
	private void stopScenario(){
		currentScenario.stopScenario();
		currentScenario.gameObject.SetActive(false);
	}

	public static void GoToNextStepLocally() { GameLogic.Instance.stepScenario(); }

	public void stepScenario(int delta = 1) {
		// manually step into the next scenario (on the client)
		int scenarioIndex;
		int stepIndex;
		getNextStepDelta(delta, out scenarioIndex, out stepIndex);
		setState(scenarioIndex, stepIndex);
	}

	public void getPreviousStep(out int scenarioIndex, out int stepIndex) {
		getNextStepDelta(-1, out scenarioIndex, out stepIndex);
	}

	public void getNextStep(out int scenarioIndex, out int stepIndex) {
		getNextStepDelta(1, out scenarioIndex, out stepIndex);
	}

	public void getNextStepDelta(int delta, out int scenarioIndex, out int stepIndex) {
		if (currentScenario == null) {
			if (delta > 0) {
				scenarioIndex = startAtScenario;
				stepIndex = 0;
			} else {
				scenarioIndex = ATTRACT_MODE;
				stepIndex = -1;
			}

			return;
		}
			
		stepIndex = currentScenario.currentStepIndex + delta;

		if (stepIndex >= currentScenario.steps.Length) {
			scenarioIndex = CurrentScenarioIndex;
			stepIndex = currentScenario.steps.Length;
		} else if (stepIndex < 0) {
			scenarioIndex = CurrentScenarioIndex - 1;
			if (scenarioIndex >= 0)
				stepIndex = scenarios[scenarioIndex].stepTemplates.Length - 1;
			else
				stepIndex = -1;
		} else {
			scenarioIndex = CurrentScenarioIndex;
		}
	}
}
