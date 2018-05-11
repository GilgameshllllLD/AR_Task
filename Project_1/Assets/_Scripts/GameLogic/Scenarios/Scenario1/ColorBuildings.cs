using UnityEngine;
using System;
using System.Collections.Generic;


public class ColorBuildings : MonoBehaviour
{
    public enum Waves
    {
        Default,
        Beginwave,
        Heatwave,
        Coolwave,
		None
    }

	public static ColorBuildings Instance;

    public CityManager HeatMapTemplate; // The source to pull the color from.

    // Heat colors.
    public Material[] CoolMaterials; // Only 7 of these.

	public static Waves Wave;
	private Waves lastWave = Waves.None;

    private const float transition_speed = 7.0f;
    private const float fade_off_speed = 3.0f;

    private float heat_lerp = 0.0f;
    private float cool_lerp = 0.0f;

    private Color[] original_colors; // Store the original colors so we can get them back.
    private Color[] heat_colors; // Store the colors from the cityHeatMapTemplate.
    private Color[] cool_colors;

    // Sidewalk texture swaping.
    public GameObject CityBlocks;
    //private Material original_block_material;
    public Material HeatBlockMaterial;
    public Material CoolBlockMaterial;
    //private bool swap_material = false;

    // Grab all nested children.
    private Transform[] allChildren;
	private MeshRenderer[] allMesh;

	int GetMeshRendererIndex(MeshRenderer meshRenderer) {
		for (var i = 0; i < allMesh.Length; ++i)
			if (allMesh[i] == meshRenderer)
				return i;
		return -1;
	}

	Color GetCoolColor(MeshRenderer meshRenderer) {
		var index = GetMeshRendererIndex(meshRenderer);
		if (index >= 0 && index < cool_colors.Length)
			return cool_colors[index];
		else
			return CoolMaterials[0].color;
	}
	private MeshRenderer sidewalkMesh;

	Color hotColor;
	Color origColor;

	public static void SetWave(ColorBuildings.Waves wave) {
		if (ColorBuildings.Wave != ColorBuildings.Instance.lastWave)
			ColorBuildings.Instance.lastWave = ColorBuildings.Waves.None;
		ColorBuildings.Wave = wave;
	}

	public void ResetState() {
		ColorBuildings.SetWave(ColorBuildings.Waves.Default);
	}

    void Awake()
    {
		Instance = this;
		origColor = new Color(167.0f/255.0f, 163.0f/255.0f, 160/255.0f, 1.0f);

        // Grab all nested children.
        allChildren = GetComponentsInChildren<Transform>();
		allMesh = new MeshRenderer[allChildren.Length];
        Transform[] heat_children = HeatMapTemplate.GetComponentsInChildren<Transform>();

        // Resize the color arrays.
        Array.Resize<Color>(ref original_colors, allChildren.Length);
        Array.Resize<Color>(ref heat_colors, allChildren.Length);
        Array.Resize<Color>(ref cool_colors, allChildren.Length);

		//Debug.Log("allChildren length: " + allChildren.Length);


        for (int i = 0; i < allChildren.Length; i++)
        {
            MeshRenderer renderer = allChildren[i].GetComponent<MeshRenderer>();
			allMesh [i] = renderer;
            if (renderer != null)
            {
				if (renderer.gameObject.name == "cityBlocks") {
					sidewalkMesh = renderer;
				}
                Material material = renderer.material;
                if (material != null)
                    original_colors[i] = material.color;
				if (i > heat_children.Length) {
					return;
				}
                material = heat_children[i].GetComponent<MeshRenderer>().sharedMaterial;
                if (material != null)
                {
                    heat_colors[i] = material.color;

                    switch (material.name)
                    {
                        case "Egrid_hot_1":
                            cool_colors[i] = CoolMaterials[0].color;
							hotColor = heat_colors[i];
                            break;
                        case "Egrid_hot_2":
                            cool_colors[i] = CoolMaterials[1].color;
                            break;
                        case "Egrid_hot_3":
                            cool_colors[i] = CoolMaterials[2].color;
                            break;
                        case "Egrid_hot_4":
                            cool_colors[i] = CoolMaterials[3].color;
                            break;
                        case "Egrid_hot_5":
                            cool_colors[i] = CoolMaterials[4].color;
                            break;
                        case "Egrid_hot_6":
                            cool_colors[i] = CoolMaterials[5].color;
                            break;
                        case "Egrid_hot_7":
                            cool_colors[i] = CoolMaterials[6].color;
                            break;

                        default:
                            cool_colors[i] = original_colors[i];
                            break;
                    }
                }

                //Debug.Log(allChildren[i].name + " | " + heat_children[i].name);
            }
        }

        //original_block_material = CityBlocks.GetComponent<MeshRenderer>().material;
    }

	//public string status; // for debugging

    void Update()
    {
		if (lastWave != Wave) {
			//status = "Updating " + Wave;
			updateWaves ();
		} else {
			//status = "Done " + Wave;
		}
    }
    
