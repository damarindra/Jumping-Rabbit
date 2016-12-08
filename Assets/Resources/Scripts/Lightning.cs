using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {

	public float lengthShoot;
	public float timeNeeded;
	// Use this for initialization
	void Start () {
		iTween.MoveAdd(gameObject, new Vector3(0, -lengthShoot, 0), timeNeeded);
		Invoke("DestroyThis", timeNeeded);
	}
	
	void DestroyThis(){
		Destroy(gameObject);
	}
}
