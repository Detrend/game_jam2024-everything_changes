using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _blockPrefabs;


    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _conveyorBeltSfx;

    [SerializeField]
    private float _spawningFrequency = 8f;
    private float _timeSinceLastSpawn = Single.MinValue;

    [SerializeField]
    private GameObject spawnPoint;

    private Vector3 _spawnPoint;

    private int _phase = 0;

    private List<float[]> _probDistributions = new List<float[]>()
    {
        new float[] { 6.5f, 0.0f, 0.0f, 0.0f, 0.0f },
        new float[] { 5.5f, 0.5f, 2.0f, 0.0f, 0.0f },
        new float[] { 3.5f, 1.5f, 2.0f, 0.5f, 0.0f },
        new float[] { 3.0f, 3.5f, 1.5f, 1.5f, 0.0f },
        new float[] { 3.5f, 1.5f, 2.0f, 1.0f, 0.0f },
        
    };

    private float _startTime;
    private List<float> _distributionTimings = new List<float>()
    {
        35f, 70f, 105f, 195f,
    };

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _startTime = Time.time;
        _spawnPoint = spawnPoint.transform.position;
        if(false)
        {
            Debug.LogError("Block prefabs and probabilities arrays must have the same length");
            return;
        }

        if (_distributionTimings.Count >= _probDistributions.Count)
        {
            Debug.LogWarning("You need at least one more distribution that timing");
        }
        if (_probDistributions[0].Length < (int)BlockType.COUNT)
        {
            // make uniform distribution of the desired length
            for (int i = 0; i < _probDistributions.Count; i++)
            {
                float[] distribution = new float[(int)BlockType.COUNT];
                for (int j = 0; j < distribution.Length; j++)
                {
                    distribution[j] = 1f / distribution.Length;
                }
                _probDistributions[i] = distribution;
            }
            Debug.LogWarning("Probabilities array must have at least BlockType.COUNT elements");
            return;
        }
        else
        {
            // normalize probabilities
            for (int i = 0; i < _probDistributions.Count; i++)
            {
                float sum = 0;
                for (int j = 0; j < _probDistributions[i].Length; j++)
                {
                    sum += _probDistributions[i][j];
                }
                for (int j = 0; j < _probDistributions[i].Length; j++)
                {
                    _probDistributions[i][j] /= sum;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_phase < _distributionTimings.Count && _distributionTimings[_phase] < Time.time -_startTime)
        {
            _phase++;
            Debug.Log("switching to a new distribution:" + _phase);
        }
        if(Time.time - _timeSinceLastSpawn > _spawningFrequency)
        {
            _timeSinceLastSpawn = Time.time;
            SpawnBlock();
            _audioSource.clip = _conveyorBeltSfx;
            _audioSource.Play();
        }
    }

    private void SpawnBlock()
    {
        
        float random = UnityEngine.Random.Range(0f, 1f);
        float sum = 0;
        var probabilities = _probDistributions[_phase];
        for (int i = 0; i < probabilities.Length; i++)
        {
            sum += probabilities[i];
            if(random < sum)
            {
                Debug.Log("Spawning block of type " + i);
                GameObject block = Instantiate(_blockPrefabs[i], _spawnPoint, Quaternion.identity);
                var rigidbody = block.GetComponent<Rigidbody2D>();
                rigidbody.bodyType = RigidbodyType2D.Dynamic;
                rigidbody.freezeRotation = false;
                break;
            }
        }
        
        //GameObject block = Instantiate(_blockPrefabs[0], _spawnPoint, Quaternion.identity);

    }
}
