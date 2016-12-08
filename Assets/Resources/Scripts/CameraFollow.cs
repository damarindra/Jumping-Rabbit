using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public float timeMove;
	public float offset;
	
	public void CameraMove(Vector3 position){
		position.z = -10;
		position.y += offset;
		position.x = 0;
		iTween.MoveTo(gameObject, position, timeMove);
	}
	
	public void CameraFollowPlayer(Vector2 pos){
		transform.position = new Vector2(0, pos.y);
	}
}
