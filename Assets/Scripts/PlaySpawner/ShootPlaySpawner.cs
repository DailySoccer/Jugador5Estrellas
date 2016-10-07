using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.AnimationController;
using Assets.Scripts.Physics;
using FootballStar.Common;

public class ShootPlaySpawner : PlaySpawner {

	//-----------------------------------------------------------//
	//                      CONFIGURATION                        //
	//-----------------------------------------------------------//
	#region Configuration
	[Range(0, 1)]
	public const float FIELD_WIDTH_SPAWN_FACTOR = 0.4f;
	[Range(0, 1)]
	public const float FIELD_DEPTH_SPAWN_FACTOR = 0.15f;
	[Range(0, 1)]
	public const float FIELD_DEPTH_SPAWN_GAP = 0.6f;
	#endregion

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	public const float MIN_FRONT_DIST = 15;
	public const float RANGE_FRONT_DIST = 4;
	public const float MIN_SIDE_DIST = 4;
	public const float RANGE_SIDE_DIST = 5;
	public static float SECONDS_TO_WALL = 5;
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
			List<GameObject> attackers = _matchRef.GetAttackers();
			AIPlayer.SetInvulnerabilityDribblingTime(1.2f);
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
			_thePlayer.transform.position = Vector3.right * (FIELD_DEPTH_SPAWN_GAP - 0.5f + (float)_rnd.NextDouble() * FIELD_DEPTH_SPAWN_FACTOR) * FieldDepth + Vector3.forward * ((float)(_rnd.NextDouble() - 0.5f) * 0.85f * FieldWidth * FIELD_WIDTH_SPAWN_FACTOR);
			_thePlayer.transform.rotation = Quaternion.LookRotation(Vector3.right/*new Vector3(50, 0, 0) - _thePlayer.transform.position*/, Vector3.up);
			_thePlayer.Reset();
			_thePlayer.SetStartTargetPos(new Vector3(FieldDepth * 0.48f, 0, _thePlayer.transform.position.z)/*(50, 0, 0)*/);
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
			SetSecondsToWall(difficultyLevel);
			AIDefender.SetFailProbability(0);
			int assistPartners = _rnd.Next(2, 4);
			float partDepth = FieldDepth * 0.6f / (defenders.Count / 4);
			GameObject furthest = null;
			for (int i = 0; i < defenders.Count; ++i)
			{
				AI = null;
				objRef = defenders[i];
				if (i != 0)
				{
					objRef.transform.position = new Vector3(1000, 0, 0);
					//int coeff, rest;
					//coeff = 2 - (i / 4);
					//rest = 3 - (i % 4);
					//objRef.transform.position = Vector3.forward * ((rest - 2) * 0.25f + 0.125f) * FieldWidth * 0.9f + Vector3.right * (coeff * partDepth - FieldDepth * 0.35f);
					//objRef.transform.rotation = Quaternion.LookRotation(Vector3.left);
					//if (i > 3 && i < 8)
					//{
					//	float distToPlayer = (objRef.transform.position - _thePlayer.transform.position).sqrMagnitude;
					//	if (wosrstDist < 0 || distToPlayer > wosrstDist)
					//	{
					//		furthest = objRef;
					//		wosrstDist = distToPlayer;
					//	}
					//}
					//AI = defenders[i].GetComponent<AIDefender>();
					//if (AI == null)
					//{
					//	AI = defenders[i].AddComponent<AIDefender>();
					//	AI.GetComponentInChildren<OnTriggerParser>().SetAIRef(AI);
					//}
					//(AI as AIDefender).SetStartTargetPos(AI.transform.position);
					//(AI as AIDefender).SetTargetAttacker(null);
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
				}
				if (AI != null)
				{
					AI.Activate();
					((AIGoalkeeper)AI).Setup(difficultyLevel);
				}
			}
			bool firstOpponent = true;
			int firstplayer = 4;
			int lastplayer = Mathf.Min(firstplayer + _NumDefenders + 5, defenders.Count);
			for (int i = firstplayer; i < lastplayer; ++i)
			{
				if (defenders[i] != furthest)
				{
					AIDefender defender = defenders[i].GetComponent<AIDefender>();
					{//TODO this block should be removed, so this code is being execute some lines upper
						if (defender == null)
						{
							defender = defenders[i].AddComponent<AIDefender>();
						}
					}
					if (defender != null)
					{
						defender.Deactivate();
						if (firstOpponent)
						{
							defender.transform.position = _thePlayer.transform.position + _thePlayer.transform.forward * AnimFootballPlayer.DESP_SPEED * SECONDS_TO_WALL;
							defender.transform.rotation = Quaternion.LookRotation(_thePlayer.transform.position - defender.transform.position, Vector3.up);
							firstOpponent = false;
							defender.SetStartTargetPos(defender.transform.position);
							AIDefender.SetTheWall(defender);
						}
						else if (i < lastplayer - 4)
						{
							int side = (i & 1) == 0 ? 1 : -1;
							float frontDist = MIN_FRONT_DIST + ((float)_rnd.NextDouble() * RANGE_FRONT_DIST);
							float sideDist = (MIN_SIDE_DIST + ((float)_rnd.NextDouble() * RANGE_SIDE_DIST)) * side;
							defender.transform.position = _thePlayer.transform.position + _thePlayer.transform.forward * frontDist + _thePlayer.transform.right * sideDist;
							defender.transform.rotation = Quaternion.LookRotation(_thePlayer.transform.position - defender.transform.position, Vector3.up);
							//defender.SetTargetAttacker(_thePlayer.transform);
							defender.SetStartTargetPos(_thePlayer.transform.position + _thePlayer.transform.forward * 10 - defender.transform.forward);
						}
						else
						{
							int index = (lastplayer - i);
							float section = (index - 1.5f);
							float regionPartZ = section * FieldWidth * 0.220f;
							float regionPartX = (4 - Mathf.Abs(section)) * FieldDepth * 0.12f;
							defender.transform.position = new Vector3(regionPartX, 0, regionPartZ);
							defender.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
							defender.SetStartTargetPos(defender.transform.position);
						}
						defender.GetComponentInChildren<OnTriggerParser>().SetAIRef(defender);
						defender.SetTargetAttacker(null);
						defender.Activate();
					}
				}
			}
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
				_NumDefenders = 0;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				_NumDefenders = 1;
				break;
			case MatchBridge.Difficulty.HIGH:
				_NumDefenders = 2;
				break;
			case MatchBridge.Difficulty.EXTREME:
				_NumDefenders = 3;
				break;
		}
	}

	private void SetSecondsToWall(MatchBridge.Difficulty difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case MatchBridge.Difficulty.EASY:
				SECONDS_TO_WALL = 5;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				SECONDS_TO_WALL = 5;
				break;
			case MatchBridge.Difficulty.HIGH:
				SECONDS_TO_WALL = 4;
				break;
			case MatchBridge.Difficulty.EXTREME:
				SECONDS_TO_WALL = 3;
				break;
		}
	}
	#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//
	#region Private members
	private static int _NumDefenders;
	#endregion  //End private members
}
