using UnityEngine;
using System.Collections;
using Pathfinding;
using System.IO;

public class TilesetLoader {
	
	Tileset[] tilesets;
	
	public TilesetLoader() {
		loadTilesets();
	}
	
	public Tileset getTileset(string name) {
		
		foreach(Tileset tileset in tilesets) {
			if(tileset.type == name) {
				return tileset;
			}
		}
		
		return null;
	}
	
	public GameObject[] getGameObjectPrefabs(string dir, string names) {
		GameObject[] prefabs = new GameObject[0];
	    string[] prefabNames = names.Split("."[0]);
		
	    foreach(string prefabName in prefabNames) {
			GameObject prefab = Resources.Load("tilesets/" + dir + "/prefabs/" + prefabName) as GameObject;
			GameObject[] curPrefabs =  new GameObject[prefabs.Length + 1];
			
			for(int i = 0; i < prefabs.Length; i++) {
				curPrefabs[i] = prefabs[i];
			}
			curPrefabs[curPrefabs.Length - 1] = prefab;
			prefabs = curPrefabs;
		}
		return prefabs;
	}
	
	public Texture[] getTexturePrefabs(string dir, string names) {
		Texture[] textures = new Texture[0];
	    string[] textureNames = names.Split("."[0]);
		
	    foreach(string textureName in textureNames) {
			Texture texture = Resources.Load("tilesets/" + dir + "/textures/" + textureName) as Texture;
			Texture[] curTextures =  new Texture[textures.Length + 1];
			
			for(int i = 0; i < textures.Length; i++) {
				curTextures[i] = textures[i];
			}
			curTextures[curTextures.Length - 1] = texture;
			textures = curTextures;
		}
		
		return textures;
	}
	
	public void loadTilesets() {
	    TextAsset sr = Resources.Load("tilesets/tilesets") as TextAsset;
	
	    string[] lines = sr.text.Split("|"[0]);
		
	    foreach(string line in lines) {
		    TextAsset tilesetFile = Resources.Load("tilesets/" + line + "/" + line) as TextAsset;
			
			string[] tilesetString = tilesetFile.text.Split(","[0]);
			
			string type = tilesetString[0];
			
			GameObject[] wallPrefabs = getGameObjectPrefabs(line, tilesetString[1]);
			Texture[] wallTextures = getTexturePrefabs(line, tilesetString[2]);
			
			GameObject[] floorPrefabs = getGameObjectPrefabs(line, tilesetString[3]);
			Texture[] floorTextures = getTexturePrefabs(line, tilesetString[4]);
			
			GameObject[] ceilingPrefabs = getGameObjectPrefabs(line, tilesetString[5]);
			Texture[] ceilingTextures = getTexturePrefabs(line, tilesetString[6]);
			
			GameObject[] doorPrefabs = getGameObjectPrefabs(line, tilesetString[7]);
			Texture[] doorTextures = getTexturePrefabs(line, tilesetString[8]);
			
			GameObject[] eventPrefabs = getGameObjectPrefabs(line, tilesetString[9]);
			Texture[] eventTextures = getTexturePrefabs(line, tilesetString[10]);
			
			GameObject[] destructableWalls = getGameObjectPrefabs(line, tilesetString[11]);
			Texture[] destructableWallTextures = getTexturePrefabs(line, tilesetString[12]);
			Texture[] destructedWallTextures = getTexturePrefabs(line, tilesetString[13]);
			
			GameObject[] enemyPrefabs = getGameObjectPrefabs(line, tilesetString[14]);
			
			Tileset tileset = new Tileset(type, 
				wallPrefabs, wallTextures, 
				floorPrefabs, floorTextures, 
				ceilingPrefabs, ceilingTextures, 
				doorPrefabs, doorTextures, 
				eventPrefabs, eventTextures, 
				destructableWalls, destructableWallTextures, destructedWallTextures,
				enemyPrefabs
			);
			Tileset[] curTilesets =  new Tileset[tilesets.Length + 1];
			for(int i = 0; i < tilesets.Length; i++) {
				curTilesets[i] = tilesets[i];
			}
			curTilesets[curTilesets.Length - 1] = tileset;
			tilesets = curTilesets;
					
	    }
	}
}
