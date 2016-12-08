using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public CameraFollow cameraFollow;
	public GameObject player;
	private Rigidbody2D rb2dPlayer;
    [HideInInspector]
    public int score = 0;
    public Text scoreText;

    [HideInInspector]
    public int coin = 0;
    public Text coinText;
	public enum GAME_STATE{
		SETUP, PLAYING, GAMEOVER
	}
	public GAME_STATE gameState = GAME_STATE.SETUP;

	void Awake(){
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);
	}

	void Start(){
        coin = SaveLoad.GetCoin();
		rb2dPlayer = player.GetComponent<Rigidbody2D>();
		Services.ShowBanner();
	}

    void Update() {
        scoreText.text = score.ToString();
        coinText.text = coin.ToString();
		if(gameState == GAME_STATE.GAMEOVER && Input.GetKeyDown(KeyCode.Escape))
			Application.LoadLevel("MainMenu");
    }

	public void Restart(){
		rb2dPlayer.isKinematic = false;
        score = 0;
	}
}
