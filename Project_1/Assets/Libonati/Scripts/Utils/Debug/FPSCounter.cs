using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace UnityStandardAssets.Utility
{
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display = "{0} FPS";
		public TextMesh textMesh;
		public UnityEngine.UI.Text guiTxt;


		private float startTime = 0;
		private string fpsText = "";
		private Stopwatch stopwatch;
		public string savedTxt = "";

		private string text{
			set{
				if (textMesh != null) {
					textMesh.text = value;
				}
				if (guiTxt != null) {
					guiTxt.text = value;
				}
				savedTxt = value;
			}
			get{
				return fpsText;
			}
		}

		private void Awake(){
			stopwatch = new Stopwatch ();
			m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
		}
        private void Start()
        {

			StartCoroutine (EndOfFrame());
        }

		public void overrideLog(string text){
			this.text = fpsText + "\n" + text;
		}


        private void Update()
		{
			startTime = Time.realtimeSinceStartup;
			stopwatch.Start ();

			// measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int) (m_FpsAccumulator/fpsMeasurePeriod);
                m_FpsAccumulator = 0;
				m_FpsNextPeriod += fpsMeasurePeriod;
				fpsText = string.Format(display, m_CurrentFps);
				text = fpsText;
            }
        }

		private IEnumerator EndOfFrame(){
			while (true) {
				yield return new WaitForEndOfFrame ();
				float frameTimeMS = (Time.realtimeSinceStartup - startTime) * 1000;
				frameTimeMS = stopwatch.ElapsedMilliseconds;
				stopwatch.Reset();

				string text = frameTimeMS.ToString("F2") + " MS";
				//fpsText = string.Format(display, (1f / frameTimeMS * 1000f).ToString("F0"));
				overrideLog(text);
			}
		}
    }
}
