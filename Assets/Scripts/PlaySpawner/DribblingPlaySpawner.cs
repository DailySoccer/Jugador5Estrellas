using UnityEngine;
using System.Collections;
using Assets.Scripts.AnimationController;
using Assets.Scripts.Physics;
using System.Collections.Generic;
using FootballStar.Common;
using System;

public class DribblingPlaySpawner : PlaySpawner {

	//-----------------------------------------------------------//
	//                      CONFIGURATION                        //
	//-----------------------------------------------------------//
	#region Configuration
	[Range(0, 1)]
	public const float FIELD_WIDTH_SPAWN_FACTOR = 0.85f;
	[Range(0, 1)]
	public const float FIELD_DEPTH_SPAWN_FACTOR = 0.25f;
	[Range(0, 1)]
	public const float FIELD_DEPTH_SPAWN_GAP = 0.5f;
	#endregion

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	public const float MIN_ANGLE = 30;
	public const float RANGE_ANGLE = 50;
	public static float MIN_DIST = 5;
	public static float RANGE_DIST = 7;
	public const int MAX_DEFENDERS = 3;
	public static float FAIL_PROBABILITY = 1;
	public static float INVULNERABLE_TIME = 1.2f;
	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods
	public override void PlaceAttackers(MatchBridge.Difficulty difficultyLevel)
	{
		if (_matchRef != null)
		{
			SetNumDefenders(difficultyLevel);
			SetDistances(difficultyLevel);
			SetPlayerInv(difficultyLevel);
			List<GameObject> attackers = _matchRef.GetAttackers();
			foreach (GameObject go in attackers)
			{
				AIAgent AI = go.GetComponent<AIAgent>();
				if (AI != null)
				{
					AI.Deactivate();
				}
				go.transform.position = new Vector3(-100, 0, 0);
			}
			//set up the player
			if (_thePlayer == null)
			{
				_thePlayer = attackers[PlayerDorsal].gameObject.AddComponent<AIPlayer>();
				_thePlayer.GetComponentInChildren<OnTriggerParser>().SetAIRef(_thePlayer);
			}
			_thePlayer.transform.position = Vector3.right * Mathf.Min((FIELD_DEPTH_SPAWN_GAP - 0.5f + (float)_rnd.NextDouble() * FIELD_DEPTH_SPAWN_FACTOR) * FieldDepth, FieldDepth * 0.5f - (MIN_DIST + RANGE_DIST) * (_NumDefenders+1)) + Vector3.forward * ((float)(_rnd.NextDouble() - 0.5f) * FIELD_WIDTH_SPAWN_FACTOR * FieldWidth);
			_thePlayer.transform.rotation = Quaternion.LookRotation(new Vector3(50, 0, 0) - _thePlayer.transform.position, Vector3.up);
			_thePlayer.Reset();
			_thePlayer.SetStartTargetPos(new Vector3(50, 0, 0));
			_thePlayer.Activate();
			AnimFootballPlayer animController = _thePlayer.gameObject.GetComponent<AnimFootballPlayer>();
			//Attach the ball to player
			PhysicBall ball = _matchRef.TheBall.GetComponent<PhysicBall>();
			if (ball != null)
			{
				SphereCollider sphere = ball.GetComponent<SphereCollider>();
				ball.AttachTo(animController.BallPlacer, _thePlayer.transform.forward * 0.5f + _thePlayer.transform.right * -0.1f);
			}
		}
	}

