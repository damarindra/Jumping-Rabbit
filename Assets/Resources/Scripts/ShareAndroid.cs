using UnityEngine;
using System.Collections;

public class ShareAndroid : MonoBehaviour {

	public static ShareAndroid mInstance = new ShareAndroid();
	
	public static ShareAndroid Instance{
		get{return mInstance;}
	}
	
	private string url = "https://play.google.com/store/apps/details?id=com.cgranule.jumpingrabits";

	public void TakeScreenshot(){
		string path = System.IO.Path.Combine(Application.persistentDataPath, "share.png");
		Application.CaptureScreenshot("share.png");
	}
	
	public IEnumerator Share(){
		yield return new WaitForEndOfFrame();
		#if UNITY_ANDROID
		TakeScreenshot();
		
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		intentObject.Call<AndroidJavaObject>("setType", "image/*");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "I've got " + GameManager.instance.score.ToString() + "!!! Can you beat it?");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "I've got " + GameManager.instance.score.ToString() + "!!! Can you beat it?");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Play now at " +url);
		
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaClass fileClass = new AndroidJavaClass("java.io.File");
		
		AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", Application.persistentDataPath + "/share.png");// Set Image Path Here
		
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);
		
		//			string uriPath =  uriObject.Call<string>("getPath");
		bool fileExist = fileObject.Call<bool>("exists");
		Debug.Log("File exist : " + fileExist);
		if (fileExist)
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
		
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		currentActivity.Call("startActivity", intentObject);
		#endif
	}
}
