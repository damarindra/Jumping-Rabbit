using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwitcher : MonoBehaviour {

	public Sprite[] sprite;

	private SpriteRenderer sr;

	public bool playAnimationOnAwake;
	[TooltipAttribute("Only for PING_PONG")]
	public bool flip;
	[TooltipAttribute("Only for PING_PONG")]
	public bool isLooping;
	public enum PLAY_TYPE{
		PING_PONG, LOOP
	}
	public PLAY_TYPE type;
	public float timeAnimation;

	void Awake(){
		sr = GetComponent<SpriteRenderer>();
	}

	void Start(){
		if(playAnimationOnAwake)
			StartCoroutine(PlayAnimationSprite());
	}

	public void SwitchSprite(int index){
		sr.sprite = sprite[index];
	}

	IEnumerator PlayAnimationSprite(){
		int index = 0;
		bool playBack = false;
	loop:
		switch(type){
		case PLAY_TYPE.PING_PONG:
			yield return new WaitForSeconds(timeAnimation);
			if(index + 1 > sprite.Length - 1 || index - 1 < 0){
				if(flip){
					Vector2 local = transform.localScale;
					local.x *= -1;
					transform.localScale = local;
				}
			}
			playBack = index + 1 > sprite.Length - 1 ? true : index - 1 < 0 ? false : playBack;
			if(playBack){
				index -= 1;
				if(!isLooping && index == 0){
					StopCoroutine(PlayAnimationSprite());	
					break;
				}
			}
			else
				index += 1;
			sr.sprite = sprite[index];
			goto loop;
		case PLAY_TYPE.LOOP:
			yield return new WaitForSeconds(timeAnimation);
			index = index + 1 > sprite.Length - 1? 0 : index+1;
			sr.sprite = sprite[index];
			goto loop;
			
		}
	}
	
	public void Play(){
		StartCoroutine(PlayAnimationSprite());
	}
}