	public void CoolDownObject(GameObject obj) {
		var mr = obj.GetComponent<MeshRenderer>();
		if (mr)
			CoolDownRenderer(mr);

		foreach (var r in obj.GetComponentsInChildren<MeshRenderer>())
			CoolDownRenderer(r);
	}

	readonly Dictionary<MeshRenderer, bool> skipRenderers = new Dictionary<MeshRenderer, bool>();

	public void cleakSkippedCoolDown(){
		skipRenderers.Clear ();
	}
	public void SkipCoolDown(GameObject obj) {
		var mr = obj.GetComponent<MeshRenderer>();
		if (mr) SkipRenderer(mr);

		foreach (var r in obj.GetComponentsInChildren<MeshRenderer>())
			SkipRenderer(r);
	}

	void SkipRenderer(MeshRenderer meshRenderer) {
		skipRenderers[meshRenderer] = true;
	}

	void CoolDownRenderer(MeshRenderer meshRenderer) {
		SkipRenderer(meshRenderer);

		const float time = 5.0f;
		var coolColor = GetCoolColor(meshRenderer);

		LeanTween.color(meshRenderer.gameObject, coolColor, time);
	}

	private void updateWaves()
    {
        // Update the lerp values.
        if (Wave == Waves.Beginwave) // Default to Coolwave.
        {
            if (cool_lerp < 1.0)
            {
                cool_lerp += Time.deltaTime / transition_speed;
            }
            else
                cool_lerp = 1;
        }
        else if (Wave == Waves.Heatwave) // Coolwave to Heatwave.
        {
			if (heat_lerp < 1.0)
			{
				heat_lerp += Time.deltaTime / transition_speed;
			}
			else
				heat_lerp = 1;
		}
		else if (Wave == Waves.Coolwave) // Heatwave to Coolwave.
		{
            if (cool_lerp < 1.0)
            {
                cool_lerp += Time.deltaTime / transition_speed;
            }
            else
                cool_lerp = 1;
		}
		else if (Wave == Waves.Default) // Coolwave to Defalult.
		{
			var falloff = Time.deltaTime / fade_off_speed;

			if (heat_lerp > 0.0f) {
				heat_lerp -= falloff;
				if (heat_lerp < 0.0f) heat_lerp = 0.0f;
			}

			if (cool_lerp > 0.0f) {
				cool_lerp -= falloff;
				if (cool_lerp < 0.0f) cool_lerp = 0.0f;
			}
        }

		if (Wave == Waves.Heatwave || Wave == Waves.Beginwave || Wave == Waves.Default) {
			foreach (var heatsUp in FindObjectsOfType<HeatUp>()) {
				var renderer = heatsUp.GetComponent<MeshRenderer>();
				var coolColor = CoolMaterials[0].color;
				if (Wave == Waves.Beginwave)
					renderer.material.color = Color.Lerp(origColor, coolColor, cool_lerp);
				else if (Wave == Waves.Heatwave)
					renderer.material.color = Color.Lerp(coolColor, hotColor, heat_lerp);
				else if (Wave == Waves.Default)
					renderer.material.color = Color.Lerp(origColor, coolColor, heat_lerp);
			}
		}

		// Color all the buildings.
		for (int i = 0; i < allMesh.Length; i++)
		{
			MeshRenderer renderer = allMesh [i];

			if (renderer == null || (Wave != Waves.Default && skipRenderers.ContainsKey(renderer)))
				continue;

			if (Wave == Waves.Beginwave)
			{
				renderer.material.color = Color.Lerp(original_colors[i], cool_colors[i], cool_lerp);
				if (renderer != sidewalkMesh) {
					renderer.material.SetFloat ("_TextureBlend", 1 - cool_lerp);
				}
				if (cool_lerp >= 1)
				{
					lastWave = Wave;
				}
			}
			else if (Wave == Waves.Heatwave)
			{
				renderer.material.color = Color.Lerp(cool_colors[i], heat_colors[i], heat_lerp);
				//renderer.material.SetFloat("_TextureBlend", 1 - heat_lerp);

				if (heat_lerp >= 1) {
					lastWave = Wave;
					cool_lerp = 0; // Reset for the next coolwave.
				}
			}
			else if (Wave == Waves.Coolwave)
			{
				renderer.material.color = Color.Lerp(heat_colors[i], cool_colors[i], cool_lerp);

				if (cool_lerp >= 1) {
					lastWave = Wave;
				}
			}
			else
			{
				renderer.material.color = Color.Lerp(original_colors[i], cool_colors[i], heat_lerp);
				if (renderer != sidewalkMesh) {
					renderer.material.SetFloat ("_TextureBlend", 1 - heat_lerp);
				}
				if (heat_lerp == 0.0f && cool_lerp == 0.0f)
					lastWave = Wave;
			}
		}
	}
}
