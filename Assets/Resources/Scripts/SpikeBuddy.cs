using UnityEngine;
using System.Collections;

public class SpikeBuddy : MonoBehaviour {

	[TooltipAttribute("Using for moving spikeBuddy")]
	private float lengthLand;
	[TooltipAttribute("Using for Offset")]
	public float moveSpeed;
	private Vector2 originPos;
	// Use this for initialization
	void Start () {
		originPos = transform.position;
		StartCoroutine(Move());
	}
	
	IEnumerator Move(){
		loop:
		Vector2 pos = transform.position;
		Vector2 localScale = transform.localScale;
		if(pos.x >= originPos.x + lengthLand/2){
			moveSpeed = -(Mathf.Abs(moveSpeed));
			localScale.x = -(Mathf.Abs(localScale.x));
		}
		else if(pos.x <= originPos.x - lengthLand/2){
			moveSpeed = Mathf.Abs(moveSpeed);
			localScale.x = (Mathf.Abs(localScale.x));
		}
		pos.x = pos.x + moveSpeed * Time.deltaTime;
		transform.position = pos;
		transform.localScale = localScale;
		yield return new WaitForEndOfFrame();
		if(transform.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize -2)
			Destroy(gameObject);
		goto loop;
	}
	
	public float setgetLengthLand{
		set{lengthLand = value;}
		get{return lengthLand;}
	}
	
}
