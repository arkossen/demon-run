using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public Weapon selectedWeapon;
	public GameObject[] ownedWeapons;
	public int[] ammo;
	public Texture[] uiWeapons;
	public Vector2[] uiWeaponsSizes;
	
	public int gold = 0;
	Vector3 oldPos;
	bool up = true;
	
	public int health = 10;
	public int armor = 0;
	
	public int maxHealth = 10;
	public int maxArmor = 5;
	
	AudioSource audioSource;
	public AudioClip[] sndGotHit;
	public AudioClip sndDie;
	
	float gotHitTime = 0;
	float gotHitTimeMax = 100;
	
	// Upgrades
	bool boostEnabled = false;
	bool seeSecretsEnabled = false;
	bool repelDemonEnabled = false;
	bool seeThroughEyesEnabled = false;
	
    private Camera cam;
    private Plane[] planes;
    private GameObject[] camPlanes = new GameObject[6];
	
	public bool switchingScreen = false;
	public string state = "PLAYING";
	
	// Dashing
	Vector3 dashDirection = new Vector3(0, 0);
	float maxDashTime = 20;
	float dashTime = 0;
	float pressDashTime = 0;
	float maxPressDashTime = 10;
	
	string comboChain = "";
	
	// Heart 
	public GameObject heartPrefab;
	
	// Use this for initialization
	void Start () {
		oldPos = transform.position;
		audioSource = gameObject.GetComponent<AudioSource>();
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		
		Transform camTransform = GameObject.Find("Main Camera").transform;
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
		
        int i = 0;
        while (i < planes.Length) {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
			Destroy(p.GetComponent<Collider>());
			Destroy(p.GetComponent<Renderer>());
            p.name = "Plane " + i.ToString();
            p.transform.position = -planes[i].normal * planes[i].distance;
            p.transform.rotation = Quaternion.FromToRotation(Vector3.up, planes[i].normal);
			camPlanes[i] = p;
            i++;
        }
		selectedWeapon = showWeapon(ownedWeapons[0]);
	}
	
	void checkIfEvil() {
		if(GameObject.FindGameObjectWithTag("Evil")) {
			GameObject evil = GameObject.FindGameObjectWithTag("Evil");
			Transform camTransform = GameObject.Find("Main Camera").transform;
			evil.GetComponent<Evil>().speed = evil.GetComponent<Evil>().maxSpeed;
			
	        planes = GeometryUtility.CalculateFrustumPlanes(cam);
	        int i = 0;
	        while (i < planes.Length) {
				GameObject p = camPlanes[i];
	            p.transform.position = -planes[i].normal * planes[i].distance;
	            p.transform.rotation = Quaternion.FromToRotation(Vector3.up, planes[i].normal);
	            i++;
	        }
			
			
			if (GeometryUtility.TestPlanesAABB(planes, evil.collider.bounds)) { 
				RaycastHit hit;
				float wallDist = -1;
				if(Physics.Raycast (camTransform.position, (evil.transform.position - camTransform.position).normalized, out hit, 100, 1 << 9)) {
					wallDist = Vector3.Distance(transform.position, hit.transform.position);
				}
				
				if(Physics.Raycast (camTransform.position, (evil.transform.position - camTransform.position).normalized, out hit, 100, 1 << 12)) {
					float dist = Vector3.Distance(transform.position, hit.transform.position);
					if(wallDist == -1 || dist < wallDist) {
						if(hit.transform.gameObject.tag == "Evil") {
							hit.transform.gameObject.GetComponent<Evil>().speed = evil.GetComponent<Evil>().maxLookSpeed;
						}
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!switchingScreen) {
			updateHealth();
			
			// Hide gotHit
			if(GameObject.Find("gotHit").GetComponent<GUITexture>().color.a > 0) {
				Color col = GameObject.Find("gotHit").GetComponent<GUITexture>().color;
				col.a -= Time.deltaTime;
				GameObject.Find("gotHit").GetComponent<GUITexture>().color = col;
			}
			
			checkIfEvil();
			
			if(health <= 0) {
				Die();
				return;
			}
			if(dashTime <= 0) {
				if(Input.GetKeyDown("w")) {
					comboChain += "w1";
					pressDashTime = maxPressDashTime;
				}
				if(Input.GetKeyDown("a")) {
					comboChain += "a1";
					pressDashTime = maxPressDashTime;
				}
				if(Input.GetKeyDown("s")) {
					comboChain += "s1";
					pressDashTime = maxPressDashTime;
				}
				if(Input.GetKeyDown("d")) {
					comboChain += "d1";
					pressDashTime = maxPressDashTime;
				}
				if(Input.GetKeyUp("w")) {
					comboChain += "w2";
					pressDashTime = maxPressDashTime;
				}
				if(Input.GetKeyUp("a")) {
					comboChain += "a2";
					pressDashTime = maxPressDashTime;
				}
				if(Input.GetKeyUp("s")) {
					comboChain += "s2";
					pressDashTime = maxPressDashTime;
				}
				if(Input.GetKeyUp("d")) {
					comboChain += "d2";
					pressDashTime = maxPressDashTime;
				}
			}
			
			if(pressDashTime >= 0) {
				pressDashTime -= Time.deltaTime * 100;
			}else {
				comboChain = "";
			}
			
			if(comboChain.Contains("w1w2w1")) {
				comboChain = "";
				dashTime = maxDashTime;
				dashDirection = transform.TransformDirection(Vector3.forward);
			}
			if(comboChain.Contains("a1a2a1")) {
				comboChain = "";
				dashTime = maxDashTime;
				dashDirection = transform.TransformDirection(Vector3.left);
			}
			if(comboChain.Contains("s1s2s1")) {
				comboChain = "";
				dashTime = maxDashTime;
				dashDirection = transform.TransformDirection(Vector3.back);
			}
			if(comboChain.Contains("d1d2d1")) {
				comboChain = "";
				dashTime = maxDashTime;
				dashDirection = transform.TransformDirection(Vector3.right);
			}
			
			if(dashTime >= 0) {
				dashTime -= Time.deltaTime * 100;
		        gameObject.GetComponent<CharacterController>().SimpleMove(dashDirection * 25);
			} else {
				dashDirection = Vector3.zero;
			}
			
			
			
	        if (Input.GetKeyDown("1")) {
				//Debug.Log("Select CROWBAR!");
				if(ownedWeapons[0]) {
					selectedWeapon = showWeapon(ownedWeapons[0]);
				}
			}
	        if (Input.GetKeyDown("2")) {
				//Debug.Log("Select PISTOL!");
				if(ownedWeapons[1]) {
					selectedWeapon = showWeapon(ownedWeapons[1]);
				}
			}
	        if (Input.GetKeyDown("3")) {
				//Debug.Log("Select SHOTGUN!");
				if(ownedWeapons[2]) {
					selectedWeapon = showWeapon(ownedWeapons[2]);
				}
			}
			if (Input.GetButton("Fire1")) {
				
       			Screen.lockCursor = true;
				
				Transform cam = GameObject.Find("Main Camera").transform;
				if(selectedWeapon) {
					if(ammo[selectedWeapon.id] > 0 || selectedWeapon.id == 0) {
						if(selectedWeapon.attack(cam.position, cam.forward)) {
							audioSource.clip = selectedWeapon.sndShoot;
							audioSource.Play();
							ammo[selectedWeapon.id]--;
						}
					} else if(selectedWeapon.sndEmpty) {
						if(!audioSource.isPlaying) {
							audioSource.clip = selectedWeapon.sndEmpty;
							audioSource.Play();
						}
					}
					
				}
			}
			
			if(selectedWeapon) {
				GameObject.Find("ammo").GetComponent<GUIText>().text = ammo[selectedWeapon.id].ToString();
				if(selectedWeapon.id == 0) {
					GameObject.Find("ammo").GetComponent<GUIText>().text = "0";
				}
			}
			
			GameObject.Find("goldGUI").GetComponent<GUIText>().text = gold.ToString();
			
			// Move selected weapon up and down
			if(oldPos != transform.position) {
				if(selectedWeapon) {
					Vector3 gunPos = selectedWeapon.gameObject.transform.localPosition;
					if(up && gunPos.y > selectedWeapon.localPos.y + 0.02) {
						up = false;
					}else if(!up && gunPos.y < selectedWeapon.localPos.y - 0.02) {
						up = true;
					}
					if(up) {
						selectedWeapon.gameObject.transform.localPosition = new Vector3(gunPos.x, gunPos.y + (Time.deltaTime / 5), gunPos.z);
					}else {
						selectedWeapon.gameObject.transform.localPosition = new Vector3(gunPos.x, gunPos.y - (Time.deltaTime / 5), gunPos.z);
					}
				}
			}
			
			oldPos = transform.position;
			
		} else { // WHEN SWITCHING LEVEL!
			if(state == "PLAYING") {
		        if (Input.anyKeyDown && GameObject.Find("TussenSchermHUB").GetComponent<ScoreScreen>().showTime <= 0) {
					GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>().createLevel();
					GameObject.Find("TussenSchermHUB").GetComponent<ScoreScreen>().hideScoreScreen();
					switchingScreen = false;
				}
			}
			if(state == "GAMEOVER" && GameObject.Find("TussenSchermHUB").GetComponent<ScoreScreen>().showTime <= 0) {
		        if (Input.anyKeyDown) {
					Reset();
				}
			}
		}
	}
	
	void updateHealth() {
		GameObject.Find("Health").GetComponent<GUIText>().text = health.ToString();
		GameObject.Find("Armor").GetComponent<GUIText>().text = armor.ToString();
		
		GameObject[] hearts = GameObject.FindGameObjectsWithTag("Heart");
		
		if(hearts.Length != maxHealth / 2) {
			setHearts();
			hearts = GameObject.FindGameObjectsWithTag("Heart");
		}
		
		int halves = 0;
		for(int i = 0; i < maxHealth; i++) {
			GameObject heart = hearts[(int)Mathf.Floor(i / 2)];
			if(health < i) {
				heart.GetComponent<GUITexture>().texture = heart.GetComponent<Heart>().emptyHeart;
			}
			
			if(health - 1 >= i) {
				if(halves == 0) {
					heart.GetComponent<GUITexture>().texture = heart.GetComponent<Heart>().halfHeart;
				}
				if(halves == 1) {
					heart.GetComponent<GUITexture>().texture = heart.GetComponent<Heart>().fullHeart;
				}
			}
			
			halves++;
			if(halves == 2) {
				halves = 0;
			}
		}
		
		if(armor > 0) {
			GameObject.Find("Armor").GetComponent<GUIText>().enabled = true;
			GameObject.Find("ArmorTexture").GetComponent<GUITexture>().enabled = true;
		} else {
			GameObject.Find("Armor").GetComponent<GUIText>().enabled = false;
			GameObject.Find("ArmorTexture").GetComponent<GUITexture>().enabled = false;
		}
	}
	
	void setHearts() {
		GameObject[] hearts = GameObject.FindGameObjectsWithTag("Heart");
		
		foreach(GameObject heart in hearts) {
			Destroy(heart);
		}
		
		int x = 0;
		int y = 0;
		
		for(int i = 0; i < Mathf.Ceil(maxHealth / 2); i++) {
			GameObject heart = Instantiate(heartPrefab, new Vector3(0.7f, 0.9f, 0), transform.rotation) as GameObject;
			heart.GetComponent<GUITexture>().pixelInset = new Rect(0 + (36 * x), 0 + -(y * 36), 32, 32);
			heart.GetComponent<GUITexture>().texture = heart.GetComponent<Heart>().emptyHeart;
			x++;
			if(x == 5) {
				y++;
			}
			if(x == 5) {
				x = 0;
			}
		}
	}
	
	void Die() {
		if(audioSource.clip != sndDie) {
			audioSource.clip = sndDie;
			audioSource.Play();
		}
		GameObject.Find("TussenSchermHUB").GetComponent<ScoreScreen>().showScoreScreen("GameOverScreen");
		switchingScreen = true;
		state = "GAMEOVER";
	}
	
	void Reset() {
		selectedWeapon = showWeapon(ownedWeapons[0]);
		
		ownedWeapons[1] = null;
		ownedWeapons[2] = null;
		ammo[1] = 0;
		ammo[2] = 0;
		
		
		health = 10;
		armor = 0;
		maxHealth = 10;
		maxArmor = 5;
		gold = 0;
		
		GUITexture close = GameObject.Find("EvilClose").GetComponent<GUITexture>();
		Color col = close.color;
		col.a = 0;
		close.color = col;
		
		GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>().level = 0;
		GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>().destroyLevel();
		GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>().createLevel();
		GameObject.Find("TussenSchermHUB").GetComponent<ScoreScreen>().hideScoreScreen();
		GameObject.Find("TussenSchermHUB").GetComponent<ScoreScreen>().score = 0;
		state = "PLAYING";
		
		
		switchingScreen = false;
	}
	
	Weapon showWeapon(GameObject weapon) {
		bool showWeapon = true;
		Weapon newWeapon = selectedWeapon;
		
		if(selectedWeapon) {
			if(selectedWeapon.id != weapon.GetComponent<Weapon>().id) {
				showWeapon = true;
				Destroy(selectedWeapon.transform.gameObject);
			}else {
				showWeapon = false;
			}
		}
		
		if(showWeapon) { // SHOW THA WEAPON!
			GameObject cam = GameObject.Find("Main Camera");
			GameObject weaponGo = Instantiate(weapon, transform.position, transform.rotation) as GameObject;
			int id = weapon.GetComponent<Weapon>().id;
			GameObject.Find("SelectedWeaponUI").GetComponent<GUITexture>().texture = uiWeapons[id];
			GameObject.Find("SelectedWeaponUI").GetComponent<GUITexture>().pixelInset = new Rect(40-(uiWeaponsSizes[id].x), 40-(uiWeaponsSizes[id].y), uiWeaponsSizes[id].x * 2, uiWeaponsSizes[id].y * 2);
			
			//Vector3 newPos = cam.transform.forward;
			
			newWeapon = weaponGo.GetComponent<Weapon>();
			//weaponGo.transform.localPosition = newWeapon.localPos;
			if(newWeapon.sndSelect) {
				audioSource.clip = newWeapon.sndSelect;
				audioSource.Play();
			}
		}
		
		return newWeapon;
	}
	
	void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Evil") {
			health = 0;
		}
	}
	
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "pickup") {
			Pickup pickup = col.gameObject.GetComponent<Pickup>();
			
			if(pickup.sndPickup) {
				PlayAudioClip(pickup.sndPickup, col.gameObject.transform.position, 1);
			}
			
			if(pickup.type == "gold") {
				gold += pickup.amount;
			}
			if(pickup.type == "weapon") {
				Weapon weapon = pickup.prefab.GetComponent<Weapon>();
				ammo[weapon.id] += pickup.amount;
				if(selectedWeapon) {
					if(selectedWeapon.id != weapon.id && !ownedWeapons[weapon.id]) {
						ownedWeapons[weapon.id] = pickup.prefab;
						selectedWeapon = showWeapon(ownedWeapons[weapon.id]);
					}
				}else {
					ownedWeapons[weapon.id] = pickup.prefab;
					selectedWeapon = showWeapon(ownedWeapons[weapon.id]);
				}
			}
			if(pickup.type == "ammo") {
				Weapon weapon = pickup.prefab.GetComponent<Weapon>();
				ammo[weapon.id] += pickup.amount;
			}
			if(pickup.type == "health") {
				health = Mathf.Min(health + pickup.amount, maxHealth);
			}
			Destroy(col.gameObject);
		}
		
		if(col.gameObject.tag == "ExitPlane") {
			GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>().destroyLevel();
			switchingScreen = true;
			GUITexture close = GameObject.Find("EvilClose").GetComponent<GUITexture>();
			Color colour = close.color;
			colour.a = 0;
			close.color = colour;
			GameObject.Find("TussenSchermHUB").GetComponent<ScoreScreen>().showScoreScreen("ScoreScreen");
		}
		if(col.gameObject.tag == "EnemyAwareness") {
			col.gameObject.transform.parent.GetComponent<Enemy>().awareOfPlayer = true;
		}
	}
	
	public void gotHit(int damage) {
		if(health > 0) {
			int snd = Random.Range(0, sndGotHit.Length);
			Debug.Log("PLAY SOUND: " + snd.ToString());
			PlayAudioClip(sndGotHit[snd], transform.position, 1);
			armor -= damage;
			if(armor < 0) {
				health += armor;
				armor = 0;
			}
			
			Color col = GameObject.Find("gotHit").GetComponent<GUITexture>().color;
			col.a = 1;
			GameObject.Find("gotHit").GetComponent<GUITexture>().color = col;
		}
	}
	
	public void handleShopItem(ShopItem shopItem) {
		Debug.Log(shopItem.id);
		int id = shopItem.id;
		switch(id) {
		case 0 : {
			armor = Mathf.Min(armor + 1, maxArmor);
			break;
		}
		case 1 : {
			boostEnabled = true;
			break;
		}
		case 2 : {
			seeSecretsEnabled = true;
			break;
		}
		case 3 : {
			if(ownedWeapons[0]) {
				ownedWeapons[0].GetComponent<Weapon>().damage++;
			}
			if(selectedWeapon.id == 0) {
				selectedWeapon.damage++;
			}
			break;
		}
		case 4 : {
			repelDemonEnabled = true;
			break;
		}
		case 5 : {
			seeThroughEyesEnabled = true;
			break;
		}
		case 6 : {
			health = Mathf.Min(health + 1, maxHealth);
			break;
		}
		case 7 : {
			maxHealth += 1;
			health += 1;
			break;
		}
		case 8 : {
			ammo[1] += 9;
			break;
		}
		case 9 : {
			if(ownedWeapons[1]) {
				ownedWeapons[1].GetComponent<Weapon>().damage++;
			}
			if(selectedWeapon.id == 1) {
				selectedWeapon.damage++;
			}
			break;
		}
		case 10 : {
			ammo[2] += 3;
			break;
		}
		case 11 : {
			if(ownedWeapons[2]) {
				ownedWeapons[2].GetComponent<Weapon>().damage++;
			}
			if(selectedWeapon.id == 2) {
				selectedWeapon.damage++;
			}
			break;
		}
		case 12 : {
			gameObject.GetComponent<CharacterMotor>().movement.maxForwardSpeed += 2;
			gameObject.GetComponent<CharacterMotor>().movement.maxSidewaysSpeed += 2;
			gameObject.GetComponent<CharacterMotor>().movement.maxBackwardsSpeed += 2;
			break;
		}
		case 13 : {
			Weapon weapon = shopItem.prefab.GetComponent<Weapon>();
			
			if(selectedWeapon) {
				if(selectedWeapon.id != weapon.id && !ownedWeapons[weapon.id]) {
					ownedWeapons[weapon.id] = shopItem.prefab;
					selectedWeapon = showWeapon(ownedWeapons[weapon.id]);
				}
			}else {
				ownedWeapons[weapon.id] = shopItem.prefab;
				selectedWeapon = showWeapon(ownedWeapons[weapon.id]);
			}
			break;
		}
		case 14 : {
			Weapon weapon = shopItem.prefab.GetComponent<Weapon>();
			
			if(selectedWeapon) {
				if(selectedWeapon.id != weapon.id && !ownedWeapons[weapon.id]) {
					ownedWeapons[weapon.id] = shopItem.prefab;
					selectedWeapon = showWeapon(ownedWeapons[weapon.id]);
				}
			}else {
				ownedWeapons[weapon.id] = shopItem.prefab;
				selectedWeapon = showWeapon(ownedWeapons[weapon.id]);
			}
			break;
		}
		}
	}
	
    AudioSource PlayAudioClip(AudioClip clip, Vector3 position, float volume) {
        GameObject go = new GameObject("One shot audio");
        go.transform.position = position;
		go.transform.parent = transform;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
		source.minDistance = 5;
		source.panLevel = 0.5f;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }
}
