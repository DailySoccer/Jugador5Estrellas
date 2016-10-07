using UnityEngine;
using System.Collections;
using FootballStar.Match3D;
using Assets.Scripts.AnimationController;
using Assets.Scripts.TouchControl;
using Assets.Scripts.Physics;
using FootballStar.Common;

public abstract class AIAgent : MonoBehaviour
{
	//-----------------------------------------------------------//
	//                      PUBLIC MEMBERS                       //
	//-----------------------------------------------------------//
	#region Public members
	#endregion  //End public members

	//-----------------------------------------------------------//
	//                      PUBLIC METHODS                       //
	//-----------------------------------------------------------//
	#region Public methods
	public virtual void Activate()
	{
		Debug.Log("AIAgent::Activate>>" + name);

		if (!_active)
		{
			SetActive(true);
		}
	}
	public virtual void Deactivate()
	{
		if (_active)
		{
			SetActive(false);
		}
	}
	public void Tackling()
	{
		_animController.Tackling();
	}
	public void Shoot()
	{
		_animController.Shoot();
	}
	public void Dribbling(Vector2 tapPos)
	{
		_animController.Dribbling();
	}
	public virtual void SetStartTargetPos(Vector3 targetPos)
	{
		_targetPos = targetPos;
	}
	#endregion  //End public methods

	//-----------------------------------------------------------//
	//                  PUBLIC STATIC METHODS                    //
	//-----------------------------------------------------------//
	#region Public static methods
	public static void SetMatchManager(InteractiveMatch theMatch)
	{
		_matchRef = theMatch;
	}
	public static void SetBall(Transform theBall)
	{
		_ball = theBall.GetComponent<PhysicBall>();
	}
	#endregion //End public static methods

	//-----------------------------------------------------------//
	//                  MONOBEHAVIOUR METHODS                    //
	//-----------------------------------------------------------//
	#region Monobehaviour methods
	// Use this for initialization
	protected virtual void Awake()
	{
		_animator = GetComponent<Animator>();
		_animController = GetComponent<AnimFootballPlayer>();
		_active = false;
		_desiredSpeed = 0;
	}
	protected virtual void Start()
	{
	}
	// Update is called once per frame
	protected virtual void Update()
	{
		if (_active)
		{
			UpdateAITarget();
			Action();
			_animController.OwnerUpdate();
		}
	}
	protected abstract void OnTriggerEnter(Collider other);
	public void TriggerEnter(Collider other)
	{
		if (_active && !InteractiveMatch.IsNotified)
		{
			OnTriggerEnter(other);
		}
	}
	//void OnAnimatorMove()
	//{
	//	if (_active)
	//	{
	//		_desiredSpeed = animator.deltaPosition/* * Time.deltaTime*/;
	//		if (_desiredSpeed == Vector3.zero)
	//		{
	//			if (!AgentDone())
	//			{
	//				_desiredSpeed = transform.forward /** Time.deltaTime*/;
	//			}
	//		}
	//		else
	//		{
	//			Debug.Log("PArece que tengo valor, putaaaaa: "+_desiredSpeed);
	//		}
	//		transform.rotation = animator.rootRotation;
	//	}
	//}

	#endregion  //End monobehaviour methods

	//-----------------------------------------------------------//
	//                      PRIVATE METHODS                      //
	//-----------------------------------------------------------//
	#region Private methods
	protected abstract void SetDestination();
	protected void SetupAgentLocomotion()
	{
		if (_active)
		{
			if (AgentDone())
			{
				_animController.GoTo(transform.position);
			}
			else
			{
				_animController.GoTo(_targetPos);
				//Vector3 toTarget = _targetPos - transform.position;
				//float straightFactor = Vector3.Dot(transform.right, toTarget);
				//float angleTowards = Vector3.Angle(transform.forward, toTarget) * Mathf.Sign(straightFactor);
				//float speed = _desiredSpeed != 0 ? (Mathf.Abs(angleTowards) < 15 ? 5 : (Mathf.Abs(angleTowards) < 30 ? 2.5f : 0)) : 5;
				///*float speed = _desiredSpeed.magnitude;

				//Vector3 velocity = Quaternion.Inverse(transform.rotation) * _desiredSpeed;

				//float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;

				//locomotion.Do(speed, angle);*/

				//locomotion.Do(speed, angleTowards);
				//_desiredSpeed = speed;
			}
		}
	}
	protected bool AgentDone()
	{
		return AgentStopping();
	}
	protected bool AgentStopping()
	{
		return (_targetPos - transform.position).sqrMagnitude < 3;
	}
	private void CheckTargetPos()
	{
		//TODO: calculate target
		SetDestination();
		//TODO: compare new target with previous target
		//TODO: update the target if neccesary
	}
	private void UpdateAITarget()
	{
		CheckTargetPos();
		UpdateSpeed();
		SetupAgentLocomotion();
	}
	private void SetActive(bool active)
	{
		_active = active;
		if (!_active)
		{
			_animController.Reset();
			_targetPos = transform.position;
		}
	}
	private void Action()
	{
	}
	private void UpdateSpeed()
	{
		if (_active)
		{
			//_desiredSpeed = animator.deltaPosition/* * Time.deltaTime*/;
			//if (_desiredSpeed == Vector3.zero)
			//{
			//	if (!AgentDone())
			//	{
			//		_desiredSpeed = transform.forward /** Time.deltaTime*/;
			//	}
			//}
			//transform.rotation = animator.rootRotation;
		}
	}
	#endregion  //End private methods

	//-----------------------------------------------------------//
	//                      PRIVATE MEMBERS                      //
	//-----------------------------------------------------------//
	#region Private members
	protected Animator _animator;
	protected AnimFootballPlayer _animController;
	private bool _active;
	protected bool IsActive
	{
		get { return _active; }
	}
	private float _desiredSpeed;
	protected Vector3 _targetPos;
	#endregion  //End private members

	#region Private static members
	protected static PhysicBall _ball;
	protected static InteractiveMatch _matchRef;
	protected static System.Random _rnd = new System.Random();
	#endregion

	
}
