using UnityEngine;
using System.Collections;
using Pathfinding;

public class Enemy : LookAtSprite {
	
	
	//Enemy Base Stats
	public float baseDamage;
	public float baseHealth;
	public float baseAccuracy;
	public float baseRange;
	public float baseSightRange;
	
	public float modDamage;
	public float modHealth;
	public float modAccuracy;
	public float modRange;
	public float modSightRange;
	
	public float maxDamage;
	public float maxHealth;
	public float maxAccuracy;
	public float maxRange;
	public float maxSightRange;
	
	public int damage = 5;
	public int health;
	public int accuracy = 25; // max = 100
	public int range = 15;
	public int sightRange = 30;
	
	public Vector3 baseBeam;
	
	public float attackSpeed = 200;
	public float attackSpeedMax = 200;
	
	public int spawnChance;
	public int overallDropChance;
	public GameObject[] drops;
	public int[] dropsChance;
	
	// Enemy AI
	public bool awareOfPlayer = false;
	public bool inRange = false;
	
	AudioSource audioSource;
	public AudioClip sndShoot;
	
	string curState = "IDLE"; // IDLE, AWARE, SHOOT, CHASE
	
	float searchTime = 0;
	float maxsearchTime = 200;
	bool searching = false;
	
	// Seeker variables
	public Transform target;
	Seeker agent;
	CharacterController controller;
    public Path path;
    public float speed = 100;
    public float nextWaypointDistance = 3;
    private int currentWaypoint = 0;
	float newPathTime = 50;
	float maxNewPathTime = 50;
	string pathSet = "";
	
	SpriteManager spriteManager;
	Sprite enemySprite;
	public Vector3 startPos;
	
	GameObject enemy;
	
	//Animation variables
	
	public Vector2 InGameSize;
	public Vector2 TextureSize;
	public Vector2 FrameSize;
	public int fps;
	
	public Vector2 idleStartPos;
	public Vector2 idleSpriteSheetPos;
	public int idleAnimLength;
	
	public Vector2 walkStartPos;
	public Vector2 walkSpriteSheetPos;
	public int walkAnimLength;
	
	public Vector2 shootStartPos;
	public Vector2 shootSpriteSheetPos;
	public int shootAnimLength;
	
	public Vector2 dieStartPos;
	public Vector2 dieSpriteSheetPos;
	public int dieAnimLength;
	
	public int shootFrame;
	bool shot = true;
	
	// Use this for initialization
	void Start () {
		agent = gameObject.GetComponent<Seeker>();
		controller = gameObject.GetComponent<CharacterController>();
		audioSource = gameObject.GetComponent<AudioSource>();
		spriteManager = gameObject.GetComponent<SpriteManager>() as SpriteManager;
		
		createSprite();
	}
	
	public void OnAnimateComplete(string name) {
		switch(name) {
		case "die" : {
			//Destroy(gameObject);
			break;
		}
		}
	}
	
	private void createSprite() {
        enemySprite = spriteManager.AddSprite(gameObject, InGameSize.x, InGameSize.y, spriteManager.PixelSpaceToUVSpace((int)TextureSize.x,(int)TextureSize.y), spriteManager.PixelSpaceToUVSpace((int)FrameSize.x, (int)FrameSize.y), false);
		enemySprite.SetAnimCompleteDelegate(OnAnimateComplete);
		
        Vector2 spriteSize = spriteManager.PixelSpaceToUVSpace((int)FrameSize.x, (int)FrameSize.y);
		
        //Idle animation
        UVAnimation idleAnimation = new UVAnimation();
        Vector2 idleStartPosUV = spriteManager.PixelCoordToUVCoord((int)idleStartPos.x, (int)idleStartPos.y);

        idleAnimation.BuildUVAnim(idleStartPosUV, spriteSize, (int)idleSpriteSheetPos.x, (int)idleSpriteSheetPos.y, (int)idleAnimLength, fps);
		idleAnimation.loopCycles = -1;
        idleAnimation.name = "idle";
        enemySprite.AddAnimation(idleAnimation);
		
        //Walk animation
        UVAnimation walkAnimation = new UVAnimation();
        Vector2 walkStartPosUV = spriteManager.PixelCoordToUVCoord((int)walkStartPos.x, (int)walkStartPos.y);
		
        walkAnimation.BuildUVAnim(walkStartPosUV, spriteSize, (int)walkSpriteSheetPos.x, (int)walkSpriteSheetPos.y, (int)walkAnimLength, fps);
		walkAnimation.loopCycles = -1;
        walkAnimation.name = "walk";
        enemySprite.AddAnimation(walkAnimation);
		
        //Shoot animation
        UVAnimation shootAnimation = new UVAnimation();
        Vector2 shootStartPosUV = spriteManager.PixelCoordToUVCoord((int)shootStartPos.x, (int)shootStartPos.y);
		
        shootAnimation.BuildUVAnim(shootStartPosUV, spriteSize, (int)shootSpriteSheetPos.x, (int)shootSpriteSheetPos.y, (int)shootAnimLength, fps);
		shootAnimation.loopCycles = 0;
        shootAnimation.name = "shoot";
        enemySprite.AddAnimation(shootAnimation);
		
        //Die animation
        UVAnimation dieAnimation = new UVAnimation();
        Vector2 dieStartPosUV = spriteManager.PixelCoordToUVCoord((int)dieStartPos.x, (int)dieStartPos.y);
		
        dieAnimation.BuildUVAnim(dieStartPosUV, spriteSize, (int)dieSpriteSheetPos.x, (int)dieSpriteSheetPos.y, (int)dieAnimLength, fps);
		dieAnimation.loopCycles = 0;
        dieAnimation.name = "die";
        enemySprite.AddAnimation(dieAnimation);
		
		
		gameObject.transform.position = startPos;
	}
	
	// Update is called once per frame
	void Update () {
//		
//        if (Input.GetKeyDown("1")) {
//			enemySprite.PlayAnim("idle");
//		}
//        if (Input.GetKeyDown("2")) {
//			enemySprite.PlayAnim("walk");
//		}
//        if (Input.GetKeyDown("3")) {
//			enemySprite.PlayAnim("die");
//		}
//		enemySprite.PlayAnim("idle");
//		return;
		
		
		LookAtPlayer();
		
		if(health <= 0) {
			if(enemySprite.isPlaying != "die") {
				Die();
			}
			return;
		}
		
		if(enemySprite.isPlaying == "shoot") {
			if(enemySprite.framesPlayed >= shootFrame) {
				
				RaycastHit hit;
				Vector3 pos = new Vector3(transform.position.x + baseBeam.x, transform.position.y + baseBeam.y, transform.position.z + baseBeam.z);
				Vector3 dir = transform.forward;
				
				if(Physics.Raycast(pos, dir, out hit, range)) {
					Debug.DrawLine(pos, hit.point);
					if(hit.transform.gameObject.tag == "player" && !shot) {
						shot = true;
						int roll = Random.Range(0, 100);
						if(roll < accuracy) {
							hit.transform.gameObject.GetComponent<Player>().gotHit(damage); // HITS PLAYER!!
						}
						if(sndShoot) {
							audioSource.clip = sndShoot;
							audioSource.Play();
						}
					}
				}
			}
		}
		
		if(health > 0) {
			
			RaycastHit hit;
			Vector3 pos = new Vector3(transform.position.x + baseBeam.x, transform.position.y + baseBeam.y, transform.position.z + baseBeam.z);
			Vector3 dir = transform.forward;
			
			if(!awareOfPlayer) {
				if(Physics.Raycast (pos, dir, out hit, sightRange)) {
					Debug.DrawLine(pos, hit.point);
					if(hit.transform.gameObject.tag == "player") {
						awareOfPlayer = true;
					}
				}
				enemySprite.PlayAnim("idle");
			}
			
			// Continue below here
			if(awareOfPlayer) {
				bool canShoot = false;
				if(attackSpeed > 0) {
					attackSpeed -= Time.deltaTime * 100;
				}
				
				if(Physics.Raycast(pos, dir, out hit, range)) {
					Debug.DrawLine(pos, hit.point);
					if(hit.transform.gameObject.tag == "player") {
						canShoot = true;
						if(attackSpeed <= 0 && shot) { // SHOOT!
							enemySprite.PlayAnim("shoot");
							attackSpeed = attackSpeedMax;
							shot = false;
						}
					}
				}
				
				if(!canShoot) {
					lookForPath();
				}
			
	//			if(newPathTime >= 0) {
	//	       		newPathTime -= Time.deltaTime * 100;
	//			}else {
	//				lookForPath();
	//			}
			}
		}
	}
	
	public void Die() {
		int totalDrops = Random.Range(0, 100);
		enemySprite.PlayAnim("die");
		
		Destroy(gameObject.GetComponent<CharacterMotor>());
		Destroy(controller);
		if(totalDrops <= overallDropChance) {
			int maxDrop = 0;
			
			foreach(int num in dropsChance) {
				maxDrop += num;
			}
			
			int chance = Random.Range(0, maxDrop);
			maxDrop = 0;
			int i = 0;
			foreach(int num in dropsChance) {
				maxDrop += num;
				if(maxDrop >= chance) {
					GameObject drop = Instantiate(drops[i], new Vector3(transform.position.x + (Random.Range(0, 3)), -0.5f, transform.position.z + Random.Range(0, 3)), drops[i].transform.rotation) as GameObject;
					drop.transform.parent = GameObject.Find("LevelGenerator").transform;
					break;
				}
				i++;
			}
		}
	}
	
	public void lookForPath() {
		
		if(searching) {
       		searchTime -= Time.deltaTime * 100;
			if(searchTime < 0) {
				searching = false;
			}
			return;
		}
		
		if (path == null) {
            //We have no path to move after yet
            //Debug.Log ("No Path");
			
			if(controller.isGrounded) {
           		Debug.Log ("Grounded");
				target = GameObject.Find("player").transform;
				Vector3 startPos = transform.position;
				agent.StartPath(startPos, target.position, OnPathComplete);
			searching = true;
			searchTime = maxsearchTime;
			}
            return;
        }
        
        if (currentWaypoint >= path.vectorPath.Length && path.IsDone()) {
            //Debug.Log ("End Of Path Reached");
			
			Vector3 startPos = transform.position;
			target = GameObject.Find("player").transform;
			agent.StartPath(startPos, target.position, OnPathComplete);
			searching = true;
			searchTime = maxsearchTime;
            return;
        }
		
        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
        dir *= speed * Time.deltaTime;
        controller.SimpleMove (dir);
		if(enemySprite.isPlaying != "walk") {
			enemySprite.PlayAnim("walk");
		}
        
        if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            currentWaypoint++;
            return;
        }
		
//		if(path != null) {
//			if(path.vectorPath[currentWaypoint] != target.position) {
//				if(newPathTime < 0) {
//		            currentWaypoint++;
//					newPathTime = maxNewPathTime;
//		            return;
//				}
//			}
//		}
	}
	
	public void OnPathComplete (Path p) {
	    //Debug.Log ("Yey, we got a path back. Did it have an error? " + p.error);
		
        if (!p.error) {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
	}
}
