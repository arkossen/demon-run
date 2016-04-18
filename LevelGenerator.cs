using UnityEngine;
using System.Collections;
using Pathfinding;
using System.IO;

public class LevelGenerator : MonoBehaviour {
	
	public int emptyRoom = 1;
	
	public Vector2 levelSize = new Vector2(20, 20);
	public int[,] map = new int[20, 20];
	int doors;
	bool shop = false;
	
	public int level = 0;
	
	public GameObject startPlane;
	public GameObject exitPlane;
	public GameObject shopPrefab;
	
	public GameObject[] floors;
	public Texture[] floorTextures;
	public GameObject[] ceilings;
	public Texture[] ceilingTextures;
	public GameObject[] walls;
	public Texture[] wallTextures;
	public GameObject[] destructableWalls;
	public Texture[] destructableWallTextures;
	public Texture[] destructedWallTextures;
	
	public Texture[] eventFloorTexture;
	public Texture[] eventCeilingTextures;
	public Texture[] eventWallTextures;
	public Texture[] eventDestructableWallTextures;
	public Texture[] eventDestructedWallTextures;
	
	public GameObject[] spawnablePickups;
	
	public GameObject[] enemies;
	public GameObject evil;
	
	public int totalEnemies;
	public int totalEnemiesMod;
	
	public LootTable[] lootTables;
	
	public TilesetLoader tilesets;
	public Tileset curTileset;
	
	Room[] rooms = new Room[0];
	
	
	// Level parts consist of:
	// 0 is Nothing
	// 1 is Hall > everything above 999 is a hall
	// 2 is Room
	// 3 is DoorStart
	// 4 is Start
	// 5 is Exit
	// 6 is DoorFinish
	// 7 is StartRoom (and Hall)
	// 20-98 is Secret Room
	// 99 is Secret Room Entrance (Destructable wall)
	// 1000 > is HALL
	
	
	// Use this for initialization
	void Start () {
		//tilesets = new TilesetLoader();
		
		getRooms();
		createLevel();
		
		GameObject.Find("TussenSchermHUB").GetComponent<ScoreScreen>().hideScoreScreen();
        Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void createLevel() {
		doors = 0; // Reset doors to 0
		levelSize = new Vector2(levelSize.x + level, levelSize.y + level);
		map = new int[(int)levelSize.x, (int)levelSize.y];
		shop = false;
		level++;
		
		int rooms = Random.Range(4 + (int)Mathf.Floor(level / 6),7 + (int)Mathf.Floor((float)level / 3)); //TODO Make global vars for these values
		int eventRooms = Random.Range(0, 2 + (int)Mathf.Floor(level / 5)); //TODO Make global vars for these values
		
		
		//TODO Improve placement of these so that they are further apart from eachother
		Vector2 startPos = new Vector2(Mathf.Floor(Random.Range(1, levelSize.x - 2)), Mathf.Floor(Random.Range(1, levelSize.y - 2)));
		Vector2 exitPos = new Vector2(Mathf.Floor(Random.Range(1, levelSize.x - 2)), Mathf.Floor(Random.Range(1, levelSize.y - 2)));
		
		createRoom(startPos, "start");
		createRoom(exitPos, "exit");
		
		
		GameObject player = GameObject.Find("player");
		player.transform.position = new Vector3((startPos.x * 5), 4, (startPos.y * 5));
		for(int i = 0; i < rooms; i++) {
			createRoom(new Vector2(Mathf.Floor(Random.Range(1, levelSize.x - 2)), Mathf.Floor(Random.Range(1, levelSize.y - 2))), "room");
		}
		
		while(doors > 0) {
			int num = 1000;
			for(int x = 0; x < levelSize.x; x++) {
				for(int y = 0; y < levelSize.y; y++) {
					num++;
					if(map[x, y] == 3) {
						if(!whereHall(new Vector2(x, y), num)) {
							//Debug.Log("x: " + x + ", y: " + y);
							map[x, y] = 6;
							doors--;
						}
					}
				}
			}
		}
		
		for(int a = 0; a < levelSize.x; a++) {
			for(int b = 0; b < levelSize.y; b++) {
				if(a == 0 || a == levelSize.x - 1 || b == 0 || b == levelSize.y - 1) {
					if(map[a, b] >= 1000) {
						removeExcessHall(new Vector2(a, b));
					}
				}
			}
		}
	
		RoomTile[] halls = new RoomTile[0];
		
		for(int x = 0; x < levelSize.x; x++) {
			for(int y = 0; y < levelSize.y; y++) {
				if(map[x, y] >= 1000) {
					RoomTile[] curHalls = new RoomTile[halls.Length + 1];
					
					for(var h = 0; h < halls.Length; h++) {
						curHalls[h] = halls[h];
					}
					
					curHalls[curHalls.Length - 1] = new RoomTile(new Vector2(x, y), 1000);
					halls = curHalls;
				}
			}
		}
		
		
		
		// add secret rooms here
		for(int i = 0; i < eventRooms; i++) {
			createSecretRoom(halls);
		}
		
		for(int x = 0; x < levelSize.x; x++) {
			for(int y = 0; y < levelSize.y; y++) {
				if(map[x, y] != 0) {
					GameObject plane = createFloor();
					if(map[x, y] == 4) { // Start Level
						Destroy(plane);
						plane = Instantiate(startPlane, transform.position, transform.rotation) as GameObject;
					}else if(map[x, y] == 5) { // Exit Level
						Destroy(plane);
						plane = Instantiate(exitPlane, transform.position, transform.rotation) as GameObject;
					}
					Vector3 pos = new Vector3(x*5, 0, y*5);
					plane.transform.position = pos;
					plane.layer = 8;
					plane.name = "ground";
					plane.transform.parent = GameObject.Find("LevelGenerator").transform;
					plane.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
					plane.name = "x" + x + "y" + y + "t" + map[x, y];
					
					GameObject ceiling = createCeiling();
					pos = new Vector3(x*5, 5, y*5);
					ceiling.transform.position = pos;
					Quaternion rotation = Quaternion.Euler(0, 0, 180);
					ceiling.transform.rotation = rotation;
					ceiling.transform.parent = GameObject.Find("LevelGenerator").transform;
					ceiling.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
					placeWallsAround(new Vector2(x, y));
					
					
					if(map[x, y] >= 40 && map[x, y] <= 99) {
						plane.renderer.material.mainTexture = eventFloorTexture[0];
						spawnPickup(plane.transform.position, map[x, y]);
					}
				}
//				else {
//					GameObject cube = Instantiate(walls[0], transform.position, transform.rotation) as GameObject;
//					Vector3 pos = new Vector3(x*10, 0, y*10);
//					Quaternion rotation = Quaternion.Euler(90, 0, 0);
//					cube.transform.position = pos;
//					cube.layer = 9;
//					
//					cube.transform.localScale = new Vector3(10, 10, 10);
//					cube.transform.parent = GameObject.Find("LevelGenerator").transform;
//				}
			}
		}
		spawnEnemies();
		if(GameObject.FindGameObjectWithTag("Evil")) {
			GameObject.FindGameObjectWithTag("Evil").GetComponent<Evil>().ignoreEnemyCollisions();
		}
		AstarPath.active.astarData.gridGraph.width = (int)levelSize.x * 5;
		AstarPath.active.astarData.gridGraph.depth = (int)levelSize.y * 5;
		AstarPath.active.astarData.gridGraph.center = new Vector3(AstarPath.active.astarData.gridGraph.width / 2, -0.1f, AstarPath.active.astarData.gridGraph.depth / 2);
		
		AstarPath.active.astarData.gridGraph.UpdateSizeFromWidthDepth();
		AstarPath.active.Scan();
	}
	
	public void destroyLevel() {
		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}
		GameObject shopGUI = GameObject.Find("ShopGUI") as GameObject;
		foreach (Transform shopChild in shopGUI.transform) {
			Destroy(shopChild.gameObject);
		}
	}
	
	GameObject createFloor() {
		int floorNum = Random.Range(0, floorTextures.Length);
		GameObject floor = Instantiate(floors[0], transform.position, transform.rotation) as GameObject;
		
		floor.renderer.material.mainTexture = floorTextures[floorNum];
		return floor;
	}
	
	GameObject createCeiling() {
		int ceilingNum = Random.Range(0, ceilingTextures.Length);
		GameObject ceiling = Instantiate(ceilings[0], transform.position, transform.rotation) as GameObject;
		
		ceiling.renderer.material.mainTexture = ceilingTextures[ceilingNum];
		return ceiling;
	}
	
	GameObject createWall(int type) {
		int wallNum = Random.Range(0, 100);
		if(wallNum >= 98) {
			wallNum = 4;
		}else if(wallNum >= 93) {
			wallNum = 3;
		}else {
			wallNum = Random.Range(0, 3);
		}
		
		GameObject wallPrefab = Instantiate(walls[0], transform.position, transform.rotation) as GameObject;
		wallPrefab.renderer.material.mainTexture = wallTextures[wallNum];
		
		if(!shop && Random.Range(0, 100) <= 1 && (type <= 40 || type >= 1000)) {
			Destroy(wallPrefab);
			wallPrefab = Instantiate(shopPrefab, transform.position, transform.rotation) as GameObject;
			shop = true;
		}
		
		return wallPrefab;
	}
	
	GameObject createDestructableWall() {
		GameObject wallPrefab = Instantiate(destructableWalls[0], transform.position, transform.rotation) as GameObject;
		
		return wallPrefab;
	}
	
	// Place walls around a Vector2
	void placeWallsAround(Vector2 pos) {
		float height = 2.5f;
		float extraDist = 2.5f;
		if(pos.x - 1 >= 0) {
			if(map[(int)pos.x - 1, (int)pos.y] == 0) {
				GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
				Vector3 wallPos = new Vector3((pos.x * 5) - extraDist, height, (pos.y * 5));
				Quaternion rotation = Quaternion.Euler(90, 90, 0);
				wall.transform.position = wallPos;
				wall.transform.rotation = rotation;
				wall.layer = 9;
				wall.transform.parent = GameObject.Find("LevelGenerator").transform;
				
				if(map[(int)pos.x, (int)pos.y] == 40 || map[(int)pos.x, (int)pos.y] == 99) {
					wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
				}
			}
		} else if(pos.x - 1 < 0) {
			GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
			Vector3 wallPos = new Vector3((pos.x * 5) - extraDist, height, (pos.y * 5));
			Quaternion rotation = Quaternion.Euler(90, 90, 0);
			wall.transform.position = wallPos;
			wall.transform.rotation = rotation;
			wall.layer = 9;
			wall.transform.parent = GameObject.Find("LevelGenerator").transform;
			
			if(map[(int)pos.x, (int)pos.y] == 40 || map[(int)pos.x, (int)pos.y] == 99) {
				wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
			}
		}
		
		if(pos.x < levelSize.x - 1) { 
			if(map[(int)pos.x + 1, (int)pos.y] == 0) {
				GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
				Vector3 wallPos = new Vector3((pos.x * 5) + extraDist, height, (pos.y * 5));
				Quaternion rotation = Quaternion.Euler(90, 270, 0);
				wall.transform.position = wallPos;
				wall.transform.rotation = rotation;
				wall.layer = 9;
				wall.transform.parent = GameObject.Find("LevelGenerator").transform;
				
				if(map[(int)pos.x, (int)pos.y] == 40 || map[(int)pos.x, (int)pos.y] == 99) {
					wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
				}
			}
		} else if(pos.x >= levelSize.x - 1) {
			GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
			Vector3 wallPos = new Vector3((pos.x * 5) + extraDist, height, (pos.y * 5));
			Quaternion rotation = Quaternion.Euler(90, 270, 0);
			wall.transform.position = wallPos;
			wall.transform.rotation = rotation;
			wall.layer = 9;
			wall.transform.parent = GameObject.Find("LevelGenerator").transform;
			
			if(map[(int)pos.x, (int)pos.y] == 40 || map[(int)pos.x, (int)pos.y] == 99) {
				wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
			}
		}
		
		if(pos.y - 1 >= 0) { 
			if(map[(int)pos.x, (int)pos.y - 1] == 0) {
				GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
				Vector3 wallPos = new Vector3((pos.x * 5), height, (pos.y * 5) - extraDist);
				Quaternion rotation = Quaternion.Euler(90, 0, 0);
				wall.transform.position = wallPos;
				wall.transform.rotation = rotation;
				wall.layer = 9;
				wall.transform.parent = GameObject.Find("LevelGenerator").transform;
				
				if(map[(int)pos.x, (int)pos.y] == 40 || map[(int)pos.x, (int)pos.y] == 99) {
					wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
				}
			}
		} else if(pos.y - 1 < 0) {
			GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
			Vector3 wallPos = new Vector3((pos.x * 5), height, (pos.y * 5) - extraDist);
			Quaternion rotation = Quaternion.Euler(90, 0, 0);
			wall.transform.position = wallPos;
			wall.transform.rotation = rotation;
			wall.layer = 9;
			wall.transform.parent = GameObject.Find("LevelGenerator").transform;
			
			if(map[(int)pos.x, (int)pos.y] == 40 || map[(int)pos.x, (int)pos.y] == 99) {
				wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
			}
		}
		
		if(pos.y < levelSize.y - 1) { 
			if(map[(int)pos.x, (int)pos.y + 1] == 0) {
				GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
				Vector3 wallPos = new Vector3((pos.x * 5), height, (pos.y * 5) + extraDist);
				Quaternion rotation = Quaternion.Euler(90, 180, 0);
				wall.transform.position = wallPos;
				wall.transform.rotation = rotation;
				wall.layer = 9;
				wall.transform.parent = GameObject.Find("LevelGenerator").transform;
				
				if(map[(int)pos.x, (int)pos.y] == 40 || map[(int)pos.x, (int)pos.y] == 99) {
					wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
				}
			}
		} else if(pos.y >= levelSize.y - 1) {
			GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
			Vector3 wallPos = new Vector3((pos.x * 5), height, (pos.y * 5) + extraDist);
			Quaternion rotation = Quaternion.Euler(90, 180, 0);
			wall.transform.position = wallPos;
			wall.transform.rotation = rotation;
			wall.layer = 9;
			wall.transform.parent = GameObject.Find("LevelGenerator").transform;
			
			if(map[(int)pos.x, (int)pos.y] == 40 || map[(int)pos.x, (int)pos.y] == 99) {
				wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
			}
		}
		
		if(map[(int)pos.x, (int)pos.y] == 40) {
			if(!posOutOfBounds(new Vector2(pos.x - 1, pos.y))) {
				if(map[(int)pos.x - 1, (int)pos.y] >= 1000 || (map[(int)pos.x - 1, (int)pos.y] <= 19 && map[(int)pos.x - 1, (int)pos.y] > 0)) {
					// Inside Wall
					GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
					wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
					Vector3 wallPos = new Vector3((pos.x * 5) - extraDist, height, (pos.y * 5));
					Quaternion rotation = Quaternion.Euler(90, 90, 0);
					wall.transform.position = wallPos;
					wall.transform.rotation = rotation;
					wall.layer = 9;
					wall.transform.parent = GameObject.Find("LevelGenerator").transform;
					
					// Outside Wall
					GameObject outsideWall = createWall(map[(int)pos.x, (int)pos.y]);
					Vector3 outsideWallPos = new Vector3((pos.x * 5) - extraDist, height, (pos.y * 5));
					Quaternion outsideRotation = Quaternion.Euler(90, 270, 0);
					outsideWall.transform.position = outsideWallPos;
					outsideWall.transform.rotation = outsideRotation;
					outsideWall.layer = 9;
					outsideWall.transform.parent = GameObject.Find("LevelGenerator").transform;
				}
			}
			if(!posOutOfBounds(new Vector2(pos.x + 1, pos.y))) {
				if(map[(int)pos.x + 1, (int)pos.y] >= 1000 || (map[(int)pos.x + 1, (int)pos.y] <= 19 && map[(int)pos.x + 1, (int)pos.y] > 0)) {
					// Inside Wall
					GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
					wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
					Vector3 wallPos = new Vector3((pos.x * 5) + extraDist, height, (pos.y * 5));
					Quaternion rotation = Quaternion.Euler(90, 270, 0);
					wall.transform.position = wallPos;
					wall.transform.rotation = rotation;
					wall.layer = 9;
					wall.transform.parent = GameObject.Find("LevelGenerator").transform;
					
					// Outside Wall
					GameObject outsideWall = createWall(map[(int)pos.x, (int)pos.y]);
					Vector3 outsideWallPos = new Vector3((pos.x * 5) + extraDist, height, (pos.y * 5));
					Quaternion outsideRotation = Quaternion.Euler(90, 90, 0);
					outsideWall.transform.position = outsideWallPos;
					outsideWall.transform.rotation = outsideRotation;
					outsideWall.layer = 9;
					outsideWall.transform.parent = GameObject.Find("LevelGenerator").transform;
				}
			}
			if(!posOutOfBounds(new Vector2(pos.x, pos.y - 1))) {
				if(map[(int)pos.x, (int)pos.y - 1] >= 1000 || (map[(int)pos.x, (int)pos.y - 1] <= 19 && map[(int)pos.x, (int)pos.y - 1] > 0)) {
					// Inside Wall
					GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
					wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
					Vector3 wallPos = new Vector3((pos.x * 5), height, (pos.y * 5) - extraDist);
					Quaternion rotation = Quaternion.Euler(90, 0, 0);
					wall.transform.position = wallPos;
					wall.transform.rotation = rotation;
					wall.layer = 9;
					wall.transform.parent = GameObject.Find("LevelGenerator").transform;
					
					// Outside Wall
					GameObject outsideWall = createWall(map[(int)pos.x, (int)pos.y]);
					Vector3 outsideWallPos = new Vector3((pos.x * 5), height, (pos.y * 5) - extraDist);
					Quaternion outsideRotation = Quaternion.Euler(90, 180, 0);
					outsideWall.transform.position = outsideWallPos;
					outsideWall.transform.rotation = outsideRotation;
					outsideWall.layer = 9;
					outsideWall.transform.parent = GameObject.Find("LevelGenerator").transform;
				}
			}
			if(!posOutOfBounds(new Vector2(pos.x, pos.y + 1))) {
				if(map[(int)pos.x, (int)pos.y + 1] >= 1000 || (map[(int)pos.x, (int)pos.y + 1] <= 19 && map[(int)pos.x, (int)pos.y + 1] > 0)) {
					// Inside Wall
					GameObject wall = createWall(map[(int)pos.x, (int)pos.y]);
					wall.renderer.material.mainTexture = eventWallTextures[Random.Range(0, eventWallTextures.Length)];
					Vector3 wallPos = new Vector3((pos.x * 5), height, (pos.y * 5) + extraDist);
					Quaternion rotation = Quaternion.Euler(90, 180, 0);
					wall.transform.position = wallPos;
					wall.transform.rotation = rotation;
					wall.layer = 9;
					wall.transform.parent = GameObject.Find("LevelGenerator").transform;
					
					// Outside Wall
					GameObject outsideWall = createWall(map[(int)pos.x, (int)pos.y]);
					Vector3 outsideWallPos = new Vector3((pos.x * 5), height, (pos.y * 5) + extraDist);
					Quaternion outsideRotation = Quaternion.Euler(90, 0, 0);
					outsideWall.transform.position = outsideWallPos;
					outsideWall.transform.rotation = outsideRotation;
					outsideWall.layer = 9;
					outsideWall.transform.parent = GameObject.Find("LevelGenerator").transform;
				}
			}
		}
		
		// MAKE DESTRUCTABLE WALLS!
		
		if(map[(int)pos.x, (int)pos.y] == 99) {
			if(!posOutOfBounds(new Vector2(pos.x - 1, pos.y))) {
				if(map[(int)pos.x - 1, (int)pos.y] >= 1000 || (map[(int)pos.x - 1, (int)pos.y] <= 19 && map[(int)pos.x - 1, (int)pos.y] > 0)) {
					GameObject destWall = createDestructableWall();
					destWall.GetComponent<Breakable>().insideDestructedTexture = eventDestructedWallTextures[0];
					destWall.GetComponent<Breakable>().outsideDestructedTexture = destructedWallTextures[0];
					
					// Inside Wall
					GameObject wall = destWall.transform.Find("insideWall").gameObject;
					wall.renderer.material.mainTexture = eventDestructableWallTextures[0];
					
					// Outside Wall
					GameObject outsideWall = destWall.transform.Find("outsideWall").gameObject;
					outsideWall.renderer.material.mainTexture = destructableWallTextures[0];
					
					Vector3 wallPos = new Vector3((pos.x * 5) - extraDist, height, (pos.y * 5));
					Quaternion rotation = Quaternion.Euler(90, 90, 0);
					destWall.transform.position = wallPos;
					destWall.transform.rotation = rotation;
					destWall.layer = 9;
					destWall.transform.parent = GameObject.Find("LevelGenerator").transform;
					
				}
			}
			if(!posOutOfBounds(new Vector2(pos.x + 1, pos.y))) {
				if(map[(int)pos.x + 1, (int)pos.y] >= 1000 || (map[(int)pos.x + 1, (int)pos.y] <= 19 && map[(int)pos.x + 1, (int)pos.y] > 0)) {
					GameObject destWall = createDestructableWall();
					destWall.GetComponent<Breakable>().insideDestructedTexture = eventDestructedWallTextures[0];
					destWall.GetComponent<Breakable>().outsideDestructedTexture = destructedWallTextures[0];
					
					// Inside Wall
					GameObject wall = destWall.transform.Find("insideWall").gameObject;
					wall.renderer.material.mainTexture = eventDestructableWallTextures[0];
					
					// Outside Wall
					GameObject outsideWall = destWall.transform.Find("outsideWall").gameObject;
					outsideWall.renderer.material.mainTexture = destructableWallTextures[0];
					
					Vector3 wallPos = new Vector3((pos.x * 5) + extraDist, height, (pos.y * 5));
					Quaternion rotation = Quaternion.Euler(90, 270, 0);
					destWall.transform.position = wallPos;
					destWall.transform.rotation = rotation;
					destWall.layer = 9;
					destWall.transform.parent = GameObject.Find("LevelGenerator").transform;
				}
			}
			if(!posOutOfBounds(new Vector2(pos.x, pos.y - 1))) {
				if(map[(int)pos.x, (int)pos.y - 1] >= 1000 || (map[(int)pos.x, (int)pos.y - 1] <= 19 && map[(int)pos.x, (int)pos.y - 1] > 0)) {
					GameObject destWall = createDestructableWall();
					destWall.GetComponent<Breakable>().insideDestructedTexture = eventDestructedWallTextures[0];
					destWall.GetComponent<Breakable>().outsideDestructedTexture = destructedWallTextures[0];
					
					// Inside Wall
					GameObject wall = destWall.transform.Find("insideWall").gameObject;
					wall.renderer.material.mainTexture = eventDestructableWallTextures[0];
					
					// Outside Wall
					GameObject outsideWall = destWall.transform.Find("outsideWall").gameObject;
					outsideWall.renderer.material.mainTexture = destructableWallTextures[0];
					
					Vector3 wallPos = new Vector3((pos.x * 5), height, (pos.y * 5) - extraDist);
					Quaternion rotation = Quaternion.Euler(90, 0, 0);
					destWall.transform.position = wallPos;
					destWall.transform.rotation = rotation;
					destWall.layer = 9;
					destWall.transform.parent = GameObject.Find("LevelGenerator").transform;
				}
			}
			if(!posOutOfBounds(new Vector2(pos.x, pos.y + 1))) {
				if(map[(int)pos.x, (int)pos.y + 1] >= 1000 || (map[(int)pos.x, (int)pos.y + 1] <= 19 && map[(int)pos.x, (int)pos.y + 1] > 0)) {
					GameObject destWall = createDestructableWall();
					destWall.GetComponent<Breakable>().insideDestructedTexture = eventDestructedWallTextures[0];
					destWall.GetComponent<Breakable>().outsideDestructedTexture = destructedWallTextures[0];
					
					// Inside Wall
					GameObject wall = destWall.transform.Find("insideWall").gameObject;
					wall.renderer.material.mainTexture = eventDestructableWallTextures[0];
					
					// Outside Wall
					GameObject outsideWall = destWall.transform.Find("outsideWall").gameObject;
					outsideWall.renderer.material.mainTexture = destructableWallTextures[0];
					
					Vector3 wallPos = new Vector3((pos.x * 5), height, (pos.y * 5) + extraDist);
					Quaternion rotation = Quaternion.Euler(90, 180, 0);
					destWall.transform.position = wallPos;
					destWall.transform.rotation = rotation;
					destWall.layer = 9;
					destWall.transform.parent = GameObject.Find("LevelGenerator").transform;
				}
			}
		}
		
	}
	
