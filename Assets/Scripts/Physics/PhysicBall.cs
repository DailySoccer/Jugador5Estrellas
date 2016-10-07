using System;
using UnityEngine;
using UnityPhysics = UnityEngine.Physics;

namespace Assets.Scripts.Physics
{
	[RequireComponent(typeof(Rigidbody))]
	public class PhysicBall : MonoBehaviour
	{
		/// <summary>
		/// 
		/// </summary>
		public enum PhysicState
		{
			None		= 0,
			Stopped		= 1,
			Simulating	= 2,
			Attached	= 3,
		}

		/// <summary>
		/// 
		/// </summary>
		public struct PhysicData
		{
			public Vector3 P; // position
			public Vector3 V; // velocity

			public bool IsGrounded {
				get {
					return Mathf.Approximately(P.y + V.y, 0f);
				}
			}

			public bool IsMoving {
				get {
					return !IsGrounded || V.sqrMagnitude >  0f;
				}
			}

			public PhysicData(Vector3 p, Vector3 v)
			{
				P = p;
				V = v;
			}
		}

	
		//-----------------------------------------------------------//
		//                      PUBLIC MEMBERS                       //
		//-----------------------------------------------------------//
		#region Public members

		public event Action<Vector3, Vector3> SimulationBegin;
		public event Action<Vector3> SimulationEnd;

