using UnityEngine;
using System;

public class BezierSpline : MonoBehaviour {

	[SerializeField]
    private Vector3[] points; // All of the points along the path.

	[SerializeField]
	private BezierControlPointMode[] modes; // The handle mode for each segment of the path.

	[SerializeField]
	private bool loop;

	public bool Loop {
		get {
			return loop;
		}
		set {
			loop = value;
			if (value == true) {
				modes[modes.Length - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}

	public int ControlPointCount
    {
		get { return points.Length; }
	}

	public Vector3 GetControlPoint (int index)
    {
		return points[index];
	}

	public void SetControlPoint (int index, Vector3 point) {
		if (index % 3 == 0) {
			Vector3 delta = point - points[index];
			if (loop) {
				if (index == 0) {
					points[1] += delta;
					points[points.Length - 2] += delta;
					points[points.Length - 1] = point;
				}
				else if (index == points.Length - 1) {
					points[0] = point;
					points[1] += delta;
					points[index - 1] += delta;
				}
				else {
					points[index - 1] += delta;
					points[index + 1] += delta;
				}
			}
			else {
				if (index > 0) {
					points[index - 1] += delta;
				}
				if (index + 1 < points.Length) {
					points[index + 1] += delta;
				}
			}
		}
		points[index] = point;
		EnforceMode(index);
	}

	public BezierControlPointMode GetControlPointMode (int index) {
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		int modeIndex = (index + 1) / 3;
		modes[modeIndex] = mode;
		if (loop) {
			if (modeIndex == 0) {
				modes[modes.Length - 1] = mode;
			}
			else if (modeIndex == modes.Length - 1) {
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}

	private void EnforceMode (int index) {
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes[modeIndex];
		if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			if (fixedIndex < 0) {
				fixedIndex = points.Length - 2;
			}
			enforcedIndex = middleIndex + 1;
			if (enforcedIndex >= points.Length) {
				enforcedIndex = 1;
			}
		}
		else {
			fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Length) {
				fixedIndex = 1;
			}
			enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0) {
				enforcedIndex = points.Length - 2;
			}
		}

		Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
		if (mode == BezierControlPointMode.Aligned) {
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
		}
		points[enforcedIndex] = middle + enforcedTangent;
	}

	public int CurveCount {
		get {
			return (points.Length - 1) / 3;
		}
	}

	public Vector3 GetPoint (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Length - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
        // TODO: if node I is a pause node return the nodes position until pause duration is over.

        //else
		return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}
	
	public Vector3 GetVelocity (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Length - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
	}
	
	public Vector3 GetDirection (float t) {
		return GetVelocity(t).normalized;
	}


    ///-----------------------------------------------------------------------------------------------------------
    /// <summary>
    /// The original add functionality.
    /// </summary>
    ///-----------------------------------------------------------------------------------------------------------
    public void AddCurve ()
    {
		Vector3 point = points[points.Length - 1];

		Array.Resize(ref points, points.Length + 3);
		point.x += 1f;
		points[points.Length - 3] = point;
		point.x += 1f;
		points[points.Length - 2] = point;
		point.x += 1f;
		points[points.Length - 1] = point;

		Array.Resize(ref modes, modes.Length + 1);
		modes[modes.Length - 1] = modes[modes.Length - 2];
		EnforceMode(points.Length - 4);

		if (loop)
        {
			points[points.Length - 1] = points[0];
			modes[modes.Length - 1] = modes[0];
			EnforceMode(0);
		}
	}


    ///-----------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Delete the selected Control Point.
    /// </summary>
    ///-----------------------------------------------------------------------------------------------------------
    public bool DeleteCurve(int index)
    {
        bool succeeded = false;

        if (index == 0 || index == 1)
            index = 0;
        else if (index % 3 == 0) // A Control Point is selected.
            index--; // Rewind to the "before" handle.
        else if ((index - 1) % 3 == 0)// The handle after the Contol Point is selected.
            index -= 2; // Rewind to the "before" handle.


        if (index != -1)
        {
            // Copy the point data from the last points to the earlier points before adjusting the size of the array.
            for (int i = index + 3; i < points.Length; i++)
                points[i - 3] = points[i];

            for (int i = (index + 1) / 3 + 1; i < modes.Length; i++)
                modes[i - 1] = modes[i];

            Array.Resize(ref points, points.Length - 3); // Try to delete the last point.
            Array.Resize(ref modes, modes.Length - 1); // Also delete the handle mode assosiated with that segment.

            succeeded = true;
        }

        Debug.Log("Point Count: " + points.Length);
        Debug.Log("Modes Count: " + modes.Length);
        Debug.Log("Delete: " + index);

        return succeeded;
    }


    ///-----------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Insert a new Control Point between the current selection and the next one.
    /// </summary>
    ///-----------------------------------------------------------------------------------------------------------
    public bool InsertCurve(int index)
    {
        // Make room in the arrays.
        Array.Resize(ref points, points.Length + 3);
        Array.Resize(ref modes, modes.Length + 1);

        bool succeeded = false;

        if (index == points.Length - 1) // It's the last control point.
             index -= 3; // Select the previous Control Point.
         else if ((index + 1) % 3 == 0) // The handle before the Control Point is selected.
             index++;
         else if ((index - 1) % 3 == 0) // The handle after the Contol Point is selected.            
             index--; // Select the Contorl Point.

        Vector3 control_point = points[index];
        Vector3 next_control_point = points[index + 3];

        if (index != -1)
        {
            // Copy the point data from the earlier points to the last points.
            for (int i = points.Length - 1; i > index; i--)
            {
                if (i <= 4) // 01---23456 | First Point was selected.
                    break;
                else if (index == points.Length - 4 && i <= index + 1) // 01234---56 | Last Point was selected.
                    break;
                else
                    points[i] = points[i - 3];
            }

            for (int i = modes.Length - 1; i > index; i--)
                modes[i] = modes[i - 1];

            EnforceMode(index);

            // Position the new insert point.
            if (index == points.Length - 4)
            {
                // The Last Control Point was selected so insert the new point before.
                Vector3 spawning_point = (points[index - 3] - points[index + 3]) / 2 + points[index + 3];
                Vector3 spawn_first_handle = (points[index - 3] - spawning_point) / 2 + spawning_point;
                Vector3 spawn_last_handle = (spawning_point - points[index + 3]) / 2 + points[index + 3];

                points[index - 1] = spawn_first_handle;
                points[index + 0] = spawning_point;
                points[index + 1] = spawn_last_handle;
            }
            else
            {
                Vector3 spawning_point = (control_point - next_control_point) / 2 + next_control_point;
                Vector3 spawn_first_handle = (control_point - spawning_point) / 2 + spawning_point;
                Vector3 spawn_last_handle = (spawning_point - next_control_point) / 2 + next_control_point;

                points[index + 2] = spawn_first_handle;
                points[index + 3] = spawning_point;
                points[index + 4] = spawn_last_handle;
            }

            succeeded = true;
        }

        return succeeded;
    }

    public void Reset()
    {
		points = new Vector3[]
        {
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};

		modes = new BezierControlPointMode[]
        {
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
		};
	}
}