using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {

	public float rocketForce;
	public float timeRockets;
	public AudioClip getItemClip;
	float timer = 0;
	// Use this for initialization
	void Start () {
		SoundManager.instance.PlaySingle(getItemClip);
	}
	
	bool animated = false;
	void Update(){
		timer += Time.deltaTime;
		Debug.Log(timer);
		if(timer > timeRockets + 1.5f){
			Destroy(gameObject);
		}
		else if(timer > timeRockets){
			if( !animated){
				bool left = Random.Range(0,2) == 0 ? true : false;
				if(left){
					iTween.RotateAdd(gameObject, new Vector3(0,0, Random.Range(20, 130)), 1.5f);
					iTween.MoveAdd(gameObject, new Vector3(Random.Range(1.0f, 4.0f), -Random.Range(1.0f, 3.0f), 0), 1.5f);				
				}
				else{
					iTween.RotateAdd(gameObject, new Vector3(0,0, -Random.Range(20, 130)), 1.5f);					
					iTween.MoveAdd(gameObject, new Vector3(-(Random.Range(1.0f, 4.0f)), -Random.Range(1.0f, 3.0f), 0), 1.5f);				
				}
			}
			animated = true;
		}
		else
			transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
		Debug.Log(timeRockets + 1.5f);
	}
}