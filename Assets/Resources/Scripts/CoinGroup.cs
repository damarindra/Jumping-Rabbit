using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CoinGroup : MonoBehaviour {
	
	[HideInInspector]
	public Vector2 startPosition;
	
	public Transform lastTransform;
	[HideInInspector]
	public Vector2 lastPosition;
	[HideInInspector]
	public float lengthGroup;

	private List<Transform> children;
	// Use this for initialization
	void Awake () {
		startPosition = transform.position;
		lastPosition = lastTransform.position;
		lengthGroup = lastPosition.y - startPosition.y;
		children = transform.Cast<Transform>().ToList();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		CoinDisposer();
	}
	
	void CoinDisposer(){
		foreach(Transform tr in children.ToArray()){
			if(tr.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize - 2){
				children.Remove(tr);
				Destroy(tr.gameObject);
			}
		}
		if(children.Count == 0)
			Destroy(gameObject);
	}
}
