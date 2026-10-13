using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class ShowPerfo : MonoBehaviour
{
    private Text perfoText;

    // Smoothed live FPS.
    private float deltaTime;

    // One-second FPS statistics.
    private float statsElapsed;
    private int sampleCount;
    private float fpsSum;
    private float fpsSquaredSum;
    private float averageFps;
    private float fpsStandardDeviation;

    // RAM and VRAM values, refreshed once per second.
    private float memoryElapsed;
    private float ramGb;
    private float vramGb;

    private void Awake()
    {
        perfoText = GetComponent<Text>();
        UpdateMemoryUsage();
    }

    private void Update()
    {
        // Update live FPS and collect samples for the one-second statistics.
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        statsElapsed += Time.deltaTime;
        fpsSum += fps;
        fpsSquaredSum += fps * fps;
        sampleCount++;

        if (statsElapsed >= 1.0f)
        {
            averageFps = fpsSum / sampleCount;
            float variance = (fpsSquaredSum / sampleCount) - (averageFps * averageFps);
            fpsStandardDeviation = Mathf.Sqrt(Mathf.Max(0.0f, variance));

            statsElapsed = 0.0f;
            sampleCount = 0;
            fpsSum = 0.0f;
            fpsSquaredSum = 0.0f;
        }

        // Poll memory usage once per second, independent of time scale.
        memoryElapsed += Time.unscaledDeltaTime;
        if (memoryElapsed >= 1.0f)
        {
            UpdateMemoryUsage();
            memoryElapsed = 0.0f;
        }

        // Refresh the display so the current FPS remains dynamic.
        perfoText.text =
            $"FPS: {Mathf.RoundToInt(fps)} ({averageFps:F1} ± {fpsStandardDeviation:F1})\n" +
            $"RAM: {ramGb:F2}GB\n" +
            $"VRAM: {vramGb:F2}GB";
    }

    private void UpdateMemoryUsage()
    {
        const float bytesPerGb = 1024f * 1024f * 1024f;

        ramGb = Profiler.GetTotalAllocatedMemoryLong() / bytesPerGb;
        vramGb = Profiler.GetAllocatedMemoryForGraphicsDriver() / bytesPerGb;
    }
}