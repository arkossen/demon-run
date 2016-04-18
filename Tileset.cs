using UnityEngine;
using System.Collections;

public class Tileset {
	
	public string type;
	
	public GameObject[] wallPrefabs;
	public Texture[] wallTextures;
	
	public GameObject[] floorPrefabs;
	public Texture[] floorTextures;
	
	public GameObject[] ceilingPrefabs;
	public Texture[] ceilingTextures;
	
	public GameObject[] doorPrefabs;
	public Texture[] doorTextures;
	
	public GameObject[] eventPrefabs;
	public Texture[] eventTextures;
	
	public GameObject[] destructableWalls;
	public Texture[] destructableWallTextures;
	public Texture[] destructedWallTextures;
	
	public GameObject[] enemyPrefabs;
	
	public Tileset(string incType, 
			GameObject[] incWallPrefabs, Texture[] incWallTextures,
			GameObject[] incFloorPrefabs, Texture[] incFloorTextures,
			GameObject[] incCeilingPrefabs, Texture[] incCeilingTextures,
			GameObject[] incDoorPrefabs, Texture[] incDoorTextures,
			GameObject[] incEventPrefabs, Texture[] incEventTextures,
			GameObject[] incDestructableWalls, Texture[] incDestructableWallTextures, Texture[] incDestructedWallTextures,
		 	GameObject[] incEnemyPrefabs) {
		
		type = type;
		
		wallPrefabs = incWallPrefabs;
		wallTextures = incWallTextures;
	
		floorPrefabs = incFloorPrefabs;
		floorTextures = incFloorTextures;
	
		ceilingPrefabs = incCeilingPrefabs;
		ceilingTextures = incCeilingTextures;
	
		doorPrefabs = incDoorPrefabs;
		doorTextures = incDoorTextures;
	
		eventPrefabs = incEventPrefabs;
		eventTextures = incEventTextures;
		
		destructableWalls = incDestructableWalls;
		destructableWallTextures = incDestructableWallTextures;
		destructedWallTextures = incDestructedWallTextures;
		
		enemyPrefabs = incEnemyPrefabs;
	}
}
