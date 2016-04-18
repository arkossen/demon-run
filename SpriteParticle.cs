using UnityEngine;
using System.Collections;

public class SpriteParticle : LookAtSprite {
	
	SpriteManager spriteManager;
	Sprite particleSprite;
	
	public Vector3 startPos; 
	
	// Use this for initialization
	void Start () {
		spriteManager = gameObject.GetComponent<SpriteManager>() as SpriteManager;
		
        particleSprite = spriteManager.AddSprite(gameObject, 1.0f, 1.0f, spriteManager.PixelSpaceToUVSpace(128,128), spriteManager.PixelSpaceToUVSpace(30, 30), false);
		particleSprite.SetAnimCompleteDelegate(OnAnimateComplete);
		
        Vector2 spriteSize = spriteManager.PixelSpaceToUVSpace(30, 30);
		
        //Idle animation
        UVAnimation idleAnimation = new UVAnimation();
        Vector2 idleStartPosUV = spriteManager.PixelCoordToUVCoord(0, 30);

        idleAnimation.BuildUVAnim(idleStartPosUV, spriteSize, 3, 2, 5, 15);
		idleAnimation.loopCycles = 0;
        idleAnimation.name = "idle";
        particleSprite.AddAnimation(idleAnimation);
		particleSprite.PlayAnim("idle");
		transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
	}
	
	public void OnAnimateComplete(string name) {
		Destroy(gameObject);
	}
	
	void Update() {
		LookAtPlayer();
	}
}
