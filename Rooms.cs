using UnityEngine;
using System.Collections;

public class Rooms{
	
	Room[] rooms;
	
	// Use this for initialization
	void Start () {
		RoomTile[] roomtiles1 = new RoomTile[17];
		
		roomtiles1[0] = new RoomTile(Vector2.zero, 2);
		roomtiles1[1] = new RoomTile(new Vector2(), 2);
		roomtiles1[2] = new RoomTile(new Vector2(), 2);
		roomtiles1[3] = new RoomTile(new Vector2(), 2);
		roomtiles1[4] = new RoomTile(new Vector2(), 2);
		roomtiles1[5] = new RoomTile(new Vector2(), 2);
		roomtiles1[6] = new RoomTile(new Vector2(), 2);
		roomtiles1[7] = new RoomTile(new Vector2(), 2);
		roomtiles1[8] = new RoomTile(new Vector2(), 2);
		roomtiles1[9] = new RoomTile(new Vector2(), 2);
		roomtiles1[10] = new RoomTile(new Vector2(), 2);
		roomtiles1[11] = new RoomTile(new Vector2(), 2);
		roomtiles1[12] = new RoomTile(new Vector2(), 2);
		roomtiles1[13] = new RoomTile(new Vector2(), 2);
		roomtiles1[14] = new RoomTile(new Vector2(), 2);
		roomtiles1[15] = new RoomTile(new Vector2(), 2);
		roomtiles1[16] = new RoomTile(new Vector2(), 2);
		
	}
}
