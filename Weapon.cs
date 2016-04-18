using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	public int id;
	
	public int damage;
	public int range;
	
	public float reloadTime;
	public float attackSpeed;
	public float maxAttackSpeed;
	
	public Vector3 localPos;
	public Vector3 localAniPos;
	
	float animateAttackTime = 0;
	float animateAttackTimeMax = 10;
	
	public AudioClip sndSelect;
	public AudioClip sndShoot;
	public AudioClip sndEmpty;
	
	public SpriteManager spriteManager;
	public Sprite weaponSprite;
	
	public GameObject hitPrefab;
	
	// Use this for initialization
	void Start () {
		// 0.3 -0.15 0.6 CROWBAR
		// 0 -0.15  PISTOL
		
	}
	
	// Update is called once per frame
	void Update () {
//        if (Input.GetKeyDown("y")) 
//			weaponSprite.PlayAnim("idle");
//        if (Input.GetKeyDown("u")) 
//			weaponSprite.PlayAnim("shoot");
		
		if(weaponSprite.isPlaying != "shoot") {
			weaponSprite.PlayAnim("idle");
		}
		
		if(attackSpeed > 0) {
			attackSpeed -= Time.deltaTime * 100;
		}
		
		if(animateAttackTime > 0) {
			animateAttackTime -= Time.deltaTime * 100;
			transform.localPosition = localAniPos;
			if(animateAttackTime < 0) {
				transform.localPosition = localPos;
			}
		}
	}
	
	public void OnAnimateComplete(string name) {
		weaponSprite.isPlaying = "";
	}
	
	public virtual bool attack(Vector3 pos, Vector3 dir) {
		if(attackSpeed <= 0) {
			RaycastHit hit;
			GameObject target;
			GameObject hitGo = null;
			if(Physics.Raycast (pos, dir, out hit, range, 1 << 13)) {
				Debug.DrawLine(pos, hit.point);
				target = hit.transform.gameObject;
				Debug.Log(target.name);
				if(hitPrefab) {
					hitGo = Instantiate(hitPrefab, hit.point, transform.rotation) as GameObject;
					hitGo.GetComponent<SpriteParticle>().startPos = hit.point;
				}
				if(target.tag == "enemy") {
					target.GetComponent<Enemy>().health -= damage;
				}
			}
			if(Physics.Raycast (pos, dir, out hit, range)) {
				target = hit.transform.gameObject;
				if(hitGo == null && hitPrefab) {
					hitGo = Instantiate(hitPrefab, hit.point, transform.rotation) as GameObject;
					hitGo.GetComponent<SpriteParticle>().startPos = hit.point;
				}
				if(target.tag == "breakable") {
					target.GetComponent<Breakable>().health -= damage;
				}
			}
			attackSpeed = maxAttackSpeed;
			animateAttackTime = animateAttackTimeMax;
			weaponSprite.PlayAnim("shoot");
			return true;
		}
		return false;
	}
}
