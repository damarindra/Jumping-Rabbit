using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventCanvas : MonoBehaviour {
	public static EventCanvas instance = null;

    public Image panelTransition;
	public Text soundText;
	public Text againText;
	public Text shareText;
	public Text achievementText;
	public Text leaderboardText;
	public GameObject powerUpMenu;

	public AudioClip clipButton;

	void Awake(){
		if(instance == null)
			instance = this;
	}

    void Start() {
        StartCoroutine(PanelTransitionOut());
		if(Application.loadedLevelName == "MainMenu"){
			SaveLoad.Initialize();
			Services.SetupGPGS();
			Services.BannerSetup();
			Services.InterstitialSetup();
			soundText.text = (SaveLoad.Sound == true ? "Sound : On" : "Sound : Off");
		}
		else if(Application.loadedLevelName == "Game"){
			againText.gameObject.SetActive(false);
			shareText.gameObject.SetActive(false);
			powerUpMenu.SetActive(false);
			achievementText.gameObject.SetActive(false);
			leaderboardText.gameObject.SetActive(false);
		}
    }

	public void GameOverTransition(){
		againText.gameObject.SetActive(true);
		shareText.gameObject.SetActive(true);
		//againText.CrossFadeAlpha(1, .5f,true);
		powerUpMenu.SetActive(true);
		achievementText.gameObject.SetActive(true);
		leaderboardText.gameObject.SetActive(true);
	}

    IEnumerator PanelTransitionOut() {
        panelTransition.gameObject.SetActive(true);
        Color color = panelTransition.color;
        color.a = 1;
        panelTransition.color = color; 
    loop:
        color.a -= .08f;
        panelTransition.color = color;
        if (color.a <= 0)
        {
            StopCoroutine(PanelTransitionOut());
            goto end;
        }
        yield return new WaitForEndOfFrame();
        goto loop;
    end:
        panelTransition.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator PanelTransitionIn()
    {
        panelTransition.gameObject.SetActive(true);
        Color color = panelTransition.color;
        color.a = 0;
        panelTransition.color = color;
    loop:
        color.a += .08f;
        panelTransition.color = color;
        if (color.a >= 1) {
            StopCoroutine(PanelTransitionIn());
            goto end;
        }
        yield return new WaitForEndOfFrame();
        goto loop;
        end:
        yield return new WaitForEndOfFrame();

    }

    public void AgainButton(){
		/*StageManager.instance.SetupStage();
		GameManager.instance.Restart();
		iTween.MoveTo(Camera.main.gameObject, GameObject.FindGameObjectWithTag("Player").transform.position, .5f);
		Invoke("SetToPlaying", .6f);*/
		SoundManager.instance.PlaySingle(clipButton);
		Application.LoadLevel(Application.loadedLevel);
	}
    public void PlayButton() {
		SoundManager.instance.PlaySingle(clipButton);
		Services.UnlockingAchievement(Services.firstPlayID);
		Services.IncrementAchievement(Services.fiftyPlayID);
        StartCoroutine(LoadGameScene());
    }
	public void ExitButton(){
		Application.Quit();
	}
	public void ToggleSound(){
		//Toggle the preferences
		SaveLoad.Sound = (SaveLoad.Sound == true ? false : true);
		if(SaveLoad.Sound)
			soundText.text = "Sound : On";
		else
			soundText.text = "Sound : Off";
		SoundManager.instance.PlaySingle(clipButton);
		
	}
	public void BuyRockets(){
		if(GameManager.instance.coin >= 50 && !SaveLoad.isStartWithRocket){
			GameManager.instance.coin -= 50;
			SaveLoad.SaveCoin(GameManager.instance.coin);
			SaveLoad.isStartWithRocket = true;
			SoundManager.instance.PlaySingle(clipButton);
			Services.UnlockingAchievement(Services.buyRocketID);
		}
	}
	public void Share(){
		ShareAndroid.Instance.Share();
	}
	public void AchievementShow(){
		Services.ShowAchievement();
	}
	public void LeaderboardShow(){
		Services.ShowLeaderboard();
	}

    IEnumerator LoadGameScene() {
        StartCoroutine(PanelTransitionIn());
        AsyncOperation async = Application.LoadLevelAsync("Game");
        yield return async;    
    }
    void SetToPlaying(){
		GameManager.instance.gameState = GameManager.GAME_STATE.PLAYING;
	}
}