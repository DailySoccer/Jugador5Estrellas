using UnityEngine;
using System.Collections;
using Assets.Scripts.TouchControl;

public class AIPlayer : AIAgent
{

	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	public bool IsDribbling
	{
		get { return _isDribbling; }
	}
	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods
	public override void SetStartTargetPos(Vector3 targetPos)
	{
		_targetPos = targetPos;
	}
	public void Reset()
	{
		_hasShot = false;
		_blockedShot = false;
	}
	public static void SetInvulnerabilityDribblingTime(float seconds)
	{
		DRIBBLING_INVULNERABILITY = Mathf.Clamp(seconds, 0, 1.2f);
		DRIBBLING_INVUlNERABILITY_STEP = (1.2f - DRIBBLING_INVULNERABILITY) * 0.5f;
	}
	public static void ForceHasShot()
	{
		if (!_blockedShot)
		{
			_blockedShot = true;
		}
	}
	#endregion  //End public methods

	//-----------------------------------------------------------//
	//                  MONOBEHAVIOUR METHODS                    //
	//-----------------------------------------------------------//
	#region Monobehaviour methods

	// Use this for initialization
	protected override void Awake()
	{
		base.Awake();
		_hasShot = false;
		TouchInputManager touchRef = GameObject.FindObjectOfType<TouchInputManager>();
		touchRef.Swipe -= OnSwipe;
		touchRef.Swipe += OnSwipe;
		touchRef.DoubleTap -= OnDribbling;
		touchRef.DoubleTap += OnDribbling;
	}

	protected override void Update()
	{
		base.Update();
		if (!_hasShot && (_targetPos - transform.position).sqrMagnitude < 2)
		{
			InteractiveMatch.NotifyResult(InteractiveMatch.GameAction.Dribbled);
			_targetPos += transform.forward * 5;
		}
	}
	#endregion  //End monobehaviour methods

	//-----------------------------------------------------------//
	//                      PRIVATE METHODS                      //
	//-----------------------------------------------------------//
	#region Private methods
	private IEnumerator ShootAnim()
	{
		yield return new WaitForSeconds(22f / 30);
		if (!_blockedShot)
		{
			_ball.GetComponent<BallController>().Shoot(_swipeDirection, _swipeSpeedRatio);
		}
		_swipeDirection = Vector2.zero;
		_swipeSpeedRatio = 0;
	}
	private IEnumerator Dribbled()
	{
		yield return new WaitForSeconds(DRIBBLING_INVUlNERABILITY_STEP);
		_isDribbling = true;
		yield return new WaitForSeconds(DRIBBLING_INVULNERABILITY);
		_isDribbling = false;
		_animController.FixHeight();
		_animController.UpdateRotation = true;
	}
	private void OnSwipe(Vector2 swipe, float speedRatio)
	{
		if (IsActive && !_hasShot && swipe.y >= 0 && !InteractiveMatch.IsNotified)
		{
			Shoot();
			_swipeSpeedRatio = speedRatio;
			_swipeDirection = swipe;
			_hasShot = true;
			StartCoroutine(ShootAnim());
		}
	}
	public void OnDribbling(Vector2 tapPos)
	{
		if (IsActive && !_isDribbling && !InteractiveMatch.IsNotified)
		{
			Dribbling(tapPos);
			_animController.DoNotFixHeight();
			_animController.UpdateRotation = false;
			StartCoroutine(Dribbled());
		}
	}
	protected override void SetDestination()
	{
	}
	protected override void OnTriggerEnter(Collider other)
	{
	}
	#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//
	#region Private members
	private Vector2 _swipeDirection;
	private float _swipeSpeedRatio;
	private bool _hasShot;
	public static bool _blockedShot;
	private bool _isDribbling;
	private static float DRIBBLING_INVULNERABILITY = 1.2f;
	private static float DRIBBLING_INVUlNERABILITY_STEP = 0;
	#endregion  //End private members
}
