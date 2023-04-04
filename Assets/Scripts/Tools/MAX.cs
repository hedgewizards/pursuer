using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAX : MonoBehaviour {

    public Animator anim;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
        anim.Play("idle", -1, 0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}
}
