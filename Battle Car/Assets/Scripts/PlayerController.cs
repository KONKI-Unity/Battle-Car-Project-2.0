using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {
    
    private Animator animator;

	// Use this for initialization
	void Start () {

        animator = GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
        float _zMov = Input.GetAxis("Horizontal");
        animator.SetFloat("Fowardvelocity", _zMov);
	}
}
