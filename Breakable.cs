using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {
	
	
	public int health;
	public Texture insideDestructedTexture;
	public Texture outsideDestructedTexture;
	
	// Update is called once per frame
	void Update () {
		if(health <= 0) {
			if(gameObject.collider) {
				Destroy(gameObject.collider);
			}
			
			transform.Find("insideWall").gameObject.renderer.material.mainTexture = insideDestructedTexture;
			transform.Find("outsideWall").gameObject.renderer.material.mainTexture = outsideDestructedTexture;
		}
	}
}
