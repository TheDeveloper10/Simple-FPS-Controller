using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SFPSC_Rope : MonoBehaviour
{
    [Header("Values")]
    public AnimationCurve effectOverTime;
    public AnimationCurve curve;
    public AnimationCurve curveEffectOverDistance;
    public float curveSize = 5;
    public float scrollSpeed = 5;
    public int segments = 100;
    public float animSpeed = 1.5f;

    private LineRenderer lineRenderer;
    private void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
    }

    private Vector3 _start;
    private Vector3 _end;
    private float _time;
    private bool _active;

    public void UpdateGrapple()
    {
        lineRenderer.enabled = _active;
        if (_active)
            ProcessBounce();
    }

    private Vector3[] vectors;
    private Quaternion forward;
    private Vector3 up, defaultPos;
    private float delta, realDelta, calcTime, effect;
    private int i = 0, d = 0;
    private void ProcessBounce()
    {
        vectors = new Vector3[segments + 1];
        _time = Mathf.MoveTowards(_time, 1f,
            Mathf.Max(Mathf.Lerp(_time, 1f, animSpeed * Time.deltaTime) - _time, 0.2f * Time.deltaTime));

        vectors[0] = _start;

        forward = Quaternion.LookRotation(_end - _start);
        up = forward * Vector3.up;

        for (i = 1; i < segments + 1; i++)
        {
            delta = 1f / segments * i;
            realDelta = delta * curveSize;
            
            if(realDelta > 1.0f)
            {
                d = (int)(realDelta - 1.0f);
                realDelta -= d;
                if (realDelta > 1.0f)
                    realDelta -= 1.0f;
            }
            
            calcTime = realDelta + -scrollSpeed * _time;
            if (calcTime < 0f)
            {
                calcTime -= (int)calcTime;
                if (calcTime < 0f)
                    calcTime += 1.0f;
            }

            defaultPos = GetPos(delta);
            effect = Eval(effectOverTime, _time) * Eval(curveEffectOverDistance, delta) * Eval(curve, calcTime);

            vectors[i] = defaultPos + up * effect;
        }

        lineRenderer.positionCount = vectors.Length;
        lineRenderer.SetPositions(vectors);
    }

    private Vector3 GetPos(float d)
    {
        return Vector3.Lerp(_start, _end, d);
    }

    private static float Eval(AnimationCurve ac, float t)
    {
        return ac.Evaluate(t * ac.keys.Select(k => k.time).Max());
    }

    public void Grapple(Vector3 start, Vector3 end)
    {
        _active = true;
        _time = 0f;

        _start = start;
        _end = end;
    }

    public void UnGrapple()
    {
        _active = false;
    }

    public void UpdateStart(Vector3 start)
    {
        _start = start;
    }
}
