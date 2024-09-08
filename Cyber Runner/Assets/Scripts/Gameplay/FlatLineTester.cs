using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class FlatLineTester : MonoBehaviour
{
    private Enemy _a;
    private Enemy _b;
    public Transform GunStartPos = null;
    
    public float MinTime = 0.1f;
    public float MaxTime = 0.5f;

    public float PositionVariance = 2;
    
    public AnimationCurve VarianceMultiplierOverLength;

    public bool CanUpdate;
    public bool StartEnabled;
    private LineRenderer _lineRenderer;

    private float _nextTime;
    private float _offTime;

    private Vector3[] _positions;
    
    public Enemy A
    {
        get
        {
            return _a;
        }
        set
        {
            _a = value;
            Validate();
        }
    }
    public Enemy B
    {
        get
        {
            return _b;
        }
        set
        {
            _b = value;
            Validate();
        }
    }
    public LineRenderer lr;
    void Start()
    {
    
    }

    private void OnDisable()
    {
        GunStartPos = null;
    }

    private void Validate()
    {
        if (_a == null || (_b == null && GunStartPos == null ))
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
    
    private float GetTime()
    {
        if (Application.isPlaying)
        {
            return Time.time;
        }
#if UNITY_EDITOR
        return (float)EditorApplication.timeSinceStartup;
#else
        return Time.time;
#endif
    }
    
    private void TryInit()
    {
        if (_lineRenderer == null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _positions = new Vector3[_lineRenderer.positionCount];
            SetNextTime();
        }
    }

    private void SetNextTime()
    {
        _nextTime = GetTime() + UnityEngine.Random.Range(MinTime, MaxTime);
    }
    
    void Update()
    {
        
        if (GunStartPos != null)
        {

            if ((A != null && A.State == EnemyState.Dead))
            {
                A = null;
                B = null;
                gameObject.SetActive(false);
            }
            return;
        }

        if ((B != null && B.State == EnemyState.Dead) || (A != null && A.State == EnemyState.Dead))
        {
            A = null;
            B = null;
            gameObject.SetActive(false);
        }
        else
        {
           
        }
        
        
        if ((A == null || B == null) && GunStartPos == null)
        {
            return;
        }

        Vector3 bPositionOverride;
        if (GunStartPos!= null)
        {
            bPositionOverride = GunStartPos.position;
        }
        else
        {
            bPositionOverride = B.transform.position;
        }
        
            TryInit();
            SetNextTime();
            _lineRenderer.GetPositions(_positions);

            for (int i = 0; i < _positions.Length - 1; i++)
            {
                float t = i / ((float)_positions.Length - 1);
                float var = VarianceMultiplierOverLength.Evaluate(t);

                Vector3 diff = bPositionOverride - A.transform.position;
                float length = diff.magnitude;
                Vector3 dir = diff.normalized;
                Vector3 pos = Vector3.Lerp(A.transform.position, bPositionOverride, t);
                float offset = UnityEngine.Random.Range(-PositionVariance * length, PositionVariance * length) * var;
                _positions[i] = pos + Vector3.Cross(dir, Vector3.forward) * offset;
            }

            _positions[0] = A.transform.position;
            _positions[^1] = bPositionOverride;

            _lineRenderer.SetPositions(_positions);
            _lineRenderer.positionCount = _positions.Length;
        
       
    }

    private void LateUpdate()
    {

        if ((A == null || B == null) && GunStartPos == null)
        {
            return;
        }

        Vector3 bPositionOverride;
        if (GunStartPos!= null)
        {
            bPositionOverride = GunStartPos.position;
        }
        else
        {
            bPositionOverride = B.transform.position;
        }
        
        if (GetTime() > _nextTime)
        {
            TryInit();
            SetNextTime();
            _lineRenderer.GetPositions(_positions);

            for (int i = 0; i < _positions.Length - 1; i++)
            {
                float t = i / ((float)_positions.Length - 1);
                float var = VarianceMultiplierOverLength.Evaluate(t);

                Vector3 diff = bPositionOverride - A.transform.position;
                float length = diff.magnitude;
                Vector3 dir = diff.normalized;
                Vector3 pos = Vector3.Lerp(A.transform.position, bPositionOverride, t);
                float offset = UnityEngine.Random.Range(-PositionVariance * length, PositionVariance * length) * var;
                _positions[i] = pos + Vector3.Cross(dir, Vector3.forward) * offset;
            }

            _positions[0] = A.transform.position;
            _positions[^1] = bPositionOverride;

            _lineRenderer.SetPositions(_positions);
            _lineRenderer.positionCount = _positions.Length;
        }
  
    }
}
