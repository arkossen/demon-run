using UnityEngine;
using System.Collections;

public class Room {
	public RoomTile[] roomtiles;
	
	public int minWidth;
	public int minHeight;
	
	public int maxWidth = 0;
	public int maxHeight = 0;
	
	public Room(RoomTile[] newRoomtiles) {
		roomtiles = newRoomtiles;
		
		foreach(RoomTile tile in roomtiles) {
			if(minWidth != null) {
				if(tile.pos.x < minWidth) {
					minWidth = (int)tile.pos.x;
				}
			}else {
				minWidth = (int)tile.pos.x;
			}
			if(minHeight != null) {
				if (tile.pos.y < minHeight) {
					minHeight = (int)tile.pos.y;
				}
			}else {
				minHeight = (int)tile.pos.y;
			}
			
			if(tile.pos.x > maxWidth) {
				maxWidth = (int)tile.pos.x;
			}
			if(tile.pos.y > maxHeight) {
				maxHeight = (int)tile.pos.y;
			}
		}
	}
}
