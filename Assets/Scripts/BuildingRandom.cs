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

	private float minFreq = 100000f;

	private float maxFreq = 0f;

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
	
	// Update is called once per frame
	void Update () {
		int index = 0;

		foreach (Transform child in transform) {

			float sum = 0f;
			// Get the sum of the 64 blocks

			for (int offset = 0; offset < numRanges; offset++) {
				sum += visualizer._spectrum [index + offset];
			}

			// Get the frequency for that point
			float freq = sum / numRanges;

			if (freq > maxFreq) {
				maxFreq = freq;
			}

			if (freq < minFreq) {
				minFreq = freq;
			}

			// Result := ((Input - InputLow) / (InputHigh - InputLow)) * (OutputHigh - OutputLow) + OutputLow;

			float percent = (freq - minFreq) / (maxFreq - minFreq);


			float endHeight =  ((freq - minFreq) / (maxFreq - minFreq) * (maxHeight - minHeight)) + minHeight; 

			child.transform.localScale = new Vector3 (child.localScale.x, endHeight, child.localScale.z);

			index += (int) numRanges;

			//Debug.Log (freq);

		}
	}
}
