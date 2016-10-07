using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Physics;
using FootballStar.Common;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AIGoalkeeper : AIAgent
{


	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members

	/// <summary>
	/// 
	/// </summary>
	public MatchBridge.Difficulty Difficulty
	{
		get { return _difficulty; }
		set {
			SuccessRatio = SuccessByDifficulty[value];
			_difficulty = value;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public float SuccessRatio {
		get { return _successRatio; }
		set { _successRatio = Mathf.Clamp01(value); }
	}

	/// <summary>
	/// 
	/// </summary>
	public bool IsClearing
	{
		get { return _isClearing; }
		private set
		{
			_isClearing = value;
			AreCollidersActive = value;
		}
	}

	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods

	/// <summary>
	/// 
	/// </summary>
	public void Reset()
	{
		Assert.Test(_animController != null, "AIGoalkeeper::Reset>> animController null!!");

		SuccessRatio = SuccessRatioMax;
		IsClearing = false;
		_animController.IsGoalkeeper = true;
	}

	
	/// <summary>
	/// 
	/// </summary>
	public override void Activate()
	{
		base.Activate();

		Reset();

		if (gameObject.activeSelf)
		{
			_ball.SimulationBegin += OnBallSimulationBegin;
			_ball.SimulationEnd += OnBallSimulationEnd;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public override void Deactivate()
	{
		_ball.SimulationBegin -= OnBallSimulationBegin;
		_ball.SimulationEnd -= OnBallSimulationEnd;
		IsClearing = false;
		_animController.IsGoalkeeper = false;

		base.Deactivate();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="difficultyLevel"></param>
	public void Setup(MatchBridge.Difficulty difficultyLevel = MatchBridge.Difficulty.EXTREME)
	{
		Difficulty = difficultyLevel;
	}



	#endregion  //End public methods

	//-----------------------------------------------------------//
	//                  MONOBEHAVIOUR METHODS                    //
	//-----------------------------------------------------------//

	#region Monobehaviour methods

	/// <summary>
	/// 
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		_pivotPosition = _targetPos = transform.position;

		_colliders = GetComponentsInChildren<Collider>();

		_clearSpeed = .5f * GoalSize.magnitude/SecsToClearCorner;
	}

	/// <summary>
	/// 
	/// </summary>
	private void OnDestroy()
	{
		_colliders = null;
	}

	
	/// <summary>
	/// 
	/// </summary>
	protected override void Update()
	{
		base.Update();

		if(_ball.State == PhysicBall.PhysicState.Attached)
			LookAtBall();

#if UNITY_EDITOR
		DebugSectors();
#endif
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="other"></param>
	protected override void OnTriggerEnter(Collider other)
	{
		InteractiveMatch.NotifyResult(InteractiveMatch.GameAction.Intercepted);

		float bounciness = _colliders.First(c => c.sharedMaterial != null)
			.sharedMaterial.bounciness;

		_ball.CollideWithPlane(_goalForward, bounciness);

		Vector3 myVelocity = _lastClearSecs > 0f ?
			(transform.position - _positionOnShootStart) / _lastClearSecs :
			Vector3.zero;

		_ball.AddVelocity(bounciness * myVelocity);

		AreCollidersActive = false;
	}


	#endregion  //End monobehaviour methods

	//-----------------------------------------------------------//
	//                      PRIVATE METHODS                      //
	//-----------------------------------------------------------//
	#region Private methods

	/// <summary>
	/// 
	/// </summary>
	private void LookAtBall()
	{
		Vector3 ballGroundPos = Vector3.ProjectOnPlane(_ball.Position, transform.up);

#if UNITY_EDITOR
		if (_debugSectors)
			ballGroundPos = Vector3.ProjectOnPlane(Camera.main.transform.position, transform.up);
#endif

		transform.forward = (ballGroundPos - _pivotPosition).normalized;
		transform.position = _pivotPosition + transform.forward*DistanceFromGoal;
		SetStartTargetPos(transform.position);
	}

	

	/// <summary>
	/// 
	/// </summary>
	protected override void SetDestination()
	{

	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="position"></param>
	/// <param name="velocity"></param>
	private void OnBallSimulationBegin(Vector3 position, Vector3 velocity)
	{
#if UNITY_EDITOR
		if (_debugSectors)
			return;
#endif
		if (!IsActive)
			return;

		LookAtBall();

		_goalForward = transform.forward;
		_positionOnShootStart = transform.position;
		var goalPlane = new Plane(_goalForward, _positionOnShootStart);

		var ray = new Ray(position, _ball.GroundVelocity);
		float distanceToGoal;
		if (!goalPlane.Raycast(ray, out distanceToGoal))
		{
			// TODO Tiro paralelo o en sentido contrario
			return;
		}

		float goalSecs;
		Vector3 goalPosition = _ball.PredictPositionAfterDistance(distanceToGoal, out goalSecs);
		TryClearBall(goalPosition, goalSecs);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="position"></param>
	private void OnBallSimulationEnd(Vector3 position)
	{
#if UNITY_EDITOR
		if (_debugSectors)
			return;
#endif

		IsClearing = false;

		if (_resetPositionAfterShoot)
			LookAtBall();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="goalPosition"></param>
	/// <param name="secsToGoal"></param>
	private void TryClearBall(Vector3 goalPosition, float secsToGoal = 0f)
	{
		Assert.Test(GoalSize.x > 0f && GoalSize.y > 0f, "AIGoalkeeper::TryClearBall>> Invalid goal size!!");

		Vector3 localPos = transform.InverseTransformPoint(goalPosition);
		if (localPos.z > CatchDistanceMin)
			return;

		localPos.z = 0f;

		if (Mathf.Abs(localPos.x) < .5f * GoalSizeTolerance*GoalSize.x
		 && Mathf.Abs(localPos.y) <       GoalSizeTolerance*GoalSize.y)
		{
			DoClear(localPos, secsToGoal);
		}
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="goalLocalPos"></param>
	/// <param name="secsToGoal"></param>
	private void DoClear(Vector3 goalLocalPos, float secsToGoal)
	{
		IsClearing = true;

		bool isRight = goalLocalPos.x < 0;
		Vector2 goalSector = CalcSector(goalLocalPos);

		float distanceDelay = Mathf.Min(SecsToClearCorner, 
			goalLocalPos.magnitude/_clearSpeed);

		Debug.Log("AIGoalkeeper::DoClear>> Delay=" + distanceDelay);

		if (Random.Range(0f, 1f) < CalcSectorSuccessRatio(goalSector))
			DoClearSuccess(goalSector, isRight, secsToGoal - distanceDelay);
		else
			DoClearFail(goalSector, isRight, secsToGoal - distanceDelay);
	}

	/// <summary>
	/// /
	/// </summary>
	/// <param name="sector"></param>
	/// <param name="isRight"></param>
	/// <param name="secsToGoal"></param>
	private void DoClearFail(Vector2 sector, bool isRight, float secsToGoal)
	{
		int qMax = Mathf.CeilToInt(.5f * GoalSectors.x) - 1;
		int lMax = Mathf.FloorToInt(GoalSectors.y)	    - 1;

		isRight = !isRight;
		sector.y = (sector.y + Random.Range(0, lMax - 1)) % lMax;

	/* UNDONE FRS 160609 Queda muy estúpido que el portero no se tire lateralmente en ClearFail
		sector.x = (sector.x + Random.Range(0, qMax - 1))%qMax;
	/*/
		sector.x = qMax;
	/**/

		DoClearSuccess(sector, isRight, secsToGoal);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sector"></param>
	/// <param name="isRight"></param>
	/// <param name="secsToGoal"></param>
	private void DoClearSuccess(Vector2 sector, bool isRight, float secsToGoal)
	{
		StartCoroutine(InvokeClearAnimation(sector, isRight, secsToGoal));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sector"></param>
	/// <param name="isRight"></param>
	/// <param name="secsToGoal"></param>
	/// <returns></returns>
	private IEnumerator InvokeClearAnimation(Vector2 sector, bool isRight, float secsToGoal)
	{
		int q = Mathf.Min((int) sector.x, SecsToClear.Length - 1);
		int l = Mathf.Min((int) sector.y, SecsToClear[q].Length - 1);
		_lastClearSecs = SecsToClear[q][l];

		float animSpeed = 1f;
		float delaySecs = secsToGoal - SecsToClear[q][l];
		if (delaySecs < 0f) {
			delaySecs = 0f;
			// UNDONE FRS 150602 Si se dispara a bocajarro el portero ajo y agua
			// animSpeed = SecsToClear[q][l]/secsToGoal;
		}

		Debug.Log("AIGoalkeeper::InvokeClearAnimation>> L" + l + "Q" + q 
			+ (isRight ? "_DER" : "_IZQ") 
			+ "; secsToGoal =" + secsToGoal + "; delay=" + delaySecs);

		if(delaySecs > 0f)
			yield return new WaitForSeconds(delaySecs);

		_animController.ClearBall(q, l, isRight, animSpeed);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="localPos"></param>
	/// <returns></returns>
	private Vector2 CalcSector(Vector3 localPos)
	{
		return new Vector2(
			CalcHorizontalSector(localPos.x),
			CalcVerticalSector(localPos.y));
	}

	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="posX"></param>
	/// <returns></returns>
	private int CalcHorizontalSector(float posX)
	{
//*
		return Mathf.RoundToInt(GoalSectors.x * Mathf.Abs(posX) / GoalSize.x);
/*/
		float x = Mathf.Abs(posX);
		int n = Mathf.CeilToInt(.5f * GoalSectors.x);
		float s = .5f * GoalSize.x;

		float f = 2f * x * (n - 1) / s;
		return Mathf.FloorToInt(n * f / (1f + f));
/**/
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="posY"></param>
	/// <returns></returns>
	private int CalcVerticalSector(float posY)
	{
//*
		return Mathf.FloorToInt(GoalSectors.y * Mathf.Abs(posY) / GoalSize.y);
/*/
		float y = Mathf.Abs(posY);
		int n = Mathf.FloorToInt(GoalSectors.y);
		float s = GoalSize.y;

		float f = 2f * y * (n - 1) / s;
		return Mathf.FloorToInt(n * f / (1f + f));
/**/
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sector"></param>
	/// <returns></returns>
	private float CalcSectorSuccessRatio(Vector2 sector)
	{
		float distance = sector.x + sector.y;

		bool isReached = distance < ReachByDifficulty[Difficulty] || 
			(Difficulty == MatchBridge.Difficulty.EASY && sector == Vector2.up);

		return isReached ? SuccessRatioMax : SuccessRatio;
	}


#if UNITY_EDITOR
	/// <summary>
	/// 
	/// </summary>
	private void DebugSectors()
	{
		if (!_debugSectors || !Input.GetMouseButtonDown(0))
			return;
		
		var goalPlane = new Plane( 
			(Camera.main.transform.position - transform.position).normalized, transform.position);

		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distanceToGoal;
		if (goalPlane.Raycast(ray, out distanceToGoal))
		{
			Vector3 goalPos = ray.origin + distanceToGoal * ray.direction;
			TryClearBall(goalPos);

			Debug.DrawRay(ray.origin, distanceToGoal * ray.direction, Color.red, 5f);
			Debug.Log("AIGoalkeeper::DebugSectors>> Time= " + Time.time);
			Debug.Break();
		}
	}
#endif

#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//

	#region Private members

	/// <summary>
	/// 
	/// </summary>
	private bool AreCollidersActive
	{
		set
		{
			if(_colliders != null)
				foreach (Collider c in _colliders)
					c.enabled = value;
		}
	}


	private MatchBridge.Difficulty _difficulty;

	private const float SecsToClearCorner = 1.00f;
	private const float DistanceFromGoal  = 1.00f;
	private const float CatchDistanceMin  = 5.00f;
	private const float SuccessRatioMax   = 1.00f;
	private const float GoalSizeTolerance = 1.80f;

	private static readonly Vector2 GoalSize = new Vector2(5.70f, 1.80f);
	private static readonly Vector2 GoalSectors = new Vector2(8f, 3f);

	private static readonly float[][] SecsToClear =
	{
		new [] { +0.300f, +0.300f, +0.400f},
		new [] { +0.400f, +0.000f, +0.600f},
		new [] { +0.300f, +0.200f, -0.150f},
		new [] { +0.300f, +0.200f, +0.080f},
	};


	private static readonly Dictionary<MatchBridge.Difficulty, float>
		SuccessByDifficulty = new Dictionary<MatchBridge.Difficulty, float>
	{
		{ MatchBridge.Difficulty.EASY,    0.25f },
		{ MatchBridge.Difficulty.MEDIUM,  0.50f },
		{ MatchBridge.Difficulty.HIGH,    0.75f },
		{ MatchBridge.Difficulty.EXTREME, 1.00f },
	};

	private static readonly Dictionary<MatchBridge.Difficulty, float> 
		ReachByDifficulty = new Dictionary<MatchBridge.Difficulty, float>
	{
		{ MatchBridge.Difficulty.EASY,		1f },
		{ MatchBridge.Difficulty.MEDIUM,    2f },
		{ MatchBridge.Difficulty.HIGH,		3f },
		{ MatchBridge.Difficulty.EXTREME,	4f },
	};



#if UNITY_EDITOR
	[SerializeField] private bool _debugSectors;
#endif
	[SerializeField] private bool _resetPositionAfterShoot;
	

	private Collider[] _colliders;

	private bool _isClearing;
	private float _lastClearSecs;

	private Vector3 _goalForward;
	private Vector3 _positionOnShootStart;
	private Vector3 _pivotPosition;
	private float _successRatio;
	private float _clearSpeed;

	#endregion  //End private members

	
}
