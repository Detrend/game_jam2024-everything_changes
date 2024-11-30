using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _blockPrefabs;
    [SerializeField]
    private float[] blocksProbabilities;

    [SerializeField]
    private float _spawningFrequency = 8f;
    private float _timeSinceLastSpawn = Single.MinValue;

    [SerializeField]
    private GameObject spawnPoint;

    private Vector3 _spawnPoint;


    private List<GameObject> _blocks = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        _spawnPoint = spawnPoint.transform.position;
        if(_blockPrefabs.Length != blocksProbabilities.Length)
        {
            Debug.LogError("Block prefabs and probabilities arrays must have the same length");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - _timeSinceLastSpawn > _spawningFrequency)
        {
            _timeSinceLastSpawn = Time.time;
            SpawnBlock();
        }
    }

    private void SpawnBlock()
    {
        /*
        float random = UnityEngine.Random.Range(0f, 1f);
        float sum = 0;
        for (int i = 0; i < blocksProbabilities.Length; i++)
        {
            sum += blocksProbabilities[i];
            if(random < sum)
            {
                GameObject block = Instantiate(_blockPrefabs[i], _spawnPoint, Quaternion.identity);
                _blocks.Add(block);
                break;
            }
        }
        */
        GameObject block = Instantiate(_blockPrefabs[0], _spawnPoint, Quaternion.identity);
        var rigidbody = block.GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        rigidbody.freezeRotation = false;

        _blocks.Add(block);
    }
}
