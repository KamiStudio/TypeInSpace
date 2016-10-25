using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour {
	public float tumble;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = (Rigidbody)GetComponent(typeof(Rigidbody));
		rb.angularVelocity = Random.insideUnitSphere * tumble;
	}
}
