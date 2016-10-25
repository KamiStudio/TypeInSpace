using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {
	public float speed;

	void Start ()
	{
		Rigidbody rb = (Rigidbody)GetComponent (typeof(Rigidbody));
		rb.velocity = transform.forward * speed;
	}
}
