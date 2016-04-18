using UnityEngine;
using System.Collections;

public class Crowbar : Weapon {
	
	public float checkColTime = 0;
	public float checkColMaxTime = 1000;
	public bool canHit = true;

	// Use this for initialization
	void Start () {
		spriteManager = gameObject.GetComponent<SpriteManager>() as SpriteManager;
		
        weaponSprite = spriteManager.AddSprite(gameObject, 1.0f, 1.0f, spriteManager.PixelSpaceToUVSpace(1024,1024), spriteManager.PixelSpaceToUVSpace(162, 135), false);
		weaponSprite.SetAnimCompleteDelegate(OnAnimateComplete);
		
        Vector2 spriteSize = spriteManager.PixelSpaceToUVSpace(162, 135);
		
        //Idle animation
        UVAnimation idleAnimation = new UVAnimation();
        Vector2 idleStartPosUV = spriteManager.PixelCoordToUVCoord(0, 135);

        idleAnimation.BuildUVAnim(idleStartPosUV, spriteSize, 1, 1, 1, 15);
		idleAnimation.loopCycles = -1;
        idleAnimation.name = "idle";
        weaponSprite.AddAnimation(idleAnimation);
		
        UVAnimation shootAnimation = new UVAnimation();
        Vector2 shootStartPosUV = spriteManager.PixelCoordToUVCoord(0, 135);

        shootAnimation.BuildUVAnim(shootStartPosUV, spriteSize, 4, 4, 13, 15);
		shootAnimation.loopCycles = 0;
        shootAnimation.name = "shoot";
        weaponSprite.AddAnimation(shootAnimation);
		
		transform.parent = GameObject.Find("Main Camera").transform;
		
		transform.localPosition = localPos;
		transform.rotation = GameObject.Find("Main Camera").transform.rotation;
	}
}
