using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

/*
 * Works with UI Line Renderer, easily replacable with the standard Unity LineRenderer
 */

public class RandomGraph : MonoBehaviour {

    [SerializeField]
    private int PointCount = 100;

    [Space]
    public float SamplingVel = 1f;
    public float PerlinResolution = 0.1f;

    UILineRenderer LineRenderer;

    List<float> yPoints;
    Vector2[] points;
    
    bool isInit = false;
    float lastval;
    float currentSample;

    public int SetPointCount {
        set {
            PointCount = value;
            init();
        }
    }

    public float LastVal { get { return lastval; } }

    void Start () {
        isInit = false;

        LineRenderer = GetComponent<UILineRenderer>();

        if (LineRenderer == null)
        {
            Debug.LogError("We need a Line Renderer to work.");
            return;
        }

        init();
    }
    
    void Update()
    {
        if (!isInit)
        {
            init();
            return;
        }

#if UNITY_EDITOR
        if (PointCount != points.Length)
        {
            SetPointCount = PointCount;
        }
#endif

        PushNewRandomPoint();

        LineRenderer.Points = points;
        LineRenderer.SetVerticesDirty();
    }

    void init()
    {
        points = new Vector2[PointCount];
        yPoints = new List<float>();

        currentSample = Time.time;

        for (int i = 0; i < PointCount; i++)
        {
            float yVal = 0f;

            yPoints.Add(yVal) ;

            points[i] = new Vector2(
                (float)((float)i / (float)PointCount), 
                yVal
            );
        }

        LineRenderer.Points = points;

        isInit = true;
    }

    void pushNewRandomPoint()
    {
        yPoints.RemoveAt(yPoints.Count -1);
        yPoints.Insert(0, newSampleValue());

        setPointValuesOnY(yPoints.ToArray());
    }

    void setPointValuesOnY(float[] vals)
    {
        for (int i = 0; i < PointCount; i++)
        {
            points[i].y = vals[i];
        }
    }

    float newSampleValue()
    {
        currentSample += Time.deltaTime * SamplingVel;

        lastval = getRandomValue(currentSample);

        return lastval;
    }

    float getRandomValue(float sample)
    {
        return
            Mathf.Clamp01(
                Mathf.PerlinNoise(
                    sample + (PointCount * PerlinResolution),
                    Time.time)
                );
    }
}