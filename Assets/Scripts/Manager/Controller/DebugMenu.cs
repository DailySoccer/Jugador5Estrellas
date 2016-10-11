using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
{
	
	public class DebugMenu : MonoBehaviour {
	
		void Start () {
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			
		}
		
		void Update () {
		
		}
		
		public void OnSaveClick()
		{
			mMainModel.SaveDefaultGame();
		}
		
		public void OnLoadClick()
		{
			mMainModel.LoadDefaultGame();
		}
		
		public void OnResetClick()
		{
			mMainModel.ResetDefaultGame();
		}
		
		public void OnCloseClick()
		{
			Destroy(this.gameObject);
		}
		
		public void OnButton01Click()
		{
			mMainModel.PlayPerformanceMatch();
		}
		/*
		public void OnAddEnergyClick()
		{
			mMainModel.AddEnergyUnit(1);
		}
		
		public void OnRemoveEnergyClick()
		{
			mMainModel.AddEnergyUnit(-1);
		}
		*/
		public void OnAddMoneyClick()
		{
			mMainModel.AddMoney(1000);
		}
		
		public void OnAddFansClick()
		{
			mMainModel.AddFans(1000);
		}

		public void OnEndTutorialClick() {
			mMainModel.Player.TutorialStage = TutorialStage.DONE;
		}

		public void UnLockCurrentFrendly() {
			Tier tier = mMainModel.Player.CurrentTier;
			tier.MatchBrowser.CurrentFriendly.NumTimesPlayed = 1;
			tier.MatchBrowser.CurrentFriendly.MatchResult = new FootballStar.Common.MatchResult() { ScorePerInteractionSequence = new FootballStar.Common.MatchResult.Statistic(), PlayerGoals = 1, OppGoals = 0   };
		}

		public void UnLockCurrentLiga() {
			Tier tier = mMainModel.Player.CurrentTier;
			tier.MatchBrowser.CurrentLeague.NumTimesPlayed = 1;
			tier.MatchBrowser.CurrentLeague.MatchResult = new FootballStar.Common.MatchResult() { ScorePerInteractionSequence = new FootballStar.Common.MatchResult.Statistic(), PlayerGoals = 1, OppGoals = 0  };
		}

		public void UnLockCurrentCup() {
			Tier tier = mMainModel.Player.CurrentTier;
			tier.MatchBrowser.CurrentCup.NumTimesPlayed = 1;
			tier.MatchBrowser.CurrentCup.MatchResult = new FootballStar.Common.MatchResult() { ScorePerInteractionSequence = new FootballStar.Common.MatchResult.Statistic(), PlayerGoals = 1, OppGoals = 0  };
		}

		public void UnLockCurrentEuro() {
			Tier tier = mMainModel.Player.CurrentTier;
			tier.MatchBrowser.CurrentEuro.NumTimesPlayed = 1;
			tier.MatchBrowser.CurrentEuro.MatchResult = new FootballStar.Common.MatchResult() { ScorePerInteractionSequence = new FootballStar.Common.MatchResult.Statistic(), PlayerGoals = 1, OppGoals = 0  };
		}


		MainModel mMainModel;
	}
}
