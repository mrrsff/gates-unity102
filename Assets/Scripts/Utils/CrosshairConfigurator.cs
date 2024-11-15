using System;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class CrosshairConfigurator : MonoBehaviour
{
    public static CrosshairConfigurator Instance { get; private set; }
    public Transform left;
    public Transform right;
    public Transform top;
    public Transform bottom;
    
    [Range(0.1f, 2f)]
    public float size = 10f;
    [Range(0f, 100f)]
    public float gap = 10f;
    [Range(0.1f, 10f)]
    public float thickness = 0.2f;
    
    public float maxRecoil = 250f;
    public float recoilMultiplier = 10f;
    public float unrecoilSpeed = 10f;

    private float currentGap;
    private float unrecoilStartTime;

    private void Awake()
    {
        Instance = this;
        currentGap = gap;
    }

    private void OnValidate()
    {
        currentGap = gap;
        UpdatePositions();
    }
    
    private void UpdatePositions()
    {
        if (!left || !right || !top || !bottom)
        {
            return;
        }
        
        var gapDelta = currentGap / 2f;
        
        left.localPosition = new Vector3(-thickness / 2f - gapDelta, 0f, 0f);
        right.localPosition = new Vector3(thickness / 2f + gapDelta, 0f, 0f);
        top.localPosition = new Vector3(0f, thickness / 2f + gapDelta, 0f);
        bottom.localPosition = new Vector3(0f, -thickness / 2f - gapDelta, 0f);
        
        left.localScale = new Vector3(size, thickness, 1f);
        right.localScale = new Vector3(size, thickness, 1f);
        top.localScale = new Vector3(thickness, size, 1f);
        bottom.localScale = new Vector3(thickness, size, 1f);
    }
    // recoil method
    public void Recoil(float recoilAmount)
    {
        currentGap = Mathf.Clamp(currentGap + recoilMultiplier * recoilAmount, gap, maxRecoil);
        unrecoilStartTime = Time.time + 0.1f;
    }

    private void Update()
    { 
        if (Time.time >= unrecoilStartTime)
        {
            currentGap = Mathf.Lerp(currentGap, gap, Time.deltaTime * unrecoilSpeed);
        }
        UpdatePositions();
    }
}
