using UnityEngine;
using System.Collections;

public class Pistol : Weapon {

	// Use this for initialization
	void Start () {
		spriteManager = gameObject.GetComponent<SpriteManager>() as SpriteManager;
		
        weaponSprite = spriteManager.AddSprite(gameObject, 1.0f, 1.0f, spriteManager.PixelSpaceToUVSpace(256,256), spriteManager.PixelSpaceToUVSpace(60, 60), false);
		weaponSprite.SetAnimCompleteDelegate(OnAnimateComplete);
		
        Vector2 spriteSize = spriteManager.PixelSpaceToUVSpace(60, 60);
		
        //Idle animation
        UVAnimation idleAnimation = new UVAnimation();
        Vector2 idleStartPosUV = spriteManager.PixelCoordToUVCoord(0, 60);

        idleAnimation.BuildUVAnim(idleStartPosUV, spriteSize, 1, 1, 1, 15);
		idleAnimation.loopCycles = -1;
        idleAnimation.name = "idle";
        weaponSprite.AddAnimation(idleAnimation);
		
        UVAnimation shootAnimation = new UVAnimation();
        Vector2 shootStartPosUV = spriteManager.PixelCoordToUVCoord(0, 60);

        shootAnimation.BuildUVAnim(shootStartPosUV, spriteSize, 3, 2, 5, 15);
		shootAnimation.loopCycles = 0;
        shootAnimation.name = "shoot";
        weaponSprite.AddAnimation(shootAnimation);
		
		transform.parent = GameObject.Find("Main Camera").transform;
		
		transform.localPosition = localPos;
		transform.rotation = GameObject.Find("Main Camera").transform.rotation;
	}
	
}
