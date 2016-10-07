using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Match3D;
using Assets.Scripts.AnimationController;
using Assets.Scripts.Physics;
using FootballStar.Common;

public abstract class PlaySpawner : MonoBehaviour
{

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	public static AIPlayer ThePlayer
	{
		get { return _thePlayer; }
	}
	#endregion  //End public members

	#region Public static members
	public static float FieldWidth = 70;
	public static float FieldDepth = 103;
	public static int PlayerDorsal = 6;
	#endregion

	#region Public static members
	public static void Init(InteractiveMatch matchManagerRef, float fieldWidth, float fieldDepth)
	{
		FieldWidth = fieldWidth;
		FieldDepth = fieldDepth;
		_matchRef = matchManagerRef;
	}
	public static void CreatePlay(GameObject objRef, InteractiveType action, MatchBridge.Difficulty difficultyLevel)
	{
		if (_thePlaySpawner != null)
		{
			Destroy(_thePlaySpawner);
		}
		switch (action)
		{
			case InteractiveType.Dribling:
				_thePlaySpawner = objRef.AddComponent<DribblingPlaySpawner>();
				break;
			case InteractiveType.Shot:
				_thePlaySpawner = objRef.AddComponent<ShootPlaySpawner>();
				break;
			case InteractiveType.Pass:
				_thePlaySpawner = objRef.AddComponent<PassPlaySpawner>();
				break;
			default:
				throw new System.IndexOutOfRangeException("Play action type not correct.");
		}
		_thePlaySpawner.PlaceAttackers(difficultyLevel);
		_thePlaySpawner.PlaceDefenders(difficultyLevel);
	}
	public static PlaySpawner CurrentPlay()
	{
		return _thePlaySpawner;
	}
	#endregion

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods
	public virtual void PlaceAttackers(MatchBridge.Difficulty difficultyLevel)
	{
		if (_matchRef != null)
		{
			List<GameObject> attackers = _matchRef.GetAttackers();
			GameObject objRef;
			AIAgent AI;
			float partDepth = FieldDepth * 0.6f / (attackers.Count / 4);
			//set up other players
			for (int i = attackers.Count - 1; i >= 0; --i)
			{
				AI = null;
				objRef = attackers[i];
				if (i != 0)
				{
					int coeff, rest;
					coeff = i / 4;
					rest = i % 4;
					objRef.transform.position = Vector3.forward * ((rest - 2) * 0.25f + 0.125f) * FieldWidth * 0.9f + Vector3.right * (coeff * partDepth - FieldDepth * 0.35f);
					if (i != PlayerDorsal)
					{
						AI = attackers[i].GetComponent<AIAttacker>();
						if (AI == null)
						{
							AI = attackers[i].AddComponent<AIAttacker>();
							AI.GetComponentInChildren<OnTriggerParser>().SetAIRef(AI);
						}
					}
				}
				else
				{
					objRef.transform.position = Vector3.right * -FieldDepth * 0.5f;
					AI = objRef.GetComponent<AIGoalkeeper>();
					if (AI == null)
					{
						AI = objRef.AddComponent<AIGoalkeeper>();
						AI.GetComponentInChildren<OnTriggerParser>().SetAIRef(AI);
					}
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
			_thePlayer.transform.position = Vector3.zero;
			_thePlayer.Reset();
			_thePlayer.Activate();
			_thePlayer.SetStartTargetPos(new Vector3(50, 0, 0));
			AnimFootballPlayer animController = _thePlayer.gameObject.GetComponent<AnimFootballPlayer>();
			//Attach the ball to player
			PhysicBall ball = _matchRef.TheBall.GetComponent<PhysicBall>();
			if (ball != null)
			{
				SphereCollider sphere = ball.GetComponent<SphereCollider>();
				ball.AttachTo(animController.BallPlacer, Vector3.right * 0.5f + Vector3.forward * 0.1f);
			}
		}
	}
	public virtual void PlaceDefenders(MatchBridge.Difficulty difficultyLevel)
	{
		//TODO: Placedefenders depending on attacker's position, ball position, and line connect.
		if (_matchRef != null)
		{
			List<GameObject> defenders = _matchRef.GetDefenders();
			List<GameObject> attackers = _matchRef.GetAttackers();
			GameObject objRef;
			AIAgent AI;
			for (int i = defenders.Count - 1; i >= 0; --i)
			{
				AI = null;
				objRef = defenders[i];
				if (i != 0)
				{
					objRef.transform.position = attackers[i].transform.position + (_matchRef.TheBall.position - attackers[i].transform.position) * 0.2f;
					AI = defenders[i].GetComponent<AIDefender>();
					if (AI == null)
					{
						AI = defenders[i].AddComponent<AIDefender>();
						AI.GetComponentInChildren<OnTriggerParser>().SetAIRef(AI);
					}
					(AI as AIDefender).SetTargetAttacker(attackers[i].transform);
				}
				else
				{
					objRef.transform.position = Vector3.right * -FieldDepth * 0.5f;
					AI = defenders[i].GetComponent<AIGoalkeeper>();
					if (AI == null)
					{
						AI = defenders[i].AddComponent<AIGoalkeeper>();
						AI.GetComponentInChildren<OnTriggerParser>().SetAIRef(AI);
					}
				}
				objRef.transform.rotation = Quaternion.LookRotation(-Vector3.right, Vector3.up);
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
	#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//
	#region Private members
	#endregion  //End private members

	#region Private static members
	protected static InteractiveMatch _matchRef;
	protected static AIPlayer _thePlayer;
	protected static PlaySpawner _thePlaySpawner;
	protected static System.Random _rnd = new System.Random();
	#endregion
}
