using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class CameraVisualizer : MonoBehaviour {



	float RmsValue;
	float DbValue;
	float PitchValue;

	public float maxDistance = .2f;
	public float minDistance = -.2f;
	public float adjust = -5f;
	public float multiplier = 3f;

	// For chainsmokers, pitch from 1f -> 3000f

	private float max = 0f, min = 10000f;

	private const int QSamples = 1024;
	private const float RefValue = 0.1f;
	private const float Threshold = 0.02f;

	float[] _samples;
	[HideInInspector] public float[] _spectrum;   // This will give all of the values
	private float _fSample;

	void Start()
	{
		_samples = new float[QSamples];
		_spectrum = new float[QSamples];
		_fSample = AudioSettings.outputSampleRate;
	}

	void Update()
	{
		AnalyzeSound();

		float frequencyMult = Mathf.Clamp (DbValue, -160f, 17f) + 120f;

		float moveDistance = (((maxDistance - minDistance) * frequencyMult) / 17f) * multiplier;

		transform.transform.position = new Vector3(0f, 0f, adjust + (moveDistance - Mathf.Abs(minDistance)));


	}

	void AnalyzeSound()
	{
		GetComponent<AudioSource>().GetOutputData(_samples, 0); // fill array with samples
		int i;
		float sum = 0;
		for (i = 0; i < QSamples; i++)
		{
			sum += _samples[i] * _samples[i]; // sum squared samples
		}
		RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
		DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
		if (DbValue < -160) DbValue = -160; // clamp it to -160dB min
		// get sound spectrum
		GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
		float maxV = 0;
		var maxN = 0;
		for (i = 0; i < QSamples; i++)
		{ // find max 
			if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
				continue;

			maxV = _spectrum[i];
			maxN = i; // maxN is the index of max
		}
		float freqN = maxN; // pass the index to a float variable
		if (maxN > 0 && maxN < QSamples - 1)
		{ // interpolate index using neighbours
			var dL = _spectrum[maxN - 1] / _spectrum[maxN];
			var dR = _spectrum[maxN + 1] / _spectrum[maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}
		PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency

	}
}