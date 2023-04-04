using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereManager : MonoBehaviour
{
    // Components
    public static AtmosphereManager self;
    [HideInInspector] public Camera mainCamera;

    public Color initialSkyColor;

    public Color initialFogColor;
    public float initialFogStart;
    public float initialFogEnd;

    // Messages
    private void Awake()
    {
        self = this;
        mainCamera = Camera.main;

        SetSky(initialSkyColor);
        SetFog(initialFogColor, initialFogStart, initialFogEnd);
    }
    private void OnApplicationQuit()
    {
        SetFog(Color.white, 0, 0, 0);
    }

    // Internal Functions

    // Fog Stuff
    static void SetFogRange(Vector2 range)
    {
        RenderSettings.fogStartDistance = range.x;
        RenderSettings.fogEndDistance = range.y;
    }
    static Vector2 GetFogRange()
    {
        return new Vector2(RenderSettings.fogStartDistance, RenderSettings.fogEndDistance);
    }
    void SetFogAsync(Color color, float start, float end, float fadeDuration)
    {
        if (end == 0)
        {
            if (RenderSettings.fogEndDistance != 0)
                StartCoroutine(RecedeFog(fadeDuration));
        }
        else if (RenderSettings.fogEndDistance == 0)
        {
            StartCoroutine(AdvanceFog(color, new Vector2(start, end), fadeDuration));
        }
        else
        {
            StartCoroutine(ChangeFog(color, new Vector2(start, end), fadeDuration));
        }
    }
    /// <summary>
    /// Send fog away to infinity
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator RecedeFog(float duration)
    {
        float startTime = Time.time;
        Color initialColor = RenderSettings.fogColor;
        Vector2 initialRange = GetFogRange();
        Vector2 newRange = new Vector2(initialRange.x, 1000);

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            RenderSettings.fogColor = Extensions.AccelerateInterpolate(initialColor, Color.white, t);
            SetFogRange(Extensions.AccelerateInterpolate(initialRange, newRange, t));

            yield return null;
        }


        RenderSettings.fogColor = Color.white;
        SetFogRange(new Vector2(0, 0));
    }
    /// <summary>
    /// Bring in fog from infinity
    /// </summary>
    /// <param name="color"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="fadeDuration"></param>
    /// <returns></returns>
    IEnumerator AdvanceFog(Color newColor, Vector2 newRange, float duration)
    {
        float startTime = Time.time;
        Vector2 initialRange = new Vector2(newRange.x, 1000);

        while (Time.time < startTime + duration)
        {
            float t = 1 - (Time.time - startTime) / duration;
            RenderSettings.fogColor = Extensions.AccelerateInterpolate(Color.white, newColor, t);
            SetFogRange(Extensions.AccelerateInterpolate(initialRange, newRange, t));

            yield return null;
        }

        RenderSettings.fogColor = newColor;
        SetFogRange(newRange);
    }
    /// <summary>
    /// Change fog between 2 positions / colors
    /// </summary>
    /// <param name="color"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator ChangeFog(Color newColor, Vector2 newRange, float duration)
    {
        float startTime = Time.time;
        Color initialColor = RenderSettings.fogColor;
        Vector2 initialRange = GetFogRange();

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            RenderSettings.fogColor = Extensions.SmoothInterpolate(initialColor, newColor, t);
            SetFogRange(Extensions.SmoothInterpolate(initialRange, newRange, t));

            yield return null;
        }

        RenderSettings.fogColor = newColor;
        SetFogRange(newRange);
    }

    void SetSkyAsync(Color color, float duration)
    {
        StartCoroutine(ChangeSky(color, duration));
    }
    IEnumerator ChangeSky(Color newColor, float duration)
    {
        float startTime = Time.time;
        Color initialColor = mainCamera.backgroundColor;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            mainCamera.backgroundColor = Extensions.SmoothInterpolate(initialColor, newColor, t);
            yield return null;
        }

        mainCamera.backgroundColor = newColor;
    }

    // Global Functions
    /// <summary>
    /// Change the global fog properties. if "end" is zero, disable fog
    /// </summary>
    /// <param name="color"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="fadeDuration"></param>
    public static void SetFog(Color color, float start, float end, float fadeDuration = 0)
    {
        self.SetFogInstanced(color, start, end, fadeDuration);
    }
    public void SetFogInstanced(Color color, float start, float end, float fadeDuration = 0)
    {
        if (fadeDuration == 0)
        {
            RenderSettings.fogColor = color;
            SetFogRange(new Vector2(start, end));
        }
        else
        {
            SetFogAsync(color, start, end, fadeDuration);
        }
    }
    public static void SetSky(Color color, float fadeDuration = 0)
    {
        self.SetSkyInstanced(color, fadeDuration);
    }
    public void SetSkyInstanced(Color color, float fadeDuration = 0)
    {
        if (fadeDuration == 0)
        {
            mainCamera.backgroundColor = color;
        }
        else
        {
            SetSkyAsync(color, fadeDuration);
        }
    }
}
