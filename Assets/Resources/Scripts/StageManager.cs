using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageManager : MonoBehaviour {
	
	[TooltipAttribute("Singleton")]
	public static StageManager instance = null;

	[TooltipAttribute("Max platform / coin to be created")]
	public int maxStageCreationLength;
	[TooltipAttribute("Platform Safe")]
	public GameObject[] platformSafe;
	[Range(0,1)]
	[TooltipAttribute("Probailty Platform Safe")]
	public float platformSafeProbability;
	
	[TooltipAttribute("Platform Spring")]
	public GameObject spring;
	[Range(0,1)]
	[TooltipAttribute("Probability Platform Spring")]
	public float springProbability;

    [TooltipAttribute("Rocket Items")]
    public GameObject rocketItem;
    [Range(0, 1)]
    [TooltipAttribute("Probability Platform Spring")]
    public float rocketItemProbability;

    [TooltipAttribute("Spike Top")]
    public GameObject[] spikeTop;
    [Range(0, 1)]
    [TooltipAttribute("Probability Spike Top")]
    public float spikeTopProbability;

    [TooltipAttribute("Spike Bottom")]
    public GameObject[] spikeBottom;
    [Range(0, 1)]
    [TooltipAttribute("Probability Spike Bottom")]
    public float spikeBottomProbability;

    [TooltipAttribute("Enemy Spike")]
	public GameObject enemySpike;
	[Range(0,1)]
	[TooltipAttribute("Probability Enemy Spike")]
	public float enemySpikeProbability;

    [TooltipAttribute("Platform Destrucable")]
	public GameObject platformDestrucable;
	[Range(0,1)]
	[TooltipAttribute("Probability Platform Destrucable")]
	public float platformDestrucableProbability;
	
	
	[TooltipAttribute("Cloud enemy")]
	public GameObject cloud;
	[Range(0,1)]
	[TooltipAttribute("Probability Cloud Enemy")]
	public float cloudProbability;

	[TooltipAttribute("Coin Group")]
	public GameObject[] coinGroup;

	[TooltipAttribute("Window size")]
	private Vector2 windowSize;
	private float lastYPlatform;
	[TooltipAttribute("Gap Per Platform")]
	public float gapPlatformMin, gapPlatformMax;
	[TooltipAttribute("When platform spawn has reach platform per stage, then spawn prefabs coins")]
	public int platformPerStageMin, platformPerStageMax;
	[TooltipAttribute("For checking if we must spawn coin then.")]
	private int platformPerStage;
	[TooltipAttribute("Counting how many platform has been spawn")]
	private int platformCounter;

	private List<GameObject> objectPooling = new List<GameObject>();
	private Transform stageParent;
	void Awake(){
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);
	}

	void Start(){
		windowSize = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
		stageParent = new GameObject("StageParent").transform;
		SetupStage();
		GameManager.instance.gameState = GameManager.GAME_STATE.PLAYING;
	}
	
	void FixedUpdate(){
		if(GameManager.instance.gameState == GameManager.GAME_STATE.PLAYING)
			CreateStage();
	}

	void Update(){
		if(GameManager.instance.gameState == GameManager.GAME_STATE.GAMEOVER){
			if(GameManager.instance.gameState == GameManager.GAME_STATE.GAMEOVER){
				foreach(Transform obj in stageParent.Cast<Transform>().ToArray()){
					iTween.ColorTo(obj.gameObject, Color.clear, .4f);
					Invoke("DestroyStage",.5f);
				}
			}
		}
	}

	GameObject CreateCoin(Vector2 place){
		GameObject go = coinGroup[Random.Range(0, coinGroup.Length)];
		return Instantiate(go, place, Quaternion.identity) as GameObject;
	}

	/// <summary>
	/// Create stage full of screen size y
	/// Per stage Have max 'x' Platform, Per platform have max height gap. 1 Screen / height Gap > get the range between each platform
	/// </summary>
	void CreateStage(){
		if(objectPooling.Count < maxStageCreationLength){
			float yPos = lastYPlatform + Random.Range(gapPlatformMin, gapPlatformMax);
			float xPos = Random.Range(-windowSize.x, windowSize.x);
			Vector2 newPos = new Vector2(xPos, yPos);
			GameObject obj;
			//Create Coin Group, When platform per stage has reached
			if(platformCounter % platformPerStage == 0 && platformCounter != 0){
				//Spawn Coin
				newPos.x = 0;
				obj = CreateCoin(newPos);
				platformPerStage = Random.Range(platformPerStageMin, platformPerStageMax);
				lastYPlatform = yPos + obj.GetComponent<CoinGroup>().lengthGroup;
			}
			else{
				//Spawn Platform
				float rand = Random.Range(0, platformSafeProbability + springProbability + platformDestrucableProbability);
				if(rand < platformSafeProbability){
					obj = Instantiate(platformSafe[Random.Range(0, platformSafe.Length)], newPos, Quaternion.identity) as GameObject;
					//Randoming value to spawn enemy, between probability
					//Randoming value to select what next things on land
					float nothingChance = (5 - (enemySpikeProbability + springProbability + rocketItemProbability + spikeTopProbability + spikeBottomProbability))/5;
					float randLandThings = Random.Range(0, nothingChance + enemySpikeProbability + springProbability + rocketItemProbability + spikeTopProbability + spikeBottomProbability);
                    //float randEnemy = Random.Range(0.0f, 1.0f);
                    if (randLandThings < enemySpikeProbability)
                    {
                        GameObject go = Instantiate(enemySpike
                                , new Vector2(newPos.x, newPos.y + (obj.GetComponent<SpriteRenderer>().sprite.rect.height / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2)
                                 , Quaternion.identity) as GameObject;
                        go.GetComponent<SpikeBuddy>().setgetLengthLand = obj.GetComponent<SpriteRenderer>().sprite.rect.width / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
                        go.transform.SetParent(stageParent);
                    }
                    else if (randLandThings < enemySpikeProbability + springProbability)
                    {
                        GameObject go = Instantiate(spring
                                , new Vector2(Random.Range(newPos.x - (obj.GetComponent<SpriteRenderer>().sprite.rect.width / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2,
                                                                        newPos.x + (obj.GetComponent<SpriteRenderer>().sprite.rect.width / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2)
                                            , newPos.y + (obj.GetComponent<SpriteRenderer>().sprite.rect.height / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2)
                                 , Quaternion.identity) as GameObject;
                        go.transform.SetParent(stageParent);
                    }
                    else if (randLandThings < enemySpikeProbability + springProbability + rocketItemProbability) {
                        GameObject go = Instantiate(rocketItem
                                , new Vector2(Random.Range(newPos.x - (obj.GetComponent<SpriteRenderer>().sprite.rect.width / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2,
                                                                        newPos.x + (obj.GetComponent<SpriteRenderer>().sprite.rect.width / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2)
                                            , newPos.y + (obj.GetComponent<SpriteRenderer>().sprite.rect.height / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2 + 1)
                                 , Quaternion.identity) as GameObject;
                        go.transform.SetParent(stageParent);
                    }
                    else if (randLandThings < enemySpikeProbability + springProbability + rocketItemProbability + spikeTopProbability)
                    {
                        GameObject g = spikeTop[Random.Range(0, spikeTop.Length)];
                        GameObject go = Instantiate(g
                                , new Vector2(Random.Range(newPos.x - ((obj.GetComponent<SpriteRenderer>().sprite.rect.width / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2) + (g.GetComponent<SpriteRenderer>().sprite.rect.width / g.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2,
                                                                        newPos.x + (obj.GetComponent<SpriteRenderer>().sprite.rect.width / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2 - (g.GetComponent<SpriteRenderer>().sprite.rect.width / g.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2)
                                            , newPos.y + (obj.GetComponent<SpriteRenderer>().sprite.rect.height / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2)
                                 , Quaternion.identity) as GameObject;
                        go.transform.SetParent(stageParent);
                    }
                    else if (randLandThings < enemySpikeProbability + springProbability + rocketItemProbability + spikeTopProbability + spikeBottomProbability)
                    {
                        GameObject g = spikeBottom[Random.Range(0, spikeBottom.Length)];
                        GameObject go = Instantiate(g
                                , new Vector2(Random.Range(newPos.x - ((obj.GetComponent<SpriteRenderer>().sprite.rect.width / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2 ) + (g.GetComponent<SpriteRenderer>().sprite.rect.width / g.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit)/2,
                                                                        newPos.x + (obj.GetComponent<SpriteRenderer>().sprite.rect.width / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2 - (g.GetComponent<SpriteRenderer>().sprite.rect.width / g.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2)
                                            , newPos.y - (obj.GetComponent<SpriteRenderer>().sprite.rect.height / obj.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) / 2)
                                 , Quaternion.identity) as GameObject;
                        go.transform.SetParent(stageParent);
                    }
                }
				else if(rand < platformSafeProbability + cloudProbability){
					obj = Instantiate(cloud, newPos, Quaternion.identity) as GameObject;
				}
				else{
					obj = Instantiate(platformDestrucable, newPos, Quaternion.identity) as GameObject;
					
				}
				lastYPlatform = yPos;
				objectPooling.Add(obj);
								
			}
			obj.transform.SetParent(stageParent);
			platformCounter+=1;
		}
		else{
			DestroyEachObj();
		}
	}
	
	void DestroyEachObj(){
		foreach(GameObject obj in objectPooling.ToArray()){
			if(obj.transform.position.y < Camera.main.transform.position.y - windowSize.y - 1.5f){
				//Destroy
				objectPooling.Remove(obj);
				Destroy(obj);
			}
		}
	}
	public void SetupStage(){
		lastYPlatform = -8;
		platformPerStage = Random.Range(platformPerStageMin, platformPerStageMax);

		/*
		foreach(GameObject obj in objectPooling.ToArray()){
			//objectPooling.Remove(obj);
			//Destroy(obj);
			iTween.ColorTo(obj, Color.clear, .4f);
			Invoke("DestroyStage",.5f);
		}*/
	}
	void DestroyStage(){
		objectPooling.Clear();
		Destroy(stageParent.gameObject);
		stageParent = new GameObject("StageParent").transform;
		
	}
}