	void getRooms() {
	    TextAsset sr = Resources.Load("rooms/rooms") as TextAsset;
		Debug.Log(sr.text);
	    string[] lines = sr.text.Split(","[0]);
	    foreach(string line in lines) {
	        setRoom(line);
			Debug.Log(line);
	    }
	}
	
	void setRoom(string file) {
	    TextAsset sr = Resources.Load("rooms/" + file) as TextAsset;
	    string roomTiles = sr.text;
		
		Debug.Log(roomTiles);
		RoomTile[] positions = new RoomTile[roomTiles.Length / 3];
		
		for(int i = 0; i < roomTiles.Length; i+=3) {
			RoomTile tile = new RoomTile(new Vector2(int.Parse(roomTiles[i].ToString()), 
				int.Parse(roomTiles[i+1].ToString())), 
				int.Parse(roomTiles[i+2].ToString()));
			positions[i/3] = tile;
		}
		
		Room newRoom = new Room(positions);
		Room[] curRooms = new Room[rooms.Length + 1];
		
		for(int i = 0; i < rooms.Length; i++) {
			curRooms[i] = rooms[i];
		}
		
		curRooms[curRooms.Length - 1] = newRoom;
		//debugRoom(newRoom);
		rooms = curRooms;
	}
	
	void debugRoom(Room room) {
		Debug.Log("ROOMSTART");
		for(int i = 0; i < room.roomtiles.Length; i++) {
			RoomTile tile = room.roomtiles[i];
			Debug.Log("x: " + tile.pos.x + ", y: " + tile.pos.y + ", type; " + tile.type);
		}
		Debug.Log("ROOMEND");
	}
	
	void createRoom(Vector2 pos, string type) {
		
		int roomI = Random.Range(0, rooms.Length);
		
		Room curRoom = rooms[roomI];
		
		if(canRoom(pos, curRoom.roomtiles)) {
			int roomNum = 2;
			// Set middle part of the room corresponding to the type
			if(type == "start") {
				map[(int)pos.x, (int)pos.y] = 4;
				roomNum = 7;
				
				//Set Doors
				map[(int)(pos.x - 1), (int)pos.y] = canDoor(new Vector2(pos.x - 1, pos.y));
				map[(int)(pos.x + 1), (int)pos.y] = canDoor(new Vector2(pos.x + 1, pos.y));
				map[(int)pos.x, (int)(pos.y - 1)] = canDoor(new Vector2(pos.x, pos.y - 1));
				map[(int)pos.x, (int)(pos.y + 1)] = canDoor(new Vector2(pos.x, pos.y + 1));
				
				
				//Room parts
				map[(int)(pos.x - 1), (int)(pos.y - 1)] = roomNum;
				map[(int)(pos.x - 1), (int)(pos.y + 1)] = roomNum;
				map[(int)(pos.x + 1), (int)(pos.y - 1)] = roomNum;
				map[(int)(pos.x + 1), (int)(pos.y + 1)] = roomNum;
			}
			if(type == "exit") {
				map[(int)pos.x, (int)pos.y] = 5;
				
				//Set Doors
				map[(int)(pos.x - 1), (int)pos.y] = canDoor(new Vector2(pos.x - 1, pos.y));
				map[(int)(pos.x + 1), (int)pos.y] = canDoor(new Vector2(pos.x + 1, pos.y));
				map[(int)pos.x, (int)(pos.y - 1)] = canDoor(new Vector2(pos.x, pos.y - 1));
				map[(int)pos.x, (int)(pos.y + 1)] = canDoor(new Vector2(pos.x, pos.y + 1));
				
				
				//Room parts
				map[(int)(pos.x - 1), (int)(pos.y - 1)] = roomNum;
				map[(int)(pos.x - 1), (int)(pos.y + 1)] = roomNum;
				map[(int)(pos.x + 1), (int)(pos.y - 1)] = roomNum;
				map[(int)(pos.x + 1), (int)(pos.y + 1)] = roomNum;
			}
			
			if(type == "room") {
				//map[(int)pos.x, (int)pos.y] = 2;
				foreach(RoomTile tile in curRoom.roomtiles) {
					if((int)tile.type == 2) {
						map[((int)pos.x + (int)tile.pos.x - 1), ((int)pos.y + (int)tile.pos.y) - 1] = (int)tile.type;
					}
					if((int)tile.type == 3) {
						map[((int)pos.x + (int)tile.pos.x - 1), ((int)pos.y + (int)tile.pos.y) - 1] = canDoor(new Vector2( ((int)pos.x + (int)tile.pos.x - 1), ((int)pos.y + (int)tile.pos.y) - 1 ));
					}
				}
			}
			
			if(type == "event") {
				foreach(RoomTile tile in curRoom.roomtiles) {
					if((int)tile.type == 2) { // Event room type
						map[((int)pos.x + (int)tile.pos.x - 1), ((int)pos.y + (int)tile.pos.y) - 1] = 40;
					}
					if((int)tile.type == 3) { // Doors
						map[((int)pos.x + (int)tile.pos.x - 1), ((int)pos.y + (int)tile.pos.y) - 1] = 99;
					}
				}
			}
		} else {
			Vector2 newPos = new Vector2(Mathf.Floor(Random.Range(1, levelSize.x - 2)), Mathf.Floor(Random.Range(1, levelSize.y - 2)));
			createRoom(newPos, type);
			return;
		}
	}
	
