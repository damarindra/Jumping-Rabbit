using UnityEngine;
using System.Collections;

public class CloudEnemy : MonoBehaviour {

	public GameObject lightning;
	public float attackTime;
	
	private float width;
	
	public float jumpPower;

	// Use this for initialization
	void Start () {
		width = GetComponent<SpriteRenderer>().sprite.rect.width / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
		StartCoroutine(Attack(attackTime));
	}
	
	IEnumerator Attack(float time){
		loop:
		yield return new WaitForSeconds(time);
		GameObject go = Instantiate(lightning, new Vector3(Random.Range(transform.position.x - width/2, transform.position.x + width/2),transform.position.y,transform.position.z), Quaternion.identity) as GameObject;
		go.transform.SetParent(transform);
		goto loop;
	}
}
