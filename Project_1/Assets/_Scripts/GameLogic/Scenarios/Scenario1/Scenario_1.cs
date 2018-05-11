public class Scenario_1 : Scenario {
    public ElectricGrid mainGrid;

	ColorBuildings colorBuildings {
		get {
			return game.city.gameObject.GetComponent<ColorBuildings>();
		}
	}

	public override void startScenario (int stepIndex = 0)
    {
		base.startScenario (stepIndex);
		CancelInvoke ("turnOffBuildings");
		colorBuildings.enabled = true;
	}

	public override void stopScenario () {
		base.stopScenario ();
		ColorBuildings.SetWave(ColorBuildings.Waves.Default);
		ColorBuildings.Instance.cleakSkippedCoolDown ();
		//mainGrid.DirectSetCool ();
		mainGrid.Heatwave = false;
		mainGrid.Visible (false);
		mainGrid.startingColor = ElectricGrid.GridColor.COOL;
		Invoke ("turnOffBuildings", 5);
    }
	private void turnOffBuildings(){
		colorBuildings.enabled = false;
	}

    public void InitiateHeatwave() {
		ColorBuildings.SetWave(ColorBuildings.Waves.Heatwave);
    }

    public void InitiateCoolwave() {
		ColorBuildings.SetWave(ColorBuildings.Waves.Coolwave);
    }

}
