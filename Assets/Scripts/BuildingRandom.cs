using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRandom : MonoBehaviour {

	public GameObject cameras;
	private CameraVisualizer visualizer;

	public float minHeight = 16f;
	public float maxHeight = 18f;
	public int numOfBuildings  = 16;

	private float cubeNum;
	private float numRanges;

	// Current max/min frequencies
	private float minFreq = 100000f;
	private float maxFreq = 0f;

	// Max color range
	private int MAX_COLOR = 16777215;

	// Use this for initialization
	void Start () {

		float degreePerBuilding = 360 / numOfBuildings;
		cubeNum = numOfBuildings / 2;
		float currentDegree = 0f;

		foreach (Transform child in transform) {
			child.transform.rotation = Quaternion.Euler(currentDegree, 90f, 0f);
			currentDegree += degreePerBuilding;
		}
		visualizer = cameras.GetComponent<CameraVisualizer> ();

		numRanges = visualizer._spectrum.Length / (numOfBuildings);
	}

	Color32 intToRgb(int colorNum) {
		int r = (colorNum >> 16) & 255;
		int g = (colorNum >> 8) & 255;
		int b = colorNum & 255;

		return new Color32((byte)r, (byte) g, (byte) b, (byte) 255);
	}
	
	// Update is called once per frame
	void Update () {
		int index = 0;

		foreach (Transform child in transform) {

			float sum = 0f;
			// Get the sum of the 64 blocks

			for (int offset = 0; offset < 3; offset++) {
				sum += visualizer._spectrum [index + offset];
			}

			// Get the frequency for that point
			float freq = sum / 3;//numRanges;

			//float freq = visualizer._spectrum [index];

			if (freq > maxFreq) {
				maxFreq = Mathf.Clamp(freq, freq, .022f);
			}

			if (freq < minFreq) {
				minFreq = freq;
			}

			// Result := ((Input - InputLow) / (InputHigh - InputLow)) * (OutputHigh - OutputLow) + OutputLow;

			float percent = (freq - minFreq) / (maxFreq - minFreq);


			float endHeight =  Mathf.Clamp((percent * (maxHeight - minHeight)) + minHeight, minHeight, maxHeight+1); 

			int colorInt = (int) (percent * MAX_COLOR);

			child.GetComponent<Renderer> ().material.color = intToRgb (colorInt);

			child.transform.localScale = new Vector3 (child.localScale.x, endHeight, child.localScale.z);

			//index += (int) numRanges;
			index+=3;
			//Debug.Log (freq);

		}
	}
}
