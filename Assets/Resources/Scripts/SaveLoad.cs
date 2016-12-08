using UnityEngine;
using System.Collections;

public class SaveLoad : MonoBehaviour {

	public static void Initialize(){
		if(PlayerPrefs.GetInt("Init", 0) == 0){
			PlayerPrefs.SetInt("Sound", 1);
		}
		PlayerPrefs.SetInt("Init", 1);
	}

    public static void SaveScore(int score) {
        if(score < GetScore())
            PlayerPrefs.SetInt("Score", score);
    }

    public static int GetScore() {
        return PlayerPrefs.GetInt("Score", 0);
    }

    public static void SaveCoin(int coin)
    {
        PlayerPrefs.SetInt("Coin", coin);
    }

    public static int GetCoin()
    {
        return PlayerPrefs.GetInt("Coin", 0);
    }

	public static bool Sound{
		get{SoundManager.instance.backgroundMusic.mute = PlayerPrefs.GetInt("Sound") == 1 ? false : true;
			return PlayerPrefs.GetInt("Sound") == 1 ? true : false;}
		set{PlayerPrefs.SetInt("Sound", value == true ? 1 : 0);}
	}
	public static bool isStartWithRocket{
		get{return PlayerPrefs.GetInt("RocketStart",0) == 1 ? true : false;}
		set{PlayerPrefs.SetInt("RocketStart", value == true ? 1 : 0);}
	}
}
