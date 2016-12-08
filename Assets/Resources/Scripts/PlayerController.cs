using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteSwitcher))]
public class PlayerController : MonoBehaviour {

	[TooltipAttribute("Jump Power when touch platform")]
	public float jumpPower;
	[TooltipAttribute("Scaling the gravity")]
	public float gravityScale;
	[TooltipAttribute("Speed moving at X direction")]
	public float speedX;

	[TooltipAttribute("For storing width of Player, used for offset")]
	private float width;

	[TooltipAttribute("Rigidbody")]
	private Rigidbody2D rb2d;
	[TooltipAttribute("Box Collider 2D")]
	private BoxCollider2D bc2d;

	[TooltipAttribute("For Switching sprite, when jump, down, or get hitß")]
	private SpriteSwitcher sprSwitch;

	[TooltipAttribute("Accesing CameraFollow Script, for moving Main Camera")]
	private CameraFollow camFollow;
	
	[TooltipAttribute("set True if you got an Power Up")]
	private bool gotPowers = false;
	[TooltipAttribute("set True if you got spring")]
	private bool springed = false;
	private Vector2 originPos;
    private bool isDie;
	
	public GameObject rocket;

	public AudioClip jumpClip;
	public AudioClip dieClip;

	void Awake(){
		rb2d = GetComponent<Rigidbody2D>();
		bc2d = GetComponent<BoxCollider2D>();
		sprSwitch = GetComponent<SpriteSwitcher>();
		camFollow = Camera.main.GetComponent<CameraFollow>();
	}

