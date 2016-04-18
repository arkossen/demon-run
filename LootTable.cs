using UnityEngine;
using System.Collections;

public class LootTable : MonoBehaviour {
	
	public GameObject[] lootables;
	public int[] chances;
	public int overallDropChance;
	
	GameObject drop(Vector3 pos) {
		int totalChance = 0;
		int curChance = 0;
		int i = 0;
		GameObject loot = new GameObject();
		
		foreach(int chance in chances)
			totalChance += chance;
		
		curChance = Random.Range(0, totalChance);
		totalChance = 0;
		
		foreach(int chance in chances) {
			if(totalChance >= curChance && curChance <= totalChance + chance) {
				loot = lootables[i];
				break;
			}else {
				totalChance += chance;
				i++;
			}
		}
		
		return loot;
	}
}
