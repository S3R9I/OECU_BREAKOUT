﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof (AudioSource))]
public class AudioDataCollector : MonoBehaviour {
    AudioSource _audioSource;
    public static float[] _samples =  new float[512];
    public static float[] _freqBand = new float[8];
    public static float[] _bandBuffer = new float[8];
    public static float[] _bufferDecrease = new float[8];

    private float[] _freqBandHighest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];
    public float _audioProfile;
    // Use this for initialization
    void Start () {
        _audioSource = GetComponent<AudioSource>();
        AudioProfile(_audioProfile);
	}
	
	// Update is called once per frame
	void Update () {
        GetSpectrumAudioSource();
        InitFrequencyBands();
        BandBuffer();
        CreateAudioBand();
    }

    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < _freqBandHighest.Length; i++)
        {
            _freqBandHighest[i] = audioProfile;
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for (int i = 0; i < _freqBand.Length; i++)
        {
            if(_freqBand[i] > _bandBuffer[i])
            {
                _bandBuffer[i] = _freqBand[i];
                _bufferDecrease[i] = 0.005f;
            }
            if (_freqBand[i] < _bandBuffer[i])
            {
                _bandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= 1.2f;
            }

        }
    }

    void InitFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < _freqBand.Length; i++)
        {
            float avg = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if(i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                avg += _samples[count] * (count + 1);
                count++;
            }

            avg /= count;
            _freqBand[i] = avg * 10;
        }
    }

    void CreateAudioBand()
    {
        for (int i = 0; i < _freqBand.Length; i++)
        {
            if(_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];

            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }
}
