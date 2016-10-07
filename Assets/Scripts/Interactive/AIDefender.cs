using UnityEngine;
using System.Collections;
using Assets.Scripts.Physics;
using Assets.Scripts.AnimationController;

public class AIDefender : AIAgent {

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	public static float FAIL_PROB = 0.5f;
	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods
	public void SetTargetAttacker(Transform target)
	{
		_targetAttacker = target;
	}
	public override void Deactivate()
	{
		_targetAttacker = null;
		base.Deactivate();
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
		_theWall = null;
		_updateBestPartner = false;
	}
	public static void SetTheWall(AIDefender theWall)
	{
		_theWall = theWall;
	}
	public static void SetFailProbability(float failProb)
	{
		FAIL_PROB = Mathf.Clamp01(failProb);
	}
	#endregion

	//-----------------------------------------------------------//
	//                  MONOBEHAVIOUR METHODS                    //
	//-----------------------------------------------------------//
	#region Monobehaviour methods

	protected override void Awake()
	{
		base.Awake();
		_targetPos = transform.position;
	}
	// Use this for initialization
	protected override void Start()
	{
		base.Start();
	}

	protected override void OnTriggerEnter(Collider other)
	{
		if (_theWall == this || (PlaySpawner.ThePlayer != null && (!PlaySpawner.ThePlayer.IsDribbling || _rnd.NextDouble() < FAIL_PROB)))
		{
			AIPlayer.ForceHasShot();
			InteractiveMatch.NotifyResult(InteractiveMatch.GameAction.Intercepted);
			_targetAttacker = null;
			_targetPos = transform.position - Vector3.right * 40;
			PhysicBall ball = other.attachedRigidbody.gameObject.GetComponent<PhysicBall>();
			if (ball != null)
			{
				ball.AttachTo(transform.GetComponent<AnimFootballPlayer>().BallPlacer, Vector3.zero);
			}
			_updateBestPartner = false;
		}
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
			if (_updateBestPartner)
			{
				_targetPos = (_ball.PredictPositionAfterTime(5f) + _ball.Position) * 0.5f;
			}
			//}
		}
		else
		{
			if (_targetAttacker != null)
			{
				_targetPos = _targetAttacker.position + (_ball.transform.position - _targetAttacker.position) * 0.2f;
			}
			else
			{
				if ((transform.position - _ball.transform.position).sqrMagnitude < 20)
				{
					if (_tackle /*&& this != _theWall*/)
					{
						_tackle = false;
						Tackling();
					}
				}
				else
				{
					_tackle = true;
				}
			}
		}
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
			AIDefender nearest = null;
			float bestDist = -1, currDist;
			foreach (GameObject go in _matchRef.GetDefenders())
			{
				AIDefender defender = go.GetComponent<AIDefender>();
				if (defender != null && defender.IsActive)
				{
					currDist = defender.DistanceInFuture(1);
					if (bestDist < 0 || currDist < bestDist)
					{
						nearest = defender;
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
	private Transform _targetAttacker;
	private bool _tackle;
	private static MonoBehaviour _dummy;
	private static bool _updateBestPartner;
	private static AIDefender _nearest;
	private static AIDefender _theWall;
	#endregion  //End private members

}