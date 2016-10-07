using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Manager.Model;
using FootballStar.Audio;

namespace FootballStar.Manager
{
	public class MainMenu : MonoBehaviour
	{
		public GameObject TapHereToPlay;
		public GameObject TapHereToTrain;
		public GameObject CompetitionExplanationPrefab;
		public GameObject WelcomeModal;
		public GameObject GenetsisPrefab;
		public GameObject GenetsisButton;

		void Awake()
		{
			mScreenStackController = GameObject.Find("Header").GetComponent<ScreenStack>();
			
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			mAudioController = mMainModel.GetComponent<AudioInGameController>();
#if UNITY_WEBPLAYER
			GenetsisButton.SetActive(false);
#endif
		}
		
		void OnEnable()
		{
			UpdateTutorial ();

			bool showTrainCartela = (int)mMainModel.Player.TutorialStage > (int)TutorialStage.CUP_EXPLANATION && !mMainModel.Player.EntrenarTutorialScreenAlreadyShown;
			//Debug.LogError (">>>>  Estado tutorial: " + mGameModel.Player.TutorialStage.ToString ());
			TapHereToTrain.SetActive (showTrainCartela);

			// Mostramos la cartela despues del primer partido
			if (mMainModel.Player.TutorialStage == TutorialStage.FRIENDLY_EXPLANATION) {
				TapHereToTrain.SetActive (false);
			}
		}
		
		void Start()
		{
			mAudioController.PlayDefinition(SoundDefinitions.THEME_MAIN, true);
		}

		void UpdateTutorial() {
			if (mMainModel.Player.TutorialStage == TutorialStage.DONE) {
				TapHereToPlay.SetActive (false);
				TapHereToTrain.SetActive (false);
			} else if (mMainModel.Player.TutorialStage == TutorialStage.CUP_EXPLANATION) {
				mMessageOverlap = NGUITools.AddChild (this.gameObject, CompetitionExplanationPrefab);
				mMessageOverlap.GetComponentInChildren<UIButtonMessage> ().target = this.gameObject;
			} else if (mMainModel.Player.TutorialStage == TutorialStage.WELCOME) {
				mMessageOverlap = NGUITools.AddChild (this.gameObject, WelcomeModal);
				mMessageOverlap.GetComponentInChildren<UIButtonMessage> ().target = this.gameObject;
			}
		}

		void OnContinueWelcomeClick() {
			//Evento Tutorial: Bienvenido a FS
			//MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "WELCOME"} });

			Destroy(mMessageOverlap);
			mMessageOverlap = null;
			mMainModel.NextStageTutorial ();
			UpdateTutorial ();
		}

		void OnContinueCompetitionExplanationClick() {
			//Evento Tutorial: Explicacion de competiciones
			//MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "COMPETICION"} });

			Destroy(mMessageOverlap);
			mMessageOverlap = null;
		}

		void OnTrainingClick()
		{
			//Evento Tutorial: Explicacion de Training
			//MixPanel.SendEventToMixPanel(AnalyticEvent.SCREEN_VIEW, new Dictionary<string, object>{ {"Screen Name", "Training Screen"} });
			System.Collections.Generic.Dictionary<string, string> parameters = new System.Collections.Generic.Dictionary<string, string>();
			parameters.Add(AFInAppEvents.TRAIN_SCREEN, "entrar");
			AppsFlyer.trackRichEvent(AFInAppEvents.TRAIN_SCREEN, parameters);
			mScreenStackController.PushScreenController("TrainingScreen");
		}
		
		void OnLifeClick()
		{
			//Evento Tutorial: Explicacion de Life
			//MixPanel.SendEventToMixPanel(AnalyticEvent.SCREEN_VIEW, new Dictionary<string, object>{ {"Screen Name", "Life Screen"} });
			System.Collections.Generic.Dictionary <string,string> parameters = new System.Collections.Generic.Dictionary<string, string> ();
			parameters.Add(AFInAppEvents.LIFE_SCREEN, "entrar");
			AppsFlyer.trackRichEvent(AFInAppEvents.LIFE_SCREEN, parameters);
			mScreenStackController.PushScreenController("LifeScreen");
		}
		
		void OnPlayClick()
		{
			//Evento Tutorial: Explicacion de Play
			//MixPanel.SendEventToMixPanel(AnalyticEvent.SCREEN_VIEW, new Dictionary<string, object>{ {"Screen Name", "Play Screen"} });
			System.Collections.Generic.Dictionary<string, string> parameters = new System.Collections.Generic.Dictionary<string, string>();
			parameters.Add(AFInAppEvents.PLAY_SCREEN, "entrar");
			AppsFlyer.trackRichEvent(AFInAppEvents.PLAY_SCREEN, parameters);
			mScreenStackController.PushScreenController("PlayScreen");
		}

		void OnSponsorsClick()
		{
			//Evento Tutorial: Explicacion de Sponsor
			//MixPanel.SendEventToMixPanel(AnalyticEvent.SCREEN_VIEW, new Dictionary<string, object>{ {"Screen Name", "Sponsors Screen"} });
			System.Collections.Generic.Dictionary<string, string> parameters = new System.Collections.Generic.Dictionary<string, string>();
			parameters.Add(AFInAppEvents.SPONSOR_SCREEN, "entrar");
			AppsFlyer.trackRichEvent(AFInAppEvents.SPONSOR_SCREEN, parameters);
			mScreenStackController.PushScreenController("SponsorsScreen");
		}
			
		MainModel mMainModel;
		GameObject mMessageOverlap;

		ScreenStack mScreenStackController;
		AudioInGameController mAudioController;
	}

}
