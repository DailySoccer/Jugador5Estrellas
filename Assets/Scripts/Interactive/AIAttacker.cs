using UnityEngine;
using System.Collections;
using FootballStar.Match3D;
using System.Collections.Generic;
using System;
using Assets.Scripts.AnimationController;
using Assets.Scripts.Physics;

public class AIAttacker : AIAgent {

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods
	public override void SetStartTargetPos(Vector3 targetPos)
	{
		base.SetStartTargetPos(targetPos);
		_initialTargetPos = _targetPos;
	}
	#endregion  //End public methods

	#region Public static methods
	public static void calculateNearest(Vector3 ballPos, Vector3 fooA)
	{
		//_shotMoment = Time.time;
		//_ballPos = ballPos;
		//_ballPos.y = 0;
		//_ballDir = _ball.GroundVelocity;
		//_ballDir.Normalize(); List<GameObject> attackers = _matchRef.GetAttackers();
		//float bestTime = -1, currTime, bestLateDist = -1, currLateDist;
		//AIAttacker reserve = null;
		//foreach (GameObject go in attackers)
		//{
		//	AIAttacker attacker = go.GetComponent<AIAttacker>();
		//	if (attacker != null)
		//	{
		//		currTime = attacker.BestIntercept();
		//		if (currTime > 0 && (bestTime < 0 || currTime < bestTime))
		//		{
		//			bestTime = currTime;
		//			_nearest = attacker;
		//		}
		//		currLateDist = attacker.DistanceInFuture(10);
		//		if (bestLateDist < 0 || currLateDist < bestLateDist)
		//		{
		//			bestLateDist = currLateDist;
		//			reserve = attacker;
		//		}
		//	}
		//}
		//if (true/*_nearest == null*/)
		//{
			_updateBestPartner = true;
			if (_dummy == null)
			{
				_dummy = GameObject.FindObjectOfType<MonoBehaviour>();
			}
			_dummy.StartCoroutine(NearestPartner());
		//}
	}
	public static void Reset()
	{
		_nearest = null;
		_updateBestPartner = false;
	}
	#endregion

	//-----------------------------------------------------------//
	//                   MONOBEHAVIOUR METHODS                   //
	//-----------------------------------------------------------//
	#region Monobehaviour methods
	protected override void Awake()
	{
		base.Awake();
	}
	// Use this for initialization
	protected override void Start()
	{
		base.Start();
	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
	}

	protected override void OnTriggerEnter(Collider other)
	{
		InteractiveMatch.NotifyResult(InteractiveMatch.GameAction.Pass);
		PhysicBall ball = other.attachedRigidbody.GetComponent<PhysicBall>();
		if (ball != null)
		{
			ball.AttachTo(transform, Vector3.zero);
		}
		_nearest = null;
		_updateBestPartner = false;
	}
	#endregion  //End monobehaviour methods

	//-----------------------------------------------------------//
	//                      PRIVATE METHODS                      //
	//-----------------------------------------------------------//
	#region Private methods
	protected override void SetDestination()
	{
		if (this == _nearest)
		{
			//if (Time.time - _shotMoment < _totalTime * 0.5f)
			//{
			//	_targetPos = _nearestPos;
			//}
			//else
			//{
			float dist = Vector3.Distance(_ball.Position, transform.position);
				_targetPos = (_ball.PredictPositionAfterTime(Mathf.Clamp(dist, 1f, 5f)) + _ball.Position) * 0.5f;
			//}
		}
		else
		{
			_targetPos = _initialTargetPos;
		}
	}

	private float BestIntercept()
	{
		float totalTime = -1;
		if (Vector3.Dot(_ball.Position - transform.position, transform.forward) < 0)
		{
			Vector3 zeroPos = transform.position;
			zeroPos.y = 0;
			Vector3 toTarget = _ballPos - zeroPos;
			float distPer = Vector3.Cross(_ballPos - zeroPos, _ballDir).magnitude;
			AnimFootballPlayer animFP = GetComponent<AnimFootballPlayer>();
			Vector3 finalPlayerPos = _ballPos + _ballDir * Mathf.Abs(Vector3.Dot(toTarget, _ballDir));
			totalTime = Vector3.Angle(transform.forward, finalPlayerPos - transform.position) / animFP.ROT_SPEED;
			totalTime += distPer / AnimFootballPlayer.DESP_SPEED;
			_totalTime = totalTime;
			Vector3 ballPosAfterTime = _ball.PredictPositionAfterTime(totalTime);
			Vector3 fppTObpat = ballPosAfterTime - finalPlayerPos;
			bool passed = Vector3.Dot(finalPlayerPos - _ballPos, fppTObpat) > 0 || Vector3.Dot(finalPlayerPos - _ballPos, _ballDir) <= 0;
			if (passed)
			{
				totalTime = -1;
			}
			else
			{
				totalTime += fppTObpat.magnitude / _ball.GroundVelocity.magnitude;
				_nearestPos = finalPlayerPos;
			}
		}
		return totalTime;
	}

	private float DistanceInFuture(float dTime)
	{
		Vector3 myPosFuture = transform.position + Vector3.right * AnimFootballPlayer.DESP_SPEED * dTime;
		Vector3 ballPosFuture = _ball.PredictPositionAfterTime(dTime);
		return (ballPosFuture - myPosFuture).sqrMagnitude;
	}

	private static IEnumerator NearestPartner()
	{
		while (_updateBestPartner)
		{
			AIAttacker nearest = null;
			float bestDist = -1, currDist;
			foreach (GameObject go in _matchRef.GetAttackers())
			{
				AIAttacker attacker = go.GetComponent<AIAttacker>();
				if (attacker != null && attacker.IsActive)
				{
					currDist = attacker.DistanceInFuture(1);
					if (bestDist < 0 || currDist < bestDist)
					{
						nearest = attacker;
						bestDist = currDist;
					}
				}
			}
			_nearest = nearest;
			yield return new WaitForSeconds(0.5f);
		}
	}
	#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//
	#region Private members
	private Vector3 _initialTargetPos;
	private Vector3 _nearestPos;
	private float _totalTime;
	#endregion  //End private members

	#region Private static members
	private static AIAttacker _nearest;
	private static Vector3 _ballPos;
	private static Vector3 _ballDir;
	private static float _shotMoment;
	private static MonoBehaviour _dummy;
	private static bool _updateBestPartner;
	#endregion
}
