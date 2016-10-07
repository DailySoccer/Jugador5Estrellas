using UnityEngine;
using System.Collections;

public class Appsflyermanager : MonoBehaviour {

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	public const string DEV_KEY = "8u6EnwrydmGbjY7SJTSTuc";
	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods
	public static void Init()
	{
		if (_instance == null)
		{
			GameObject appflyerRef = new GameObject("AppsFlyerTrackerCallbacks");
			DontDestroyOnLoad(appflyerRef);
			appflyerRef.AddComponent<AppsFlyerTrackerCallbacks>();
			_instance = appflyerRef.AddComponent<Appsflyermanager>();
		}
	}
	#endregion  //End public methods

	//-----------------------------------------------------------//
	//                  MONOBEHAVIOUR METHODS                    //
	//-----------------------------------------------------------//
	#region Monobehaviour methods

	// Use this for initialization
	void Start () {
		//Mandatory - set your AppsFlyer’s Developer key.
		AppsFlyer.setAppsFlyerKey(DEV_KEY);
		//CAREFULL WARNING ATTENTION
		//to test with iOS you have to provide dev keys by code
		// For detailed logging
		//AppsFlyer.setIsDebug (true);
#if UNITY_IOS
		//Mandatory - set your apple app ID
		//CAREFULL WARNING ATTENTION
		AppsFlyer.setAppID ("com.mahou.jugador5estrellas");
		AppsFlyer.trackAppLaunch ();
#elif UNITY_ANDROID
		//Mandatory - set your Android package name
		AppsFlyer.init(DEV_KEY);
		AppsFlyer.setAppID ("com.mahou.jugador5estrellas");
		AppsFlyer.loadConversionData("AppsFlyerTrackerCallbacks","didReceiveConversionData", "didReceiveConversionDataWithError");
#endif
		AppsFlyer.setCustomerUserID(SystemInfo.deviceUniqueIdentifier);
		AppsFlyer.trackAppLaunch();
	}

	// Update is called once per frame
#if UNITY_IOS
	bool tokenSent = false;
#endif
	void Update () {
#if UNITY_IOS
		if (!tokenSent) {
           byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;           
           if (token != null) {     
               AppsFlyer.registerUninstall (token);
               tokenSent = true;
           }
       }
#endif
	}

	#endregion  //End monobehaviour methods

	//-----------------------------------------------------------//
	//                      PRIVATE METHODS                      //
	//-----------------------------------------------------------//
	#region Private methods
	#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//
	#region Private members
	private static Appsflyermanager _instance = null;
#endregion  //End private members
}
