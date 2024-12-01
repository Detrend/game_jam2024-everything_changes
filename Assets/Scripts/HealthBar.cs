using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class HealthBar : MonoBehaviour
{
    public float Health 
    {
        get; 
        private set;
    }

    [SerializeField]
    private SpriteRenderer[] _HP_bits;
    [SerializeField] GameObject _hpBitPrefab;

    private int _healthBits;
    private int _displayedBits;
    [SerializeField] private float _healthToBitsRatio;
    private int _maxBits;

    ToiletBlock _toiletBlock;

    public const float MaxHealth = 100;
    [SerializeField]
    private float _maxAnimationTime = 1.0f;
    [SerializeField]
    private float _oneHPLossTime = 0.1f;
    [SerializeField]
    private float _SpacingY = -10.0f;
    private float _neigbourDistance;

    private float _lastPieceLossTime = Single.MinValue;
    // Start is called before the first frame update
    void Start()
    {
        _toiletBlock = FindAnyObjectByType<ToiletBlock>();
        
        Health = MaxHealth;
        _healthBits = (int)(MaxHealth / _healthToBitsRatio);
        _displayedBits = _healthBits;
        _neigbourDistance = _hpBitPrefab.GetComponent<SpriteRenderer>().bounds.size.y + _SpacingY;
        _maxBits = _healthBits;
        _HP_bits = new SpriteRenderer[_maxBits];
        Debug.Log("y size: " + _hpBitPrefab.GetComponent<SpriteRenderer>().bounds.size.y);
        Debug.Log("neigbour distance: " + _neigbourDistance);
        Debug.Log("hp: " + Health);
        for (int i = 0; i < _HP_bits.Length; i++)
        {
            _HP_bits[i] = Instantiate(_hpBitPrefab, transform).GetComponent<SpriteRenderer>();
            // set parent to this object
            _HP_bits[i].transform.SetParent(transform);
            _HP_bits[i].name = "HP_bit_" + i;
            _HP_bits[i].transform.localPosition = new Vector3(0, i * -_neigbourDistance, 0); // we wanna go down - negative y
        }
    }

    public void TakeDamage(float damage)
    {
        float oldHP = Health;
        Health -= damage;
        int oldHPbits = (int)(oldHP / _healthToBitsRatio);
        _healthBits = (int)(Health / _healthToBitsRatio);
        if (Health < 0)
        {
            Health = 0;
            Debug.Log("Game Over");
            return;
        }

        if ( oldHPbits == _healthBits)
        {
            return;
        }
        // gray out the hp bar portion
        for (int i = _healthBits; i < oldHPbits; i++)
        {
            // gray out the hp bit
        }


        // recalculate animation timing
    }

    public void Heal(float heal)
    {
        float oldHP = Health;
        Health += heal;
        Health = Math.Clamp(Health, 0, MaxHealth);
        int oldHPbits = (int)(oldHP / _healthToBitsRatio);
        _healthBits = (int)(Health / _healthToBitsRatio);

        if (oldHPbits == _healthBits)
        {
            return;
        }
        // gray out the hp bar portion
        for (int i = _healthBits; i < oldHPbits; i++)
        {
            // gray out the hp bit
        }


        // recalculate animation timing
    }

    private void CalculateAnimationTiming()
    {
        //int difference = _displayedBits - Health;
    }

    public void UpdateBits()
    {
        float oldHP = Health;
        Health = _toiletBlock.HP;
        Health = Math.Clamp(Health, 0, MaxHealth);
        int oldHPbits = (int)(oldHP / _healthToBitsRatio);
        _healthBits = (int)(Health / _healthToBitsRatio);

        if (Health <= 0)
        {
            Health = 0;
            Debug.Log("Game Over");
            SceneManager.LoadScene("GameOver");
            return;
        }

        if (oldHPbits == _healthBits)
        {
            return;
        }
        for (int i = _healthBits; i < oldHPbits; i++)
        {
            // gray out the hp bit
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_toiletBlock == null)
        {
            // game over
            return;
        }
        UpdateBits();
        //Health = Math.Clamp(Health, 0, MaxHealth);

        if (_healthBits != _displayedBits)
        {
            if (Time.time - _lastPieceLossTime > _oneHPLossTime)
            {
                if (_healthBits < _displayedBits)
                {
                    _displayedBits--;
                    // destroy the hp bit
                    _HP_bits[_displayedBits].DOFade(0.0f, 1.25f);
                    // generate a random blend vector between down and right
                    var blend = Random.Range(0.25f, 0.75f);
                    _HP_bits[_displayedBits].transform.DOBlendableLocalMoveBy(blend * 1.5f * Vector3.right, 1.5f, false).SetEase(Ease.InOutBounce);
                    _HP_bits[_displayedBits].transform.DOBlendableLocalMoveBy((1-blend) * 1.5f * Vector3.down, 1f, false).SetEase(Ease.InOutBounce);
                    Destroy(_HP_bits[_displayedBits].gameObject, 1.51f);
                    _HP_bits[_displayedBits] = null;
                }
                else
                {
                    
                    Debug.Log("new displayed bits: " + _displayedBits);
                    // create a new hp bit
                    var newbit = Instantiate(_hpBitPrefab, transform).GetComponent<SpriteRenderer>();
                    newbit.transform.localPosition = new Vector3(0, _displayedBits * -_neigbourDistance, 0);
                    _HP_bits[_displayedBits] = newbit;
                    newbit.name = "HP_bit_" + _displayedBits;
                    newbit.color = new Color(1f, 1f, 1f, 0.00f);
                    newbit.DOFade(1f, 1f);
                    _displayedBits++;
                }
                _lastPieceLossTime = Time.time;
            }
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var neightbourDistance = _hpBitPrefab.GetComponent<SpriteRenderer>().bounds.size.y + _SpacingY;
        var bits = (int)(MaxHealth / _healthToBitsRatio);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, bits * -neightbourDistance, 0));
        //draw circles for first two points and last one
        Gizmos.DrawSphere(transform.position, 0.05f);
        Gizmos.DrawSphere(transform.position + new Vector3(0, -neightbourDistance, 0), 0.05f);
        Gizmos.DrawSphere(transform.position + new Vector3(0, bits * -neightbourDistance, 0), 0.05f);
    }
}