	// Can a room be placed 
	bool canRoom(Vector2 pos, RoomTile[] tiles) {
		foreach(RoomTile tile in tiles) {
			if(((int)pos.x + (int)tile.pos.x) - 1 < 0
				|| ((int)pos.y + (int)tile.pos.y) - 1 < 0
				|| ((int)pos.x + (int)tile.pos.x) > levelSize.x - 1 
				|| ((int)pos.y + (int)tile.pos.y) > levelSize.y - 1) {
				return false;
			}
			
			if(map[((int)pos.x + (int)tile.pos.x) - 1, ((int)pos.y + (int)tile.pos.y) - 1] != 0) {
				return false;
			}
		}
		
		return true;
	}
	
	bool createSecretRoom(RoomTile[] halls) {
		Vector2 pos = halls[Random.Range(0, halls.Length)].pos;
		
		int roomType = Random.Range(40, 44);
		
		int roomI = Random.Range(0, rooms.Length);
		Room room = rooms[roomI];
		
		bool placed = false;
		
		Vector2[] positions = whereSecretRoomEntrance(pos);
		
		foreach(Vector2 position in positions) {
			Vector2 newRoomPos = searchPlaceSecretRoom(position, room);
			if(newRoomPos != Vector2.zero) {
				foreach(RoomTile tile in room.roomtiles) {
					if(tile.type == 2) {
						map[(int)newRoomPos.x + (int)tile.pos.x, (int)newRoomPos.y + (int)tile.pos.y] = roomType;
					}
					if(tile.type == 3) {
						map[(int)newRoomPos.x + (int)tile.pos.x, (int)newRoomPos.y + (int)tile.pos.y] = 99;
					}
				}
				return true;
			}
		}
		return createSecretRoom(halls);
	}
	
	// Can a secret room be placed
	Vector2 searchPlaceSecretRoom(Vector2 pos, Room room) {
		foreach(RoomTile tile in room.roomtiles) {
			float left = 0;
			float top = 0;
			
			if(tile.type == 3) { // Door
				left = room.minWidth - tile.pos.x;
				top = room.minHeight - tile.pos.y;
				Vector2 roomPos = new Vector2(pos.x + left, pos.y + top);
				if(canSecretRoom(roomPos, room)) {
					return roomPos;
				}
			}
		}
		
		return Vector2.zero;
	}
	
	// Can a secret room be placed
	bool canSecretRoom(Vector2 pos, Room room) {
		foreach(RoomTile tile in room.roomtiles) {
			Vector2 tilePos = new Vector2(pos.x + tile.pos.x, pos.y + tile.pos.y);
			
			if(posOutOfBounds(tilePos)) {
				return false;
			}
			if(map[(int)tilePos.x, (int)tilePos.y] != 0) {
				return false;
			}
		}
		return true;
	}
	
	
	// Where can a secret room be placed
	Vector2[] whereSecretRoomEntrance(Vector2 pos) {
		
		Vector2[] positions = new Vector2[2];
		int i = 0;
		
		if(!posOutOfBounds(new Vector2(pos.x - 1, pos.y))) {
			if(map[(int)pos.x - 1, (int)pos.y] == 0) {
				positions[i] = new Vector2(pos.x - 1, pos.y);
				i++;
			}
		}
		if(!posOutOfBounds(new Vector2(pos.x + 1, pos.y))) {
			if(map[(int)pos.x + 1, (int)pos.y] == 0) {
				positions[i] = new Vector2(pos.x + 1, pos.y);
				i++;
			}
		}
		if(!posOutOfBounds(new Vector2(pos.x, pos.y - 1))) {
			if(map[(int)pos.x, (int)pos.y - 1] == 0) {
				positions[i] = new Vector2(pos.x, pos.y - 1);
				i++;
			}
		}
		if(!posOutOfBounds(new Vector2(pos.x, pos.y + 1))) {
			if(map[(int)pos.x, (int)pos.y + 1] == 0) {
				positions[i] = new Vector2(pos.x, pos.y + 1);
				i++;
			}
		}
		
		return positions;
	}
	
	bool posOutOfBounds(Vector2 pos) {
		if(pos.x < 0 
			|| pos.x > levelSize.x - 1
			|| pos.y < 0
			|| pos.y > levelSize.y - 1) {
			return true;
		}
		
		return false;
	}
	
	// Can a door be placed or should it be part of the room
	int canDoor(Vector2 pos) {
		int tile = 3;
		if((int)pos.x == 0 || (int)pos.y == 0 || pos.x == levelSize.x - 1 || pos.y == levelSize.y - 1) {
			tile = 2;
		}
		if(tile == 3) {
			doors++;
		}
		return tile;
	}
	
	// Can a hall be placed on pos
	bool canHall(Vector2 pos, int num) {
		if(pos.x < 0 
			|| pos.x > levelSize.x - 1
			|| pos.y < 0
			|| pos.y > levelSize.y - 1) {
			return false;
		}
		if(map[(int)pos.x, (int)pos.y] == 0 || map[(int)pos.x, (int)pos.y] == num) {
			return true;
		}
		return false;
	}
	
	bool whereHall(Vector2 pos, int num) {
		if(canHall(new Vector2(pos.x - 1, pos.y), num)) {
			map[(int)pos.x - 1, (int)pos.y] = num;
			return placeHall(new Vector2(pos.x - 1, pos.y), new Vector2(-1, 0), num);
		}
		if(canHall(new Vector2(pos.x + 1, pos.y), num)) {
			map[(int)pos.x + 1, (int)pos.y] = num;
			return placeHall(new Vector2(pos.x + 1, pos.y), new Vector2(1, 0), num);
		}
		if(canHall(new Vector2(pos.x, pos.y - 1), num)) {
			map[(int)pos.x, (int)pos.y - 1] = num;
			return placeHall(new Vector2(pos.x, pos.y - 1), new Vector2(0, -1), num);
		}
		if(canHall(new Vector2(pos.x, pos.y + 1), num)) {
			map[(int)pos.x, (int)pos.y + 1] = num;
			return placeHall(new Vector2(pos.x, pos.y + 1), new Vector2(0, 1), num);
		}
		
		return false;
	}
	
	// Place a Hall at pos + dir
	bool placeHall(Vector2 pos, Vector2 dir, int num) {
		if(canHall(new Vector2(pos.x + dir.x, pos.y + dir.y), num)) {
			if(map[(int)pos.x + (int)dir.x, (int)pos.y + (int)dir.y] == 0) {
				map[(int)pos.x + (int)dir.x, (int)pos.y + (int)dir.y] = num;
				return true;
			}else if(map[(int)pos.x + (int)dir.x, (int)pos.y + (int)dir.y] == num) {
				return placeHall(new Vector2(pos.x + dir.x, pos.y + dir.y), dir, num);
			}
		}
		
		return false;
	}
	
