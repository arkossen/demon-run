using UnityEngine;
using System.Collections;

public class LookAtSprite : MonoBehaviour {
	
	public void LookAtPlayer() {
		Vector3 rot = transform.rotation.eulerAngles;
		transform.LookAt(GameObject.Find("player").transform);
		rot.y = transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
	}
}
