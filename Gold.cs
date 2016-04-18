using UnityEngine;
using System.Collections;

public class Gold : Pickup {

	// Use this for initialization
	void Start () {
		amount = Random.Range(10, 1000);
	}
}