	// Remove Excess halls
	void removeExcessHall(Vector2 pos) {
		int totalHalls = 0;
		Vector2 dir = new Vector2();
		//TODO check if outside map.
		
		if(pos.x - 1 > 0) { 
			if(map[(int)pos.x - 1, (int)pos.y] >= 1000) {
				totalHalls++;
				dir = new Vector2(-1, 0);
			}
		}
		
		if(pos.x + 1 < levelSize.x - 1) { 
			if(map[(int)pos.x + 1, (int)pos.y] >= 1000) {
				totalHalls++;
				dir = new Vector2(1, 0);
			}
		}
		
		if(pos.y - 1 > 0) { 
			if(map[(int)pos.x, (int)pos.y - 1] >= 1000) {
				totalHalls++;
				dir = new Vector2(0, -1);
			}
		}
		
		if(pos.y + 1 < levelSize.y - 1) { 
			if(map[(int)pos.x, (int)pos.y + 1] >= 1000) {
				totalHalls++;
				dir = new Vector2(0, 1);
			}
		}
		
		if(totalHalls == 1) {
			map[(int)pos.x, (int)pos.y] = 0;
			Vector2 newPos = new Vector2(pos.x + dir.x, pos.y + dir.y);
			removeExcessHall(newPos);
		} else if(totalHalls == 0) {
			map[(int)pos.x, (int)pos.y] = 0;
		}
	}
	
	void spawnPickup(Vector3 pos, int type) {
		
		switch(type) {
		case 40:{ // Gold
			break;
		}
		case 41:{ // Ammo
			break;
		}
		case 42:{ // Health
			break;
		}
		case 43:{ // Gun
			break;
		}
		}
		
		GameObject pickupPrefab = spawnablePickups[Random.Range(0, spawnablePickups.Length)];
		GameObject go = Instantiate(pickupPrefab, pos, transform.rotation) as GameObject;
		go.transform.parent = gameObject.transform;
	}
	
	void spawnEnemies() {
		GameObject enemyPrefab = enemies[0];
		
		GameObject[] enemyPrefabs = getEnemySpawnTable();
		Vector2[] enemySpawnPositions = getEnemySpawnPositions();
		
		foreach(GameObject newEnemy in enemyPrefabs) {
			Vector2 spawnPos = enemySpawnPositions[Random.Range(0, enemySpawnPositions.Length)];
			
			GameObject go = Instantiate(newEnemy, Vector3.zero, enemies[0].transform.rotation) as GameObject;
			
			if(go.GetComponent<Evil>()) {
				spawnPos = getFloor(5);
				go.GetComponent<Evil>().startPos = new Vector3(spawnPos.x * 5, newEnemy.transform.position.y, spawnPos.y * 5);
			}
			if(go.GetComponent<Enemy>()) {
				go.GetComponent<Enemy>().startPos = new Vector3(spawnPos.x * 5, newEnemy.transform.position.y, spawnPos.y * 5);
				Enemy enemy = go.GetComponent<Enemy>();
				enemy.damage = Mathf.Min((int)enemy.maxDamage, (int)enemy.baseDamage + (int)Mathf.Floor(level * enemy.modDamage));
				enemy.health = Mathf.Min((int)enemy.maxHealth, (int)enemy.baseHealth + (int)Mathf.Floor(level * enemy.modHealth));
				enemy.accuracy = Mathf.Min((int)enemy.maxAccuracy, (int)enemy.baseAccuracy + (int)Mathf.Floor(level * enemy.modAccuracy));
				enemy.range = Mathf.Min((int)enemy.maxRange, (int)enemy.baseRange + (int)Mathf.Floor(level * enemy.modRange));
				enemy.sightRange = Mathf.Min((int)enemy.maxSightRange, (int)enemy.baseSightRange + (int)Mathf.Floor(level * enemy.modSightRange));
			}
			
			
			go.transform.parent = GameObject.Find("LevelGenerator").transform;
		}
	}
	
	
	GameObject[] getEnemySpawnTable() {
		string spawnString = "";
		
		int maxEnemies = totalEnemies + (totalEnemiesMod * level);
		
		if(Random.Range(0, 100) <= 25) {
			spawnString += "E";
		}
		
		for(int i = 0; i < maxEnemies; i++) {
			int spawnChance = Random.Range(0, 100);
			if(spawnChance <= 20) {
				spawnString += "1";
			} else{
				spawnString += "0";
			}
		}
		
		GameObject[] spawnTable = new GameObject[(int)(spawnString.Length)];
		
		for(int i = 0; i < spawnString.Length; i++) {
			switch(spawnString[i].ToString()) {
			case "E": {
				spawnTable[i] = evil;
				break;
			}
			case "0" : {
				spawnTable[i] = enemies[0];
				break;
			}
			case "1" : {
				spawnTable[i] = enemies[1];
				break;
			}
			}
		}
		
		return spawnTable;
	}
	
	
	Vector2[] getEnemySpawnPositions() {
		Vector2[] tempSpawnPos = new Vector2[(int)levelSize.x * (int)levelSize.y];
		int i = 0;
		
		for(int x = 0; x < levelSize.x; x++) {
			for(int y = 0; y < levelSize.y; y++) {
				if(map[x, y] != 0 && map[x, y] != 4 && map[x, y] != 7 && map[x, y] != 3) {
					tempSpawnPos[i] = new Vector2(x, y);
					i++;
				}
			}
		}
		Vector2[] spawnPos = new Vector2[i + 1];
		i = 0;
		foreach(Vector2 pos in tempSpawnPos) {
			if(pos != Vector2.zero) {
				spawnPos[i] = pos;
				i++;
			}
		}
		
		return spawnPos;
	}
	
	// This extra for guessing purposes
	bool canEnemySpawn(Vector2 pos) {
		return true;
	}
	
	// Get first floor tile with this type
	Vector2 getFloor(int type) {
		for(int x = 0; x < levelSize.x; x++) {
			for(int y = 0; y < levelSize.y; y++) {
				if(map[x, y] == type) {
					return new Vector2(x, y);
				}
			}
		}
		return Vector2.zero;
	}
}
