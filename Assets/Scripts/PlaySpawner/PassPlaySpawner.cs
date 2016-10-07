using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Match3D;
using Assets.Scripts.AnimationController;
using Assets.Scripts.Physics;
using FootballStar.Common;

public class PassPlaySpawner : PlaySpawner
{

	//-----------------------------------------------------------//
	//                      CONFIGURATION                        //
	//-----------------------------------------------------------//
	#region Configuration
	[Range(0, 1)]
	public const float FIELD_WIDTH_SPAWN_FACTOR = 0.7f;
	[Range(0, 1)]
	public const float FIELD_DEPTH_SPAWN_FACTOR = 0.25f;
	[Range(0, 1)]
	public const float FIELD_DEPTH_SPAWN_GAP = 0.25f;
	#endregion

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	public static float RANDOM_MIN_DEF_ANGLE = 5;
	public static float RANDOM_MAX_DEF_ANGLE = 15;
	public static int MIN_NUM_PARTNER = 2;
	public static int MAX_NUM_PARTNER = 6;
	public static int MAX_DEFENDERS_MISSING = 0;
	public static float DEF_SPEED_FACTOR = 2;
	public const int MIN_ANGLE = 15;
	public const int MAX_ANGLE = 80;
	public const int MIN_DIST = 6;
	public const int MAX_DIST = 15;
	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods
	public override void PlaceAttackers(MatchBridge.Difficulty difficultyLevel)
	{
		if (_matchRef != null)
		{
			UpdatePlaySeconds(difficultyLevel);
			List<GameObject> attackers = _matchRef.GetAttackers();
			GameObject objRef;
			AIAgent AI;
			Vector3 randomPlayerPos = Vector3.zero;
			AIPlayer.SetInvulnerabilityDribblingTime(1.2f);
			randomPlayerPos.x = FieldDepth * (FIELD_DEPTH_SPAWN_GAP - 0.5f + (float)_rnd.NextDouble() * FIELD_DEPTH_SPAWN_FACTOR);
			randomPlayerPos.z = (FieldWidth - MAX_DIST * 2) * (((float)_rnd.NextDouble() - 0.5f) * FIELD_WIDTH_SPAWN_FACTOR);
			int assistPartners = NumPartners(difficultyLevel);
			float partDepth = FieldDepth * 0.6f / (attackers.Count / 4);
			//set up other players
			for (int i = attackers.Count - 1, j = 0; i >= 0; --i)
			{
				AI = null;
				objRef = attackers[i];
				if (i == 0)
				{
					objRef.transform.position = Vector3.right * -FieldDepth * 0.5f;
					AI = objRef.GetComponent<AIGoalkeeper>();
					if (AI == null)
					{
						AI = objRef.AddComponent<AIGoalkeeper>();
						AI.GetComponentInChildren<OnTriggerParser>().SetAIRef(AI);
					}
				}
				else if (i != PlayerDorsal)
				{
					if (j < assistPartners)
					{
						int side = (i & 1) == 0 ? 1 : -1;
						float angle = _rnd.Next(MIN_ANGLE, MAX_ANGLE + 1) * Mathf.Deg2Rad * side;
						float distance = _rnd.Next(MIN_DIST, MAX_DIST + 1);
						Vector3 newPos = randomPlayerPos + (Vector3.right * Mathf.Cos(angle) + Vector3.forward * Mathf.Sin(angle)) * distance;
						newPos.x = Mathf.Clamp(newPos.x, _matchRef.FieldDepth * -0.5f, _matchRef.FieldDepth * 0.5f);
						newPos.z = Mathf.Clamp(newPos.z, _matchRef.FieldWidth * -0.5f, _matchRef.FieldWidth * 0.5f);
						objRef.transform.position = newPos;
						++j;
					}
					else
					{
						int coeff, rest;
						coeff = (i + j) / 4;
						rest = (i + j) % 4;
						//objRef.transform.position = Vector3.forward * ((rest - 2) * 0.25f + 0.125f) * FieldWidth * 0.9f + Vector3.right * (coeff * partDepth - FieldDepth * 0.35f);
						objRef.transform.position = new Vector3(1000, 0, 0);
					}
					AI = attackers[i].GetComponent<AIAttacker>();
					if (AI == null)
					{
						AI = attackers[i].AddComponent<AIAttacker>();
						AI.GetComponentInChildren<OnTriggerParser>().SetAIRef(AI);
					}
						(AI as AIAttacker).SetStartTargetPos(new Vector3(48, 0, AI.transform.position.z)/* + Vector3.right * AnimFootballPlayer.DESP_SPEED * _SecondsForPass * 2*/);
				}				
				objRef.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
				if (AI != null)
				{
					AI.Activate();
				}
			}
			//set up the player
			if (_thePlayer == null)
			{
				_thePlayer = attackers[PlayerDorsal].gameObject.AddComponent<AIPlayer>();
				_thePlayer.GetComponentInChildren<OnTriggerParser>().SetAIRef(_thePlayer);
			}
			_thePlayer.transform.position = randomPlayerPos;
			_thePlayer.Reset();
			_thePlayer.Activate();
			_thePlayer.SetStartTargetPos(_thePlayer.transform.position + Vector3.right * AnimFootballPlayer.DESP_SPEED * _SecondsForPass);
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
			List<GameObject> attackers = _matchRef.GetAttackers();
			GameObject objRef;
			AIAgent AI;
			UpdateAngleRange(difficultyLevel);
			SetupDefSpeedFactor(difficultyLevel);
			UpdateDefendersMissing(difficultyLevel);
			AIDefender.SetFailProbability(0);
			int attcCounter = NumPartners(difficultyLevel);
			int firstOrnament = -1;
			for (int i = defenders.Count - 1; i >= 0; --i)
			{
				AI = null;
				objRef = defenders[i];
				if (i != 0)
				{
					if (i == PlayerDorsal)
					{
						Vector3 dir = Vector3.right/*(new Vector3(50, 0, 0) - attackers[i].transform.position).normalized*/;
						objRef.transform.position = attackers[i].transform.position + dir * AnimFootballPlayer.DESP_SPEED * _SecondsForPass;
						objRef.transform.rotation = Quaternion.LookRotation(-dir);
						AIDefender.SetTheWall(objRef.GetComponent<AIDefender>());
					}
					else if (attcCounter > 0)
					{
						if (attcCounter > MAX_DEFENDERS_MISSING)
						{
							objRef.transform.position = attackers[i].transform.position + (_matchRef.TheBall.position - attackers[i].transform.position) * 0.3f;
							objRef.transform.rotation = Quaternion.LookRotation(Vector3.right);
							float rotAngle = (_rnd.NextDouble() > 0.5 ? 1 : -1) * (RANDOM_MIN_DEF_ANGLE + (float)_rnd.NextDouble() * (RANDOM_MAX_DEF_ANGLE - RANDOM_MIN_DEF_ANGLE));
							objRef.transform.RotateAround(attackers[i].transform.position, Vector3.up, rotAngle);

							Animator anim = objRef.GetComponent<Animator>();
							if (anim != null)
							{
								anim.speed = DEF_SPEED_FACTOR;
							}
						}
						else
						{
							objRef.transform.position = new Vector3(-1000, 0, 0);
						}
						attcCounter--;
					}
					else
					{
						if (firstOrnament == -1)
						{
							firstOrnament = i;
						}
						int index = (firstOrnament - i);
						float section = (index - 1.5f);
						float regionPartZ = section * FieldWidth * 0.220f;
						float regionPartX = (4 - Mathf.Abs(section)) * FieldDepth * 0.04f;
						objRef.transform.position = new Vector3(regionPartX, 0, regionPartZ);
						objRef.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
					}
					AI = defenders[i].GetComponent<AIDefender>();
					if (AI == null)
					{
						AI = defenders[i].AddComponent<AIDefender>();
						AI.GetComponentInChildren<OnTriggerParser>().SetAIRef(AI);
					}
					if (i == PlayerDorsal)
					{
						(AI as AIDefender).SetStartTargetPos(AI.transform.position);
						(AI as AIDefender).SetTargetAttacker(null);
					}
					else if (attcCounter > 0)
					{
						(AI as AIDefender).SetTargetAttacker(attackers[i].transform);
					}
					else
					{
						(AI as AIDefender).SetStartTargetPos(AI.transform.position);
					}
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
				AI.Activate();
			}
		}
	}
	#endregion  //End public methods

	//-----------------------------------------------------------//
	//                  MONOBEHAVIOUR METHODS                    //
	//-----------------------------------------------------------//
	#region Monobehaviour methods

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	#endregion  //End monobehaviour methods

	//-----------------------------------------------------------//
	//                      PRIVATE METHODS                      //
	//-----------------------------------------------------------//
	#region Private methods
	private int NumPartners(MatchBridge.Difficulty difficultyLevel)
	{
		int assistPartners = 0;
		switch (difficultyLevel)
		{
			case MatchBridge.Difficulty.EASY:
				assistPartners = MAX_NUM_PARTNER - 3;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				assistPartners = MAX_NUM_PARTNER - 1;
				break;
			case MatchBridge.Difficulty.HIGH:
				assistPartners = MAX_NUM_PARTNER - 2;
				break;
			case MatchBridge.Difficulty.EXTREME:
				assistPartners = MIN_NUM_PARTNER;
				break;
		}
		return assistPartners;
	}

	private void UpdateDefendersMissing(MatchBridge.Difficulty difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case MatchBridge.Difficulty.EASY:
				MAX_DEFENDERS_MISSING = 2;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				MAX_DEFENDERS_MISSING = 2;
				break;
			case MatchBridge.Difficulty.HIGH:
				MAX_DEFENDERS_MISSING = 0;
				break;
			case MatchBridge.Difficulty.EXTREME:
				MAX_DEFENDERS_MISSING = 0;
				break;
		}
	}