		/// <summary>
		///
		/// </summary>
		public Vector3 Position {
			get { return transform.position; }
			private set { LocalPosition = InverseTransformPoint(value); }
		}

		
		/// <summary>
		/// 
		/// </summary>
		public Vector3 Velocity {
			get { return TransformVector(LocalVelocity);  }
		}

	
		/// <summary>
		/// 
		/// </summary>
		public Vector3 GroundVelocity {
			get {
				return TransformVector( 
					new Vector3(LocalVelocity.x, 0f, LocalVelocity.z));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public PhysicState State
		{
			get { return _state; }
			private set
			{
				if (value == _state)
					return;

				MustUpdate = value != PhysicState.Stopped;
				PhysicState lastState = _state;
				_state = value;

				if (_state == PhysicState.Simulating)
					OnSimulationBegin(Position, Velocity);

				else if(lastState == PhysicState.Simulating)
					OnSimulationEnd(Position);
			}
		}



		#endregion  //End public members

		//-----------------------------------------------------------//
		//                      PUBLIC METHODS                       //
		//-----------------------------------------------------------//

		#region Public methods

		///// <summary>
		///// 
		///// </summary>
		///// <param name="target"></param>
		//// TODO Calculamos que es siempre un tiro rodado, mejorar con tirco con arc
		//public void ShootTowards(Vector3 target)
		//{
		//	HitTheGround();
		//	target.y = 0f;
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		public void SetPosition(Vector3 position)
		{
			Position = position;
			HitTheGround();
		}

		///  <summary>
		///  
		///  </summary>
		/// <param name="velocity"></param>
		// TODO Renombra a SetVelocity... y no hablar en términos de shoot salvo en controller
		public void AddVelocity(Vector3 velocity)
		{	
			UnAttach();

			LocalVelocity += InverseTransformVector(velocity);

			State = LocalVelocity != Vector3.zero ?
				PhysicState.Simulating : PhysicState.Stopped;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="velocity"></param>
		public void SetVelocity(Vector3 velocity)
		{
			UnAttach();

			LocalVelocity = InverseTransformVector(velocity);

			State = LocalVelocity != Vector3.zero ? 
				PhysicState.Simulating : PhysicState.Stopped;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		public void CollideWith(Collider other)
		{
			_lastCollision = other;

			PhysicData lastData = Simulate(_physicData, -2f*Time.fixedDeltaTime);
			Vector3 lastPos		= TransformPoint(lastData.P);
			Vector3 direction	= TransformVector(lastData.V).normalized;

			RaycastHit surfaceInfo;
			UnityPhysics.Raycast(lastPos + _sphere.radius * direction,
				direction, out surfaceInfo, 10f, 1 << other.gameObject.layer);

			Debug.DrawRay(lastPos, direction, Color.magenta, 5f);

			if (!surfaceInfo.collider == other)
				return;

			Position = surfaceInfo.point + 1.01f * _sphere.radius * surfaceInfo.normal;

			float bounciness = other.sharedMaterial != null ? other.sharedMaterial.bounciness : 1f;
			CollideWithPlane(surfaceInfo.normal, bounciness);
			
			Debug.DrawRay(surfaceInfo.point, surfaceInfo.normal, Color.green, 5f);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="normal"></param>
		/// <param name="bounciness"></param>
		public void CollideWithPlane(Vector3 normal, float bounciness = 1f)
		{
			//float bouncinessAvg = .5f * (_sphere.sharedMaterial.bounciness + bounciness);
			SetVelocity(bounciness * Vector3.Reflect(Velocity, normal));
		}

		/// <summary>
		/// 
		/// </summary>
		public void Stop()
		{
			SetVelocity(Vector3.zero);
			HitTheGround();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newParent"></param>
		/// <param name="localPosition"></param>
		public void AttachTo(Transform newParent, Vector3 localPosition)
		{
			if (State == PhysicState.Attached)
				UnAttach();

			_parentBeforeAttach = transform.parent;

			transform.SetParent(newParent);
			Position = newParent.position + localPosition;
			Stop();

			State = PhysicState.Attached;
		}

		/// <summary>
		/// 
		/// </summary>
		public void UnAttach()
		{
			if (State != PhysicState.Attached)
				return;

			transform.SetParent(_parentBeforeAttach);

			LocalPosition = InverseTransformPoint(transform.position);
			HitTheGround();

			State = PhysicState.Stopped;
		}

		/// <summary>
		/// Predics the position after an amount of time in secs.
		/// </summary>
		/// <param name="secs"></param>
		/// <returns></returns>
		public Vector3 PredictPositionAfterTime(float secs)
		{
			return TransformPoint(Simulate(_physicData, secs).P);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="meters"></param>
		/// <param name="secs"></param>
		/// <returns></returns>
		public Vector3 PredictPositionAfterDistance(float meters)
		{
			float secs;
			return PredictPositionAfterDistance(meters, out secs);
		}

		/// <summary>
		/// Predics the position after travelling a ground distance in meters.
		/// It also returns the amount of time needed to reach that position.
		/// </summary>
		/// <param name="meters"></param>
		/// <param name="secs"></param>
		/// <returns></returns>
			// TODO Revisar !IsMoving: devolver metros y segundos del stop?
		public Vector3 PredictPositionAfterDistance(float meters, out float secs)
		{
			Vector3 groundVelocity = LocalVelocity;
			groundVelocity.y = 0f;

			float speed = groundVelocity.magnitude;

			if (Mathf.Approximately(speed, 0f)) {
				secs = 0f;
				return Position;
			}

			secs = meters/speed;

			PhysicData data = Simulate(_physicData, secs);
			if (!data.IsGrounded || !data.IsMoving)
				return TransformPoint(data.P);
			
			speed = data.V.magnitude;
			float rollSecs = speed / _friction;
			float rollMeters = meters - Vector3.Distance(LocalPosition, data.P);

			Assert.Test(rollMeters > 0f, "PhysicBall::PredictPositionAfterDistance>> Meters shouldn't have been travelled yet");

			float sqrt = speed*speed - 2f*_friction*rollMeters;
			if (sqrt > 0f)
				rollSecs -= Mathf.Sqrt(sqrt) / _friction;

			secs += rollSecs;
			return TransformPoint(SimulateRoll(data, rollSecs).P);
		}

		#endregion  //End public methods

		//-----------------------------------------------------------//
		//                  MONOBEHAVIOUR METHODS                    //
		//-----------------------------------------------------------//
		#region Monobehaviour methods

		/// <summary>
		/// 
		/// </summary>
		private void Awake()
		{
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().isKinematic = true;

			if (_sphere == null)
				_sphere = GetComponentInChildren<SphereCollider>();

			Assert.Test(_sphere != null, "PhysicBall::Awake>> Sphere other not found!!");
			_sphere.isTrigger = true;
				
			_transform = CalcTransformMatrix();
			LocalPosition = InverseTransformPoint(transform.position);
			
			float gravityMagnitude = UnityPhysics.gravity.magnitude;
			_g = -gravityMagnitude * Vector3.up;
			_frictionMax = gravityMagnitude;
			_friction = _frictionMax * _sphere.sharedMaterial.dynamicFriction;		
		}
	

		/// <summary>
		/// 
		/// </summary>
		private void OnDestroy()
		{
			_lastCollision = null;
			_parentBeforeAttach = null;
			_sphere = null;
			_ground = null;
			_shadow = null;
		}

	
	
		/// <summary>
		/// 
		/// </summary>
		private void Start()
		{
			Stop();
		}

		/// <summary>
		/// 
		/// </summary>
		private void FixedUpdate()
		{
			_hasJustCollided = false;

#if UNITY_EDITOR
			_friction = _sphere.sharedMaterial.dynamicFriction * _frictionMax;
#endif

			Assert.Test(State != PhysicState.Stopped, "PhysicBall::Update>> Ball shouldn't update when stopped");

			if (!MustUpdate)
				return;

			if (State == PhysicState.Simulating)
			{
				UpdateSimulation(Time.fixedDeltaTime);
			}
			else
			{
				Vector3 pos = InverseTransformPoint(Position);
				LocalVelocity = (pos - LocalPosition) / Time.fixedDeltaTime;
				LocalPosition = pos;
			} 

			// TODO FRS Evaluar si sólo se actualiza la rotación grounded
			//if (_physicData.IsGrounded)
			UpdateRotation(Time.fixedDeltaTime);
		}


		// <summary>
		// 
		// </summary>
		// <param name="other"></param>
		private void OnTriggerEnter(Collider other)
		{
			if(!_hasJustCollided && other != _lastCollision 
				&& other.gameObject.layer == LayerMask.NameToLayer(_collisionableLayerName))
			{
				CollideWith(other);
				_hasJustCollided = true;
			}
		}

		
		

		#endregion  //End monobehaviour methods

		//-----------------------------------------------------------//
		//                      PRIVATE METHODS                      //
		//-----------------------------------------------------------//

		#region Private methods


		/// <summary>
		/// /
		/// </summary>
		private void OnSimulationBegin(Vector3 position, Vector3 velocity)
		{
			_lastCollision = null;
			 
			var e = SimulationBegin;
			if (e != null)
				e(position, velocity);
		}

		/// <summary>
		/// 
		/// </summary>
		private void OnSimulationEnd(Vector3 position)
		{
			//HitTheGround();
			var e = SimulationEnd;
			if (e != null)
				e(position);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t"></param>
		private void UpdateSimulation(float t)
		{
#if UNITY_EDITOR
			if (_inverseSimulation)
				t *= -1;
#endif
			PhysicData data = Simulate(_physicData, t);

#if UNITY_EDITOR				
			// UNDONE FRS 160511 Verificar que esto se cumple....
			//float deltaPos = (data.P - _physicData.P).magnitude;
			//Assert.Test(Mathf.Approximately(_lastDeltaPos, 0f) 
			//	|| deltaPos <= _lastDeltaPos, "PhysicBall::Update>> Incoherent displacement");
			//_lastDeltaPos = deltaPos;
#endif
			SetPhysicData(data);

			if (!data.IsMoving)
				State = PhysicState.Stopped;
		}

		

		/// <summary>
		/// // TODO FRS 150509 Optimizar
		/// </summary>
		private void UpdateRotation(float t)
		{
			Vector3 localRotation = Vector3.Cross(-_g.normalized, LocalVelocity) 
			                        * t / _sphere.radius * Mathf.Rad2Deg;

			Vector3 rotation = TransformVector(localRotation);

			_sphere.transform.Rotate(rotation, rotation.magnitude, Space.World);
		}

		/// <summary>
		/// 
		/// </summary>
		private void HitTheGround()
		{
			LocalPosition = new Vector3(LocalPosition.x, 0f, LocalPosition.z);
		}
	


		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		private PhysicData Simulate(PhysicData data, float t)
		{
			if (Mathf.Approximately(t, 0f))
				return data;

			return t > 0f && data.IsGrounded ?
				SimulateRoll(data,t) :
				SimulateBounce(data, t);
		}

		/// <summary>
		/// 
		/// </summary>
		private PhysicData SimulateBounce(PhysicData data, float t)
		{
			Assert.Test(data.P.y >= 0f || float.IsInfinity(t) ||float.IsNaN(t), 
				"PhysicBall::SimulateBounce>> Invalid physic state!!");

			if (float.IsInfinity(t) || float.IsNaN(t))
				t = 999f;
			
			Vector3 p = data.P + data.V*t + .5f*_g*t*t;
			if (p.y > 0f)
				return new PhysicData { 
					P = p,
					V = data.V + _g * t,
				};
				
			float fallSecs;
			float sign = Mathf.Sign(t);

			if (Mathf.Approximately(data.P.y, 0f)) {
				fallSecs = sign * 2f * Math.Abs(data.V.y / _g.y);

			} else {
				float sqrt = Mathf.Sqrt(data.V.y * data.V.y - 2f * _g.y * data.P.y);
				fallSecs = (-data.V.y - sqrt) / _g.y;
				if (fallSecs * t < 0f)			
					fallSecs = (-data.V.y + sqrt) / _g.y;
			}

			data.P += data.V * fallSecs;
			data.P.y = 0f;
			data.V += _g * fallSecs;
			t -= fallSecs;

			if (Mathf.Abs(data.V.y) > _bounceSpeedMin) {
				data.V.y *= -Mathf.Pow(_sphere.sharedMaterial.bounciness, sign);
				return SimulateBounce(data, t);

			} else {			
				return SimulateRoll(data, t);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private PhysicData SimulateRoll(PhysicData data, float t)
		{
			data.P.y = 0f;
			data.V.y = 0f;

			float speed = data.V.magnitude;
			float tMax = speed / _friction;
			Vector3 vu = Mathf.Approximately(speed, 0f) ? 
				Vector3.zero : 
				data.V / speed;

			if (t > tMax) {
				t = tMax;
				data.V = Vector3.zero;

			} else {
				data.V = Mathf.Max(0f, speed - _friction * t) * vu;
			}

			data.P += (speed - .5f * _friction * t) * t * vu;

			Assert.Test(!float.IsNaN(data.P.x + data.P.y + data.P.z), "PhysicBall::SimulateRoll>> Invalid data!!");

			return data;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private Matrix4x4 CalcTransformMatrix()
		{
			var matrix = Matrix4x4.identity;

			if (_ground == null) {
				GameObject groundGo = GameObject.FindGameObjectWithTag(_groundTag);
				if(groundGo != null)
					_ground = groundGo.GetComponent<Collider>();
			}

			if (_ground != null)
			{
				Vector3 rayDirection = _ground.transform.position - transform.position;

				RaycastHit groundInfo;
				bool groundFound = UnityPhysics.Raycast(transform.position,
					rayDirection.normalized, out groundInfo,  1.5f * rayDirection.magnitude, 
					LayerMask.GetMask(_groundLayerName) );

				Assert.Test(groundFound,
					"PhysicBall::CalcTransformMatrix>> Invalid raycast!!");

				var t = groundInfo.point + _sphere.radius * groundInfo.normal; 
				var r = Quaternion.FromToRotation(Vector3.up, groundInfo.normal);
				var s = Vector3.one;

				matrix.SetTRS(t, r, s);
			}		
	
			return matrix;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		private Vector3 TransformPoint(Vector3 point)
		{
			return _transform.MultiplyPoint3x4(point);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		private Vector3 InverseTransformPoint(Vector3 point)
		{
			return _transform.inverse.MultiplyPoint3x4(point);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		private Vector3 TransformVector(Vector3 direction)
		{
			return _transform.MultiplyVector(direction);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		private Vector3 InverseTransformVector(Vector3 direction)
		{
			return _transform.inverse.MultiplyVector(direction);
		}
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		private void SetPhysicData(PhysicData data)
		{
			LocalPosition = data.P;
			LocalVelocity = data.V;
		}

		#endregion  //End private methods

		//-----------------------------------------------------------//
		//                      PRIVATE MEMBERS                      //
		//-----------------------------------------------------------//
		#region Private members



		/// <summary>
		/// 
		/// </summary>
		private bool MustUpdate
		{
			get { return _mustUpdate; }
			set
			{
				enabled = value;  // Sleep
				_mustUpdate = value;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		private Vector3 LocalPosition
		{
			get { return _physicData.P; }
			set
			{
				Assert.Test(value.y >= 0f, "PhysicBall::LocalPosition>> Position.y under plane height!!!");
				Assert.Test(!float.IsNaN(value.x), "PhysicBall::LocalPosition>> NaN values!!!");

				if (float.IsNaN(value.x))
					return;

				value.y = Mathf.Max(value.y, 0f);
				_physicData.P = value;

				transform.position = TransformPoint(value);
				value.y = 0f;
				_shadow.position = TransformPoint(value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private Vector3 LocalVelocity
		{
			get { return _physicData.V; }
			set { _physicData.V = value; }
		}


		[SerializeField]
		private string _groundTag = "Cancha";
		[SerializeField]
		private string _groundLayerName = "Cancha";
		[SerializeField]
		private string _collisionableLayerName = "BallCollisionable";
		[SerializeField]
		private Collider _ground;
		[SerializeField]
		private SphereCollider _sphere;
		[SerializeField]
		private Transform _shadow;

		[SerializeField, Range(0.1f, 10)]
		private float _bounceSpeedMin = 1f;

#if UNITY_EDITOR
		[SerializeField]
		private bool _inverseSimulation;
		[HideInInspector]
		public float ShootRightValue = 10f;
		[HideInInspector]
		public float ShootUpValue = 9.5f;
		[HideInInspector]
		public float ShootForwardValue = 1.90f;
#endif


		private float _frictionMax;
		private float _friction;
	
		private Transform _parentBeforeAttach;
		private PhysicData _physicData;
		private PhysicState _state;

		private Matrix4x4 _transform;
		private Vector3 _g; // gravity


		private bool _mustUpdate;
		private Collider _lastCollision;
		private bool _hasJustCollided;

		#endregion  //End private members

		
	}
}
