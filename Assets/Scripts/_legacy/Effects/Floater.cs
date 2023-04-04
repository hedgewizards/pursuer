using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour {

    Vector3 vel;
    public static float lifetime = 0.5f;
    public static float maxScaleDamage = 100; //100+ damage is max sized/colored
    public static float minScaleDamage = 5; //5- damage is min sized/colored
    public static float maxScale = 0.05f;
    public static float minScale = 0.02f;
    public static Color minColor = new Color(1,1,1);
    public static Color maxColor = new Color(1,0,0);
    public float ySpd = 2;
    public float gravity = 10;

	// Use this for initialization
	void Start ()
    {
        Destroy(gameObject, lifetime);
	}

    public void Initialize(float damage)
    {
        float rand = Random.value * 2 * Mathf.PI;
        vel = new Vector3(Mathf.Cos(rand), ySpd, Mathf.Sin(rand));

        Vector3 forwardDir = Camera.main.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(-forwardDir, Vector3.up);
        TextMesh t = GetComponent<TextMesh>();
        t.text = Mathf.Floor(damage).ToString(); //set text

        float scale = (damage - minScaleDamage) / (maxScaleDamage - minScaleDamage);
        scale = Mathf.Clamp(scale, 0, 1);

        t.color = Color.Lerp(minColor, maxColor, scale); //set color
        transform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, scale); //set size
    }

    public void Update()
    {
        transform.position += vel * Time.deltaTime;

        vel += Vector3.down * gravity * Time.deltaTime;
    }
}
