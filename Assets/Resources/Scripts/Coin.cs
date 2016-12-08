using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Coin : MonoBehaviour {

	public AudioClip coinClip;

	public void GotPoint(){
		SoundManager.instance.PlaySingle(coinClip);
		GetComponent<CircleCollider2D>().enabled = false;
		iTween.ColorTo(gameObject, Color.clear, .2f);
	}
}
