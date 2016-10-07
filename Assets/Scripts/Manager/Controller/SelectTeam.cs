using UnityEngine;
using System.Collections;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
	{
	public class SelectTeam : MonoBehaviour {

		public UILabel HeaderTitle;

		public GameObject PlayButton;

		public GameObject TeamsWrapper;
		public GameObject Selector;

		public GameObject NickWrapper;
		public UIInput NicknameLabel;


		int SelectedTeam;
		
		void Awake () {
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			mHeader = GameObject.Find("Header").GetComponent<Header>();

			Initializing ();
		}

		void Initializing() {
			PlayButtonLabel = PlayButton.GetComponentInChildren<UILabel>();
			mCurrentState = SelectTeamState.NONE;
			PlayButton.SetActive(false);
			Selector.SetActive (false);

			SetState (SelectTeamState.WAITING_FOR_TEAM);
		}

		void SetState (SelectTeamState newState) {
			if (mCurrentState != newState) {
				switch (newState) {
				case SelectTeamState.WAITING_FOR_TEAM:
					currentWrapperAnim = TeamsWrapper.GetComponent<Animator>();
					HeaderTitle.text = "ELIGE UN EQUIPO";
					PlayButtonLabel.text = "CONTINUAR";
					break;
				case SelectTeamState.WAITING_FOR_NAME:										
					HeaderTitle.text = "ELIGE UN NOMBRE";
					currentWrapperAnim = NickWrapper.GetComponent<Animator>();
					PlayButtonLabel.text = "COMENZAR";
					break;
				}
				PlayButton.SetActive(false);
				currentWrapperAnim.SetBool("IsOpen", true);

				mCurrentState = newState;			
			}
		}

		void Update () {
			if (mCurrentState == SelectTeamState.WAITING_FOR_TEAM) {
				PlayButton.SetActive (isTeamSelected);

			} else if (mCurrentState == SelectTeamState.WAITING_FOR_NAME) {
				PlayButton.SetActive (NicknameLabel.value.Trim ().Length >= 3);
			}
		}

		public void Select(GameObject sender) {
			isTeamSelected = true;
			Selector.SetActive(true);
			Selector.transform.position = sender.transform.position;
			SelectedTeam = int.Parse(sender.name);
			Debug.Log ("El jugador elige: " + FootballStar.Manager.Model.TierDefinition.GetTeamName(SelectedTeam));
		}

		void SetupTeamAndContinue() {

			currentWrapperAnim.SetBool("IsOpen", false);

			switch (mCurrentState) {
			case SelectTeamState.WAITING_FOR_TEAM:
					System.Collections.Generic.Dictionary<string, string> parameters = new System.Collections.Generic.Dictionary<string, string>();
					parameters.Add(AFInAppEvents.TEAM_SELECTION, FootballStar.Manager.Model.TierDefinition.GetTeamName(SelectedTeam));
					AppsFlyer.trackRichEvent(AFInAppEvents.TEAM_SELECTION, parameters);
					SetState(SelectTeamState.WAITING_FOR_NAME);
				break;
			case SelectTeamState.WAITING_FOR_NAME:

				mMainModel.Nick = NicknameLabel.value.Trim();
				
				mMainModel.SelectedTeamId = SelectedTeam;
				mMainModel.Player.IsTeamSelected = true;
				mMainModel.Player.CreateTiers ();
				
				// Guardamos los datos seleccionados
				mMainModel.SaveDefaultGame();
				
				// Nos tenemos que desactivar mientras se hace el fade-out, etc
				StartCoroutine(GoToMainScreen());
				//enabled = false;
				break;
			}
		}

		IEnumerator GoToMainScreen()
		{
			yield return StartCoroutine(CameraFade.FadeCoroutine(false, 1.0f, 0.0f));
			mHeader.GoToMainScreen();
		}

		enum SelectTeamState {
			NONE,
			WAITING_FOR_TEAM,
			WAITING_FOR_NAME
		}

		SelectTeamState mCurrentState;

		MainModel mMainModel;
		Header mHeader;

		GameObject selection;

		UILabel PlayButtonLabel;

        bool isNickNanmeTyped;
        bool isTeamSelected;

		Animator currentWrapperAnim;
	}
}