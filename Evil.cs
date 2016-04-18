using UnityEngine;
using System.Collections;
using Pathfinding;

public class Evil : LookAtSprite {
	
	public Transform target;
	Seeker agent;
	CharacterController controller;
	
    //The calculated path
    public Path path;
	
	//The AI's speed per second
    public float speed = 100;
	
	public float maxSpeed = 150;
	public float maxLookSpeed = 1000;
    
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
 
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;
	
	
	float newPathTime = 100;
	float maxNewPathTime = 200;
	float searchTime = 0;
	float maxsearchTime = 500;
	
	SpriteManager spriteManager;
	Sprite evilSprite;
	
	public Vector3 startPos;
	
	bool searching = false;
	
	// Use this for initialization
	void Start () {
		agent = gameObject.GetComponent<Seeker>();
		controller = gameObject.GetComponent<CharacterController>();
		
		
		spriteManager = gameObject.GetComponent<SpriteManager>() as SpriteManager;
		
        evilSprite = spriteManager.AddSprite(gameObject, 2.0f, 2.0f, spriteManager.PixelSpaceToUVSpace(256,256), spriteManager.PixelSpaceToUVSpace(50, 50), false);
		evilSprite.SetAnimCompleteDelegate(OnAnimateComplete);
		
        Vector2 spriteSize = spriteManager.PixelSpaceToUVSpace(50, 50);
		
        //Idle animation
        UVAnimation idleAnimation = new UVAnimation();
        Vector2 idleStartPosUV = spriteManager.PixelCoordToUVCoord(0, 50);

        idleAnimation.BuildUVAnim(idleStartPosUV, spriteSize, 3, 2, 6, 15);
		idleAnimation.loopCycles = -1;
        idleAnimation.name = "idle";
        evilSprite.AddAnimation(idleAnimation);
		
		evilSprite.PlayAnim("idle");
		gameObject.transform.position = startPos;
	}
	
	public void OnAnimateComplete(string name) {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		LookAtPlayer();
		
		GameObject player = GameObject.Find("player");
		GUITexture close = GameObject.Find("EvilClose").GetComponent<GUITexture>();
		float dist = Vector3.Distance(player.transform.position, transform.position);
		if(dist <= 3) {
			player.GetComponent<Player>().gotHit(10);
		}
        if(dist <= 30) {
			dist -= 10;
			dist =  1 - (dist / 20);
			Color col = close.color;
			col.a = dist;
			close.color = col;
		}else {
			Color col = close.color;
			col.a = 0;
			close.color = col;
		}
		
		if(newPathTime >= 0) {
       		newPathTime -= Time.deltaTime * 100;
		}
		
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
			target = GameObject.Find("player").transform;
			Vector3 startPos = transform.position;
			agent.StartPath(startPos, target.position, OnPathComplete);
			searching = true;
			searchTime = maxsearchTime;
            return;
        }
        
        if (currentWaypoint >= path.vectorPath.Length) {
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
        
        if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            currentWaypoint++;
            return;
        }
		if(path.vectorPath[currentWaypoint] != target.position) {
			if(newPathTime < 0) {
				Debug.Log("BOEM: " + path.vectorPath.Length.ToString());
	            currentWaypoint = path.vectorPath.Length;
				newPathTime = maxNewPathTime;
	            return;
			}
		}
		
	}
	
	public void ignoreEnemyCollisions() {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
	    foreach(GameObject E in enemies)
	    {
	        Physics.IgnoreCollision(E.GetComponent<Collider>(), collider);
	    }
	}
	
	public void OnPathComplete (Path p) {
	    //Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		
        if (!p.error) {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
			searching = false;
        }
	}
}
