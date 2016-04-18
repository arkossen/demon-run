using UnityEngine;
using System.Collections;

public class Pickup : LookAtSprite {
	
	public string type;
	public int amount;
	public GameObject prefab;
	public AudioClip sndPickup;
	public Texture[] textures;
	
	// Use this for initialization
	void Start () {
		if(type == "gold") {
			int i = Random.Range(1, 100);
			if(i >= 0) {
				amount = 50;
				gameObject.transform.GetChild(0).renderer.material.mainTexture = textures[0];
			}
			if(i >= 30) {
				amount = 100;
				gameObject.transform.GetChild(0).renderer.material.mainTexture = textures[1];
			}
			if(i >= 70) {
				amount = 500;
				gameObject.transform.GetChild(0).renderer.material.mainTexture = textures[2];
			}
			if(i >= 90) {
				amount = 1000;
				gameObject.transform.GetChild(0).renderer.material.mainTexture = textures[3];
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		LookAtPlayer();
	}
}
