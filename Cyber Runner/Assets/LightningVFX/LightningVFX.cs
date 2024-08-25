using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class LightningVFX : MonoBehaviour
{
    public Transform StartEntity;
    public Transform EndEntity;
    public float MinTime = 0.1f;
    public float MaxTime = 0.5f;

    public float PositionVariance = 2;

    [FormerlySerializedAs("VarianceMultiplierOverHeight")]
    public AnimationCurve VarianceMultiplierOverLength;

    public bool CanUpdate;
    public bool StartEnabled;
    private LineRenderer _lineRenderer;

    private float _nextTime;
    private float _offTime;

    //private PrefabPoolObject _poolObjectRef;

    private Vector3[] _positions;

    private void Reset()
    {
        TryInit();
        _lineRenderer.enabled = true;
    }

    private void Start()
    {
        //_poolObjectRef = GetComponent<PrefabPoolObject>();
    }

    private void Update()
    {
        if (!CanUpdate) { return; }
        
        if (_offTime != 0 && _offTime < Time.time)
        {
            CanUpdate = false;
            _offTime = 0;
            _lineRenderer.enabled = false;
            //_poolObjectRef.Return();
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

                Vector3 diff = EndEntity.position - StartEntity.position;
                float length = diff.magnitude;
                Vector3 dir = diff.normalized;
                Vector3 pos = Vector3.Lerp(StartEntity.position, EndEntity.position, t);
                float offset = Random.Range(-PositionVariance * length, PositionVariance * length) * var;
                _positions[i] = pos + Vector3.Cross(dir, Vector3.forward) * offset;
            }

            _positions[^1] = EndEntity.position;
            _lineRenderer.SetPositions(_positions);
            _lineRenderer.positionCount = _positions.Length;
        }
    }

    private void OnEnable()
    {
        TryInit();

        if (Application.isPlaying && !StartEnabled)
        {
            _lineRenderer.enabled = false;
        }
    }

    public void OnDisable()
    {
        _lineRenderer.enabled = false;
        CanUpdate = false;
        _offTime = 0;
    }

    public void TurnOn(float time = 0)
    {
        if (time > 0)
        {
            _offTime = Time.time + time;
        }
        _lineRenderer.enabled = true;
        _nextTime = GetTime();
        CanUpdate = true;
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
        _nextTime = GetTime() + Random.Range(MinTime, MaxTime);
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
}