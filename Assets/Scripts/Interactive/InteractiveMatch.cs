using UnityEngine;
using System.Collections;
using FootballStar.Match3D;
using FootballStar.Common;
using System.Collections.Generic;
using Assets.Scripts.AnimationController;
using Assets.Scripts.Physics;

public class InteractiveMatch : MonoBehaviour
{
	#region Public members
	public bool IsFinished
	{
		get { return _finished; }
	}
	public float FieldWidth = 70;
	public float FieldDepth = 100;
	public enum GameAction
	{
		Pass,
		Intercepted,
		Out,
		Dribbled,
		Goal
	}
	public static bool IsNotified
	{
		get { return _resultNotified; }
	}
	#endregion

	#region MonoBehaviour methods

	/// <summary>
	/// 
	/// </summary>
	private void Awake()
	{
		_successFeedback = _successFeedbackEditor;
		_failFeedback = _failFeedbackEditor;
	}

	/// <summary>
	/// 
	/// </summary>
	private void OnDestroy()
	{
		_matchRef	= null;
		_mainCamera	= null;
		_attackers	= null;
		_defenders	= null;
		_successFeedback = _successFeedbackEditor = null;
		_failFeedback = _failFeedbackEditor = null;
	}

	static public InteractiveMatch Instance;

	/// <summary>
	/// 
	/// </summary>
	private void Start()
	{
		_finished = false;
		_active = false;
	}

	/// <summary>
	/// 
	/// </summary>
	private void Update()
	{
		if (_active)
		{
			if (!_stepFinished)
			{
				if (_resultNotified)
				{
					EndAction();
				}
			}
			else
			{
				FinishAction();
			}
		}
	}
	#endregion

	#region Public methods
	/// <summary>
	/// Start an interactive play.
	/// </summary>
	/// <param name="matchRef"></param>
	public void Play(MatchManager matchRef, MatchBridge.Difficulty difficultyLevel, Camera mainCamera, InteractiveType action)
	{
		Instance = this;
		AIAgent.SetMatchManager(this);
		_mainCamera = mainCamera;
		_resultNotified = false;
		_finished = false;
		_stepFinished = false;
		_matchRef = matchRef;
		_lastTime = Time.time;
		_mainCamera.transform.position = new Vector3(-10, 2, 0);
		CameraFade.Fade(true, 1.0f, 0.0f);
		if (_attackers == null || _defenders == null || _attackers[0] == null || _defenders[0] == null)
		{
			List<GameObject> _origAt = _matchRef.GetAttackers();
			List<GameObject> _origDef = _matchRef.GetDefenders();
			if (_attackers != null)
			{
				_attackers.Clear();
				_attackers = null;
			}
			if (_defenders != null)
			{
				_defenders.Clear();
				_defenders = null;
			}
			_attackers = new List<GameObject>(_origAt.Count);
			_defenders = new List<GameObject>(_origDef.Count);
			for (int i = 0; i < _origAt.Count; ++i)
			{
				_attackers.Add(_origAt[i]);
				_defenders.Add(_origDef[i]);
			}
		}
		Init(action, difficultyLevel);
		if (_matchRef.TheBall != null)
		{
			if (_mainCamera != null)
			{
				ResetParent camRep = _mainCamera.gameObject.GetComponent<ResetParent>();
				if (camRep != null)
				{
					float fovCam = 60;
					switch (action) {
						case InteractiveType.Shot:
							fovCam = 60;
							camRep.SetNewParent(_matchRef.TheBall, new Vector3(0, 3, -7), _attackers[PlaySpawner.PlayerDorsal].transform.position + _attackers[PlaySpawner.PlayerDorsal].transform.forward * 5, fovCam, _attackers[PlaySpawner.PlayerDorsal].transform);
							break;
						case InteractiveType.Pass:
							fovCam = 25;
							camRep.SetNewParent(_matchRef.TheBall, new Vector3(0, 20, -30), _attackers[PlaySpawner.PlayerDorsal].transform.position + _attackers[PlaySpawner.PlayerDorsal].transform.forward * 7, fovCam, _attackers[PlaySpawner.PlayerDorsal].transform);
							break;
						case InteractiveType.Dribling:
							fovCam = 60;
							camRep.SetNewParent(_matchRef.TheBall, new Vector3(0, 3, -7), _attackers[PlaySpawner.PlayerDorsal].transform.position + _attackers[PlaySpawner.PlayerDorsal].transform.forward * 5, fovCam, _attackers[PlaySpawner.PlayerDorsal].transform);
							break;
					}
					//fovCam = 25;
					//camRep.SetNewParent(_matchRef.TheBall, new Vector3(0, 20, -30), _attackers[PlaySpawner.PlayerDorsal].transform.position + _attackers[PlaySpawner.PlayerDorsal].transform.forward * 7, fovCam, _attackers[PlaySpawner.PlayerDorsal].transform);
				}
			}
			_matchRef.TheBall.GetComponent<PhysicBall>().SimulationBegin -= AIAttacker.calculateNearest;
			_matchRef.TheBall.GetComponent<PhysicBall>().SimulationBegin += AIAttacker.calculateNearest;
			_matchRef.TheBall.GetComponent<PhysicBall>().SimulationBegin -= AIDefender.calculateNearest;
			_matchRef.TheBall.GetComponent<PhysicBall>().SimulationBegin += AIDefender.calculateNearest;
			_matchRef.TheBall.GetComponent<BallController>().Setup(action);
		}
		_active = true;
	}