	// Use this for initialization
	void Start () {
		originPos = transform.position;
		bc2d.isTrigger = true;
		rb2d.gravityScale *= gravityScale;
		width = (GetComponent<SpriteRenderer>().sprite.rect.width / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit)/2;
		if(SaveLoad.isStartWithRocket){
			gotPowers = true;
			StartCoroutine(Rockets(rocket.GetComponent<Rocket>().rocketForce, rocket.GetComponent<Rocket>().timeRockets));
			Instantiate(rocket, Vector2.zero, Quaternion.identity);
			SaveLoad.isStartWithRocket = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Move();
		if(GameManager.instance.gameState == GameManager.GAME_STATE.PLAYING){
			if(gotPowers || springed){
				Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0, transform.position.y, -10), .2f);
			}
			if(transform.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize - 2){
				EventCanvas.instance.GameOverTransition();
				transform.position = originPos;
				rb2d.isKinematic = true;
				GameManager.instance.gameState = GameManager.GAME_STATE.GAMEOVER;
			}
            CalculateScore();
		}
		else{
			transform.position = originPos;
			rb2d.isKinematic = true;
			
		}
	}

	void FixedUpdate(){
		if(GameManager.instance.gameState == GameManager.GAME_STATE.PLAYING){
			if(rb2d.velocity.y < 15){
				//bc2d.enabled = true;
				sprSwitch.SwitchSprite(1);
				gotPowers = false;
				springed = false;
			}
		}
	}

	/// <summary>
	/// Checking contact between the player
	/// </summary>
	void OnTriggerEnter2D(Collider2D col){
        if (!isDie) {
            if (rb2d.velocity.y < 0 && col.tag.Equals("Platform"))
            {
                Jump();
                camFollow.CameraMove(col.gameObject.transform.position);
            }
            else if (rb2d.velocity.y < 0 && col.tag.Equals("PlatformDestrucable"))
            {
                Jump();
                camFollow.CameraMove(col.gameObject.transform.position);
                col.gameObject.GetComponent<DestructableLand>().Destruct();
            }
            else if (rb2d.velocity.y < 5 && col.tag.Equals("Cloud"))
            {
                Jump(col.GetComponent<CloudEnemy>().jumpPower);
                camFollow.CameraMove(col.gameObject.transform.position);
            }
            else if (rb2d.velocity.y < 0 && col.tag.Equals("Spring"))
            {
                Jump(col.GetComponent<Spring>().jumpForce);
                col.GetComponent<Spring>().Springed();
                springed = true;
            }
            else if (rb2d.velocity.y < 0 && col.tag.Equals("SpikeBuddy"))
            {
                sprSwitch.SwitchSprite(2);
                StartCoroutine(Die());
                Invoke("SetToGameOver", 1.5f);
                //GameManager.instance.gameState = GameManager.GAME_STATE.GAMEOVER;
            }
            else if (rb2d.velocity.y < 5 && col.tag.Equals("SpikeUp")) {
                sprSwitch.SwitchSprite(2);
                StartCoroutine(Die());
                Invoke("SetToGameOver", 1.5f);
            }
            else if (!gotPowers && rb2d.velocity.y > 0 && col.tag.Equals("SpikeBottom"))
            {
                sprSwitch.SwitchSprite(2);
                StartCoroutine(Die());
                Invoke("SetToGameOver", 1.5f);
            }
            else if (col.tag.Equals("RocketItem"))
            {
                if (!gotPowers)
                {
                    gotPowers = true;
                    StartCoroutine(Rockets(col.GetComponent<RocketItem>().rocket.GetComponent<Rocket>().rocketForce, col.GetComponent<RocketItem>().rocket.GetComponent<Rocket>().timeRockets));
                    col.enabled = false;
                    Instantiate(col.GetComponent<RocketItem>().rocket, Vector2.zero, Quaternion.identity);
                }
                iTween.ColorTo(col.gameObject, Color.clear, .2f);
            }
            else if (!gotPowers && col.tag.Equals("Lightning"))
            {
                sprSwitch.SwitchSprite(2);
                StartCoroutine(Die());
                Invoke("SetToGameOver", 1.5f);
                //GameManager.instance.gameState = GameManager.GAME_STATE.GAMEOVER;
            }
            else if (col.tag.Equals("Coin"))
            {
                if (gotPowers || springed)
                {
                    //Do Nothing
                }
                else
                {
                    Jump();
                    camFollow.CameraMove(col.gameObject.transform.position);
                }
                GameManager.instance.coin += 1;
                col.GetComponent<Coin>().GotPoint();
				if(GameManager.instance.coin >= 50 && PlayerPrefs.GetInt("CoinAccUnlocked", 0) == 0){
					Services.UnlockingAchievement(Services.fiftyCoinsID);
					PlayerPrefs.SetInt("CoinAccUnlocked",1);
				}

            }
        }
	}

    void SetToGameOver() {
        GameManager.instance.gameState = GameManager.GAME_STATE.GAMEOVER;
    }

	/// <summary>
	/// Jump
	/// </summary>
	void Jump(){
		//bc2d.enabled = false;
		SoundManager.instance.PlaySingle(jumpClip);
		sprSwitch.SwitchSprite(0);
		Vector2 velocity = rb2d.velocity;
		velocity.y = jumpPower;
		rb2d.velocity = velocity;
		Services.IncrementAchievement(Services.twoHundredJumpsID);
		Services.IncrementAchievement(Services.fiftyHundredJumpsID);
	}
	/// <summary>
	/// Jump
	/// </summary>
	void Jump(float spd){
		SoundManager.instance.PlaySingle(jumpClip);
		//bc2d.enabled = false;
		sprSwitch.SwitchSprite(0);
		Vector2 velocity = rb2d.velocity;
		velocity.y = spd;
		rb2d.velocity = velocity;
		Services.IncrementAchievement(Services.twoHundredJumpsID);
		Services.IncrementAchievement(Services.fiftyHundredJumpsID);
	}
	/// <summary>
	/// Rocketed
	/// </summary>
	IEnumerator Rockets(float spd, float time){
		while(true){
			Vector2 velocity = rb2d.velocity;
			velocity.y = spd;
			rb2d.velocity = velocity;
			if(time > 0)
				time -= Time.deltaTime;
			else
				break;
			yield return null;
		}
		StopCoroutine("Rockets");
	}

	/// <summary>
	/// Move
	/// </summary>
	void Move(){
        if (!isDie) {
		    Vector2 velocity = rb2d.velocity;
		    float movementX;
    #if UNITY_EDITOR
		    movementX = Input.GetAxis("Horizontal");
    #elif UNITY_ANDROID || UNITY_IOS
		    movementX = Input.acceleration.x * 2.6f;
	#else
		    movementX = Input.GetAxis("Horizontal");
    #endif
		    Vector2 windowSize = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
		    velocity.x = movementX * speedX;
		    if(transform.position.x < -(windowSize.x + width))
			    transform.position = new Vector2((transform.position.x * -1) - width/2, transform.position.y);
		    else if(rb2d.position.x > windowSize.x + width)
			    transform.position = new Vector2((transform.position.x * -1) + width/2, transform.position.y);
			
		    rb2d.velocity = velocity;
        }
	}
    /// <summary>
    /// Player Die
    /// </summary>
    IEnumerator Die() {
        isDie = true;
        SaveLoad.SaveCoin(GameManager.instance.coin);
        SaveLoad.SaveScore(GameManager.instance.score);
        rb2d.velocity = new Vector2(Random.Range(-8, 8), 4);
		SoundManager.instance.PlaySingle(dieClip);
		EventCanvas.instance.GameOverTransition();
		Services.CountingInterstitialShow();
		Services.FlushAchievement();
		Services.PostScoreLeaderboard(GameManager.instance.score);
    loop:
        //iTween.RotateAdd(gameObject, new Vector3(0, 0, 40), .1f);
        //yield return new WaitForSeconds(.1f);
        transform.Rotate(new Vector3(0, 0, 5));
        yield return new WaitForEndOfFrame();

        goto loop;
    }

    int highestYPos;
    void CalculateScore() {
        highestYPos = (int)(transform.position.y + (originPos.y * -1));
        if (GameManager.instance.score < highestYPos) {
            GameManager.instance.score = highestYPos;
        }
    }
}
