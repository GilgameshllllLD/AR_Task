using UnityEngine;
using System.Collections;

public class ElectricGrid : MonoBehaviour
{
    [HideInInspector]
    public bool Heatwave = false;

    private Color HotColor;
    private Color WarmColor;
    private Color CoolColor;

	private bool visible = false;
    private Animator animator;
    private Material material;

    // Color & Speed.
    private float lerp_value = 0.0f;
    private float lerp_speed = 2.5f; // Seconds.
    private const float electrical_speed = 3.0f;
    private bool lerp_to_hot = false;

    // Visibility.
    //private float alpha_value = 0.0f;
    private float alpha_lerp_value = 0.0f;
    private float alpha_speed = 3.0f;
    private Color rgb;

	public enum GridColor{
		NONE,
		COOL,
		HEAT
	} 
	public GridColor startingColor = GridColor.NONE;

    void Awake()
    {
        animator = GetComponent<Animator>();
        material = GetComponent<MeshRenderer>().material;

        HotColor = new Color32(247, 177, 151, 255);
        WarmColor = new Color32(247, 211, 105, 255);
        CoolColor = new Color32(167, 240, 255, 255);
		Color color = new Color32(167, 240, 255, 0); //new Color(1, 1, 1, alpha_value); // Start transparent.
		switch (startingColor) {
			case GridColor.NONE:
				break;
			case GridColor.COOL:
				color = CoolColor;
				break;
			case GridColor.HEAT:
				color = WarmColor;
				break;
		}
		setColor(color);
    }

    public void Visible(bool is_visible)
    {
        visible = is_visible;
        rgb = material.color; // Store the color data and just ramp the alpha.
    }

	private void setColor(Color newColor){
		newColor.a = Mathf.Max(0,Mathf.Min(1,(float)newColor.a));
		//Debug.Log ("Alpha: " + newColor.a);
		material.SetColor ("_Color", newColor);
	}

    public void DirectSetHot()
    {
        setColor(WarmColor);
    }

    public void DirectSetCool()
    {
        setColor(CoolColor);
    }

    private void Update()
    {
        // Animate the speed of the curent traveling faster or slower.
        if (Heatwave)
        {
            // Ramp up the Color & Speed.
            if (lerp_value < 1)
            {
                lerp_value += Time.deltaTime / lerp_speed;

                if (!lerp_to_hot)
                {
					setColor(Color.Lerp(CoolColor, WarmColor, lerp_value));
                }
                else
                {
                    animator.speed = Mathf.Lerp(1.0f, electrical_speed, lerp_value);
					setColor(Color.Lerp(WarmColor, HotColor, lerp_value));
                }
            }
            else if (!lerp_to_hot)
            {
                lerp_to_hot = true;
                lerp_value = 0;
            }
            else
                lerp_value = 1;
        }
        else
        {


            // Ramp the Color & Speed.
            if (lerp_value > 0)
            {
                lerp_value -= Time.deltaTime / lerp_speed;

                if (lerp_to_hot)
                {
                    animator.speed = Mathf.Lerp(1.0f, electrical_speed, lerp_value);
					setColor(Color.Lerp(WarmColor, HotColor, lerp_value));
                }
                else
                {
					setColor(Color.Lerp(CoolColor, WarmColor, lerp_value));
                }
            }
            else if (lerp_to_hot)
            {
                lerp_to_hot = false;
                lerp_value = 1;
            }
            else
                lerp_value = 0;
        }

        // Ramp the transparency.
        if (visible)
        {
            // Alpha Up.
            if (alpha_lerp_value < 1)
            {
                alpha_lerp_value += Time.deltaTime / alpha_speed;
				setColor(new Color(rgb.r, rgb.g, rgb.b, alpha_lerp_value));
            }
            else
                alpha_lerp_value = 1;
        }
        else
        {
            // Alpha Down.
            if (alpha_lerp_value > 0)
            {
                alpha_lerp_value -= Time.deltaTime / alpha_speed;
				setColor(new Color(rgb.r, rgb.g, rgb.b, alpha_lerp_value));
            }
            else
                alpha_lerp_value = 0;
        }
    }
}
