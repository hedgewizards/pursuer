using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiphonEffect : MonoBehaviour
{
    ParticleSystem p;
    public Transform target;
    Vector3 midPoint;
    public float midpointElasticity = 0.5f; //the speed at which the midPoint of the curve approaches the middle
    public float offsetRadius = 4f;
    float disableTime;
    public float maxSize = 2f;
    public float minSize = 0.3f;

    void Start()
    {
    }
    
    public void Initialize(Transform _target, float duration, float amount)
    {
        target = _target;

        midPoint = (target.position + transform.position) / 2; //assign a starting midpoint
        p = GetComponent<ParticleSystem>();

        SetDrainAmount(amount);

        disableTime = Time.time + duration;
    }

    public void SetDrainAmount(float frac)
    {
        ParticleSystem.SizeOverLifetimeModule ps = p.sizeOverLifetime;
        ps.sizeMultiplier = Mathf.Lerp(minSize, maxSize, frac);
        
        //ParticleSystem.EmissionModule pe = p.emission;
        //pe.rateOverTime = 20 * frac;
    }

    void Update()
    {
        if(disableTime < Time.time)
        {
            p.Stop();
            Destroy(gameObject,1);
        }


        //move midpoint towards the exact middle
        midPoint = Vector3.Lerp(midPoint, (target.position + transform.position) / 2, midpointElasticity * Time.deltaTime);

        ParticleSystem.Particle[]  particles = new ParticleSystem.Particle[p.particleCount];
        List<Vector4> offset = new List<Vector4>();

        p.GetParticles(particles);
        p.GetCustomParticleData(offset, ParticleSystemCustomData.Custom1);

        for(int n = 0; n < particles.GetUpperBound(0); n++)
        {
            //calculate t-value for use with parametric equation
            float t = 1 - (particles[n].remainingLifetime / particles[n].startLifetime);
            //particles[n].position = Vector3.Lerp(particles[n].position,GetCurvePoint(t) - transform.position,t/4);
            particles[n].position = GetCurvePoint(t) - transform.position + GetOffset(t, offset[n]);
        }

        p.SetParticles(particles, particles.Length);
    }
    Vector3 GetCurvePoint(float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1 - t;
        return
               oneMinusT * oneMinusT * transform.position +
               2 * oneMinusT * t * midPoint +
               t * t * target.position;
        //http://catlikecoding.com/unity/tutorials/curves-and-splines/
        //pretty sick copy paste by me if i do say so myself
        //i get how this works i just feel dirty not doing the math myself
    }
    Vector3 GetOffset(float t, Vector3 maxOffset)
    {
        return maxOffset * Mathf.Lerp(offsetRadius, 0, t);
    }
}
    