using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteSwitcher))]
public class Spring : MonoBehaviour {
	public float jumpForce;
	public void Springed(){
		GetComponent<SpriteSwitcher>().Play();
	}
	void LateUpdate(){
		if(transform.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize -2)
			Destroy(gameObject);
	}
}