	/// <summary>
	/// Finish an interactive play.
	/// </summary>
	public void End()
	{
		AIAgent [] agents = GameObject.FindObjectsOfType<AIAgent>();
		foreach (AIAgent ag in agents)
		{
			ag.Deactivate();
		}
		AIAttacker.Reset();
		AIDefender.Reset();
	}
	public static void NotifyResult(GameAction action)
	{
		if (_active && !_resultNotified)
		{
			Debug.Log("<color=teal>InteractiveMatch::NotifyResult>> </color>" + action);
			_resultNotified = true;
			switch (action)
			{
				case GameAction.Intercepted:
				case GameAction.Out:
					_res1 = _res2 = false;
					break;
				case GameAction.Pass:
					_res2 = false;
					_res1 = (PlaySpawner.CurrentPlay() as PassPlaySpawner) != null;
					break;
				case GameAction.Goal:
					_res1 = _res2 = (PlaySpawner.CurrentPlay() as ShootPlaySpawner) != null;
					Instance._matchRef.Gol();
					break;
				case GameAction.Dribbled:
					_res2 = false;
					_res1 = (PlaySpawner.CurrentPlay() as DribblingPlaySpawner) != null;
					break;
			}

			PlayFeedback(_res1);
		}
	}



	#endregion

	#region Private methods

	/// <summary>
	/// 
	/// </summary>
	/// <param name="isSuccess"></param>
	private static void PlayFeedback(bool isSuccess)
	{
		Animator feedbackToPlay = isSuccess ? _successFeedback : _failFeedback;
		if(feedbackToPlay != null)
			feedbackToPlay.SetTrigger("Start");
	}


	private void CalculateResult()
	{
		_matchRef.FinDeJugada(_res1, _res2, _res1 ? 1 : 0);
		_active = false;
	}
	private void EndAction()
	{
		CameraFade.Fade(false, _fadeDuration, _fadeDelay);
		_stepFinished = true;
	}
	private void FinishAction()
	{
		if (CameraFade.Finished)
		{
			CalculateResult();
			_finished = true;
		}
	}
	private void Init(InteractiveType action, MatchBridge.Difficulty difficultyLevel)
	{
		PlaySpawner.Init(this, FieldWidth, FieldDepth);
		PlaySpawner.CreatePlay(gameObject, action, difficultyLevel);
	}
	public List<GameObject> GetAttackers()
	{
		return _attackers;
	}
	public List<GameObject> GetDefenders()
	{
		return _defenders;
	}
	public Transform TheBall
	{
		get { return _matchRef.TheBall; }
	}

	
	#endregion

	#region Private members

	private bool _stepFinished;
	private bool _finished;
	private static bool _active;
	private float _lastTime;
	
	private static bool _resultNotified;
	private static bool _res1;
	private static bool _res2;

	private MatchManager _matchRef;
	private Camera _mainCamera;
	private static List<GameObject> _attackers;
	private static List<GameObject> _defenders;
	private static Animator _successFeedback;
	private static Animator _failFeedback;

	[SerializeField] private Animator _successFeedbackEditor;
	[SerializeField] private Animator _failFeedbackEditor;
	[SerializeField, Range(0f, 10f)] private float _fadeDuration = 2f;
	[SerializeField, Range(0f, 10f)] private float _fadeDelay = 2f;

	#endregion
}