	public override void PlaceDefenders(MatchBridge.Difficulty difficultyLevel)
	{
		if (_matchRef != null)
		{
			List<GameObject> defenders = _matchRef.GetDefenders();
			GameObject objRef;
			AIAgent AI;
			UpdateFailProbability(difficultyLevel);
			float partDepth = FieldDepth * 0.6f / (defenders.Count / 4);
			for (int i = 0; i < defenders.Count; ++i)
			{
				AI = null;
				objRef = defenders[i];
				if (i != 0)
				{
					objRef.transform.position = new Vector3(-100, 0, 0);
					AI = objRef.GetComponent<AIDefender>();
					if (AI == null)
					{
						AI = defenders[i].AddComponent<AIDefender>();
						AI.GetComponentInChildren<OnTriggerParser>().SetAIRef(AI);
					}
					AI.Deactivate();
				}
				else
				{
					objRef.transform.position = Vector3.right * FieldDepth * 0.5f;
					AI = defenders[i].GetComponent<AIGoalkeeper>();
					if (AI == null)
					{
						AI = defenders[i].AddComponent<AIGoalkeeper>();
						AI.GetComponentInChildren<OnTriggerParser>().SetAIRef(AI);
					}
					objRef.transform.rotation = Quaternion.LookRotation(-Vector3.right);
					AI.Activate();
				}
			}
			Vector3 refPos = _thePlayer.transform.position;
			for (int i = 1; i <= _NumDefenders + 4; ++i)
			{
				AIDefender defender = defenders[i].GetComponent<AIDefender>();
				if (defender != null)
				{
					if (i <= _NumDefenders)
					{
						int side = (i & 1) == 0 ? 1 : -1;
						float dist = MIN_DIST + ((float)_rnd.NextDouble() * RANGE_DIST);
						refPos += dist * _thePlayer.transform.forward;
						float distToEnd = (refPos - _thePlayer.transform.position).magnitude;
						float angle = (MIN_ANGLE + ((float)_rnd.NextDouble() * RANGE_ANGLE)) * side * Mathf.Deg2Rad;
						defender.transform.position = refPos + (_thePlayer.transform.forward * Mathf.Cos(angle) + _thePlayer.transform.right * Mathf.Sin(angle)) * distToEnd;
						defender.transform.rotation = Quaternion.LookRotation(refPos - defender.transform.position, Vector3.up);
						defender.SetStartTargetPos(defender.transform.position + defender.transform.forward * distToEnd * 2 - _thePlayer.transform.forward * (i - 1) * 1.5f);
					}
					else
					{
						float section = (i - (_NumDefenders + 1) - 1.5f);
						float regionPartZ = section * FieldWidth * 0.220f;
						float regionPartX = (5 - Mathf.Abs(section)) * FieldDepth * 0.08f;
						defender.transform.position = new Vector3(regionPartX, 0, regionPartZ);
						defender.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
						defender.SetStartTargetPos(defender.transform.position);
					}
					defender.Activate();
				}
			}
			_thePlayer.SetStartTargetPos(refPos+_thePlayer.transform.forward * 3);
		}
	}
	#endregion  //End public methods

	//-----------------------------------------------------------//
	//                  MONOBEHAVIOUR METHODS                    //
	//-----------------------------------------------------------//
	#region Monobehaviour methods

	// Use this for initialization
	void Start () {
	
	   }
	
	   // Update is called once per frame
	   void Update () {
	
	   }

	#endregion  //End monobehaviour methods

	//-----------------------------------------------------------//
	//                      PRIVATE METHODS                      //
	//-----------------------------------------------------------//
	#region Private methods
	private static void SetNumDefenders(MatchBridge.Difficulty difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case MatchBridge.Difficulty.EASY:
				_NumDefenders = 1;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				_NumDefenders = 2;
				break;
			case MatchBridge.Difficulty.HIGH:
				_NumDefenders = 2;
				break;
			case MatchBridge.Difficulty.EXTREME:
				_NumDefenders = 3;
				break;
		}
	}

	private static void SetDistances(MatchBridge.Difficulty difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case MatchBridge.Difficulty.EASY:
				MIN_DIST = 5;
				RANGE_DIST = 7;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				MIN_DIST = 5;
				RANGE_DIST = 7;
				break;
			case MatchBridge.Difficulty.HIGH:
				MIN_DIST = 5;
				RANGE_DIST = 7;
				break;
			case MatchBridge.Difficulty.EXTREME:
				MIN_DIST = 5;
				RANGE_DIST = 7;
				break;
		}
	}

	private void UpdateFailProbability(MatchBridge.Difficulty difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case MatchBridge.Difficulty.EASY:
				FAIL_PROBABILITY = 0;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				FAIL_PROBABILITY = 0;
				break;
			case MatchBridge.Difficulty.HIGH:
				FAIL_PROBABILITY = 0;
				break;
			case MatchBridge.Difficulty.EXTREME:
				FAIL_PROBABILITY = 0;
				break;
		}
		AIDefender.SetFailProbability(FAIL_PROBABILITY);
	}

	private void SetPlayerInv(MatchBridge.Difficulty difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case MatchBridge.Difficulty.EASY:
				INVULNERABLE_TIME = 1.0f;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				INVULNERABLE_TIME = 0.7f;
				break;
			case MatchBridge.Difficulty.HIGH:
				INVULNERABLE_TIME = 0.5f;
				break;
			case MatchBridge.Difficulty.EXTREME:
				INVULNERABLE_TIME = 0.3f;
				break;
		}
		AIPlayer.SetInvulnerabilityDribblingTime(INVULNERABLE_TIME);
	}
	#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//
	#region Private members
	private static int _NumDefenders;
	#endregion  //End private members
}
