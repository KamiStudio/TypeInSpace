using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary {
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {
	public float speed;
	public float tilt;
	public Boundary boundary;

	private Rigidbody ship;

	void Start ()
	{
		ship = (Rigidbody)GetComponent(typeof(Rigidbody));

		Vector3 movement = new Vector3 (0.0f, 0.0f, 1.0f);
		ship.velocity = movement * speed;

	}

	void FixedUpdate() { // Called before each physics update
		float moveHorizontal = 0.0f; //Input.GetAxis("Horizontal");
		float moveVertical = 0.0f; //Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		ship.velocity = movement * speed;

		ship.position = new Vector3 (
			Mathf.Clamp (ship.position.x, boundary.xMin, boundary.xMax), 
			0.0f,
			Mathf.Clamp (ship.position.z, boundary.zMin, boundary.zMax)
		);

		ship.rotation = Quaternion.Euler (0.0f, 0.0f, ship.velocity.x * -tilt);
	}
}
