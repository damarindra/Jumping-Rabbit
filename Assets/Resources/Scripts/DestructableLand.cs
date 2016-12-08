using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class DestructableLand : MonoBehaviour {
	
	public void Destruct(){
		GetComponent<BoxCollider2D>().enabled = false;
		GetComponent<ParticleSystem>().Play();
		iTween.ColorTo(gameObject, Color.clear, .5f);
	}
}