	private void SetupDefSpeedFactor(MatchBridge.Difficulty difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case MatchBridge.Difficulty.EASY:
				DEF_SPEED_FACTOR = 0.9f;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				DEF_SPEED_FACTOR = 1;
				break;
			case MatchBridge.Difficulty.HIGH:
				DEF_SPEED_FACTOR = 1.5f;
				break;
			case MatchBridge.Difficulty.EXTREME:
				DEF_SPEED_FACTOR = 1.7f;
				break;
		}
	}

	private void UpdateAngleRange(MatchBridge.Difficulty difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case MatchBridge.Difficulty.EASY:
				RANDOM_MAX_DEF_ANGLE = 40;
				RANDOM_MIN_DEF_ANGLE = 30;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				RANDOM_MAX_DEF_ANGLE = 35;
				RANDOM_MIN_DEF_ANGLE = 25;
				break;
			case MatchBridge.Difficulty.HIGH:
				RANDOM_MAX_DEF_ANGLE = 30;
				RANDOM_MIN_DEF_ANGLE = 20;
				break;
			case MatchBridge.Difficulty.EXTREME:
				RANDOM_MAX_DEF_ANGLE = 20;
				RANDOM_MIN_DEF_ANGLE = 5;
				break;
		}
	}

	private void UpdatePlaySeconds(MatchBridge.Difficulty difficultyLevel)
	{
		switch (difficultyLevel)
		{
			case MatchBridge.Difficulty.EASY:
				_SecondsForPass = 6;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				_SecondsForPass = 5;
				break;
			case MatchBridge.Difficulty.HIGH:
				_SecondsForPass = 4;
				break;
			case MatchBridge.Difficulty.EXTREME:
				_SecondsForPass = 3;
				break;
		}
	}
	#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//
	#region Private members
	private static float _SecondsForPass;
	#endregion  //End private members
}
