using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour {
	
	GameObject shopGUI;
	
	public GameObject[] items;
	public int[] itemsChance;
	GameObject[] sellingItems = new GameObject[9];
	int[] sellingItemsNumbers = new int[9];
	
	public GameObject shopItemInfo;
	GameObject typed;
	
	string buyString = "";
	float buyTime = 100;
	float buyTimeMax = 100;
	
	bool browsing = false;
	
	// 150 x+
	// 137 y-
	// pos 0 = 60, 449
	
	// Use this for initialization
	void Start () {
		shopGUI = GameObject.Find("ShopGUI");
		
		int maxItems = Random.Range(1, 9);
		
		typed = Instantiate(shopItemInfo, new Vector3(0, 0, 2), transform.rotation) as GameObject;
		typed.GetComponent<GUIText>().pixelOffset = new Vector2(520, 600);
		typed.GetComponent<GUIText>().fontSize = 45;
		typed.transform.parent = shopGUI.transform;
		
		int xPos = 0;
		int yPos = 0;
		for(int i = 0; i < maxItems; i++) {
			int addedChance = 0;
			int chance = Random.Range(1, items.Length);
			GameObject shopItem = Instantiate(items[chance], new Vector3(0, 0, 10.1f), transform.rotation) as GameObject;
			GameObject info = Instantiate(shopItemInfo, new Vector3(0, 0, 10.2f), transform.rotation) as GameObject;
			
			int price = shopItem.GetComponent<ShopItem>().price;
			shopItem.GetComponent<GUITexture>().pixelInset = new Rect(60 + (xPos * 150), 449 - (yPos * 137), shopItem.GetComponent<GUITexture>().pixelInset.width, shopItem.GetComponent<GUITexture>().pixelInset.height);
			shopItem.transform.parent = shopGUI.transform;
			
			sellingItems[i] = shopItem;
			sellingItemsNumbers[i] = Random.Range(100, 999);
			
			info.GetComponent<GUIText>().text = "|" + sellingItemsNumbers[i].ToString() + "|  $" + price.ToString();
			info.GetComponent<GUIText>().pixelOffset = new Vector2(60 + (xPos * 150), 580 - (yPos * 137));
			info.transform.parent = shopGUI.transform;
			
			xPos++;
			if(xPos >= 3) {
				xPos = 0;
			}
			if(i >= 2) {
				yPos = 1;
			}
			if(i >= 5) {
				yPos = 2;
			}
		}
		
		shopGUI.GetComponent<GUITexture>().enabled = false;
		foreach (Transform child in shopGUI.transform) {
			if(child.GetComponent<GUITexture>()) {
				child.GetComponent<GUITexture>().enabled = false;
			}
			if(child.GetComponent<GUIText>()) {
				child.GetComponent<GUIText>().enabled = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(shopGUI.GetComponent<GUITexture>().enabled) {
			if (Input.GetKeyDown(KeyCode.Keypad0)) {
				buyString += "0";
			}
			if (Input.GetKeyDown(KeyCode.Keypad1)) {
				buyString += "1";
			}
			if (Input.GetKeyDown(KeyCode.Keypad2)) {
				buyString += "2";
			}
			if (Input.GetKeyDown(KeyCode.Keypad3)) {
				buyString += "3";
			}
			if (Input.GetKeyDown(KeyCode.Keypad4)) {
				buyString += "4";
			}
			if (Input.GetKeyDown(KeyCode.Keypad5)) {
				buyString += "5";
			}
			if (Input.GetKeyDown(KeyCode.Keypad6)) {
				buyString += "6";
			}
			if (Input.GetKeyDown(KeyCode.Keypad7)) {
				buyString += "7";
			}
			if (Input.GetKeyDown(KeyCode.Keypad8)) {
				buyString += "8";
			}
			if (Input.GetKeyDown(KeyCode.Keypad9)) {
				buyString += "9";
			}
			
			if(buyString.Length >= 3) {
				buyTime -= Time.deltaTime * 100;
				if(buyTime < 0) {
					int i = 0;
					foreach(int num in sellingItemsNumbers) {
						if(num.ToString() == buyString && sellingItems[i]) {
							buyItem(sellingItems[i]);
							break;
						}
						i++;
					}
					buyString = "";
					buyTime = buyTimeMax;
				}
			}
			typed.GetComponent<GUIText>().text = buyString;
		}
	}
	
	void buyItem(GameObject go) {
		Player player = GameObject.Find("player").GetComponent<Player>();
		ShopItem shopItem = go.GetComponent<ShopItem>();
		gameObject.GetComponent<AudioSource>().Play();
		
		if(player.gold >= shopItem.price) {
			player.gold -= shopItem.price;
			player.handleShopItem(shopItem);
			Destroy(go);
		}
	}
	
	void showShop() {
		shopGUI.GetComponent<GUITexture>().enabled = true;
		foreach (Transform child in shopGUI.transform) {
			if(child.GetComponent<GUITexture>()) {
				child.GetComponent<GUITexture>().enabled = true;
			}
			if(child.GetComponent<GUIText>()) {
				child.GetComponent<GUIText>().enabled = true;
			}
		}
	}
	
	void hideShop() {
		shopGUI.GetComponent<GUITexture>().enabled = false;
		foreach (Transform child in shopGUI.transform) {
			if(child.GetComponent<GUITexture>()) {
				child.GetComponent<GUITexture>().enabled = false;
			}
			if(child.GetComponent<GUIText>()) {
				child.GetComponent<GUIText>().enabled = false;
			}
		}
	}
	
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "player") {
			showShop();
		}
	}
	void OnTriggerExit(Collider col) {
		if(col.gameObject.tag == "player") {
			hideShop();
		}
	}
}
