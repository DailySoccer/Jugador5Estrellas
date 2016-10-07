using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BallController))]
public class BallControllerEditor : Editor
{
	private BallController _controller;

	/// <summary>
	/// 
	/// </summary>
	public void Awake()
	{
		_controller = (BallController)target;
	}

	/// <summary>
	/// 
	/// </summary>
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Reset Position"))
			_controller.ResetPosition();
	}
}
