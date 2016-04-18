using UnityEngine;
using System.Collections;

public class RoomTile {
	public Vector2 pos;
	public int type;
	
	public RoomTile(Vector2 newPos, int newType) {
		pos = newPos;
		type = newType;
	}
}
