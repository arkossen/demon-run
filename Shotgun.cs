using UnityEngine;
using System.Collections;

public class Shotgun : Weapon {

	// Use this for initialization
	void Start () {
		spriteManager = gameObject.GetComponent<SpriteManager>() as SpriteManager;
		
        weaponSprite = spriteManager.AddSprite(gameObject, 1.0f, 1.0f, spriteManager.PixelSpaceToUVSpace(512,512), spriteManager.PixelSpaceToUVSpace(72, 78), false);
		weaponSprite.SetAnimCompleteDelegate(OnAnimateComplete);
		
        Vector2 spriteSize = spriteManager.PixelSpaceToUVSpace(72, 78);
		
        //Idle animation
        UVAnimation idleAnimation = new UVAnimation();
        Vector2 idleStartPosUV = spriteManager.PixelCoordToUVCoord(0, 78);

        idleAnimation.BuildUVAnim(idleStartPosUV, spriteSize, 1, 1, 1, 15);
		idleAnimation.loopCycles = -1;
        idleAnimation.name = "idle";
        weaponSprite.AddAnimation(idleAnimation);
		
        UVAnimation shootAnimation = new UVAnimation();
        Vector2 shootStartPosUV = spriteManager.PixelCoordToUVCoord(0, 78);

        shootAnimation.BuildUVAnim(shootStartPosUV, spriteSize, 3, 2, 5, 15);
		shootAnimation.loopCycles = 0;
        shootAnimation.name = "shoot";
        weaponSprite.AddAnimation(shootAnimation);
		
		transform.parent = GameObject.Find("Main Camera").transform;
		
		transform.localPosition = localPos;
		transform.rotation = GameObject.Find("Main Camera").transform.rotation;
	}
	
}
