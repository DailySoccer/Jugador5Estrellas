using UnityEngine;
using System.Collections;

public class NarratorUI : MonoBehaviour {

	public GameObject narratorSlot;
    public static NarratorUI Instance { get; private set; }

    public UISprite MyBadge;
    public UISprite OpponentBadge;
    public UILabel MyScoreLabel;
    public UILabel OpponentScoreLabel;
	public UILabel SkippingButtonLabel;

    public int MyScore = 0;
    public int OpponentScore = 0;

	public UIFont fontRegular;
	public UIFont fontBold;


    void Awake()
    {
        Instance = this;
		SincronizeSkkipingLabel ();
    }

	void OnEnable() {
		SincronizeSkkipingLabel ();
	}


	public void AddGoal() {
		NarratorUI.Instance.MyScore++;
		UpdateScore ();
	}

    // Use this for initialization
    public void Start () {
        var matchBridge = GameObject.FindGameObjectWithTag("GameModel").GetComponent<FootballStar.Common.MatchBridge>();

        MyBadge.spriteName = matchBridge.CurrentMatchDefinition.MyBadgeName + "_Small";
        OpponentBadge.spriteName = matchBridge.CurrentMatchDefinition.OpponentBadgeName + "_Small";

        MyScore = 0;
        OpponentScore = 0;
        UpdateScore();

        if(_grid==null) _grid = GetComponentInChildren<UITable> ();
		if(_sv==null) _sv = GetComponentInChildren<UIScrollView> ();

        _sv.relativePositionOnReset = new Vector2(0, 0);
    }

    public void Flush() {
        while (_grid.transform.childCount != 0) {
            GameObject.DestroyImmediate(_grid.transform.GetChild(0).gameObject);
        }

    }

    // Update is called once per frame
    void Update () {

	}

    GameObject LastSlot;
	public void AddMessage(string minute, string text, bool isGoal, NarratorMessageType type, bool reevaluate, bool isPlayerInteraction) {
        if (isGoal) {
            if (type == NarratorMessageType.LocalPlayerMessage) MyScore++;
            else if (type == NarratorMessageType.VisitantPlayerMessage) OpponentScore++;
            UpdateScore();
        }
		_grid.Reposition ();
        int slotCount = _grid.transform.childCount;
        if(!reevaluate) LastSlot = NGUITools.AddChild(_grid.gameObject, narratorSlot);
        LastSlot.name = "slot_" + slotCount;

        LastSlot.GetComponent<narratorSlot> ().SetUpSlot (minute, text, isGoal, type);

		UILabel slotText = LastSlot.transform.FindChild ("TextoJugada").GetComponent<UILabel> ();
		slotText.bitmapFont = isPlayerInteraction ? fontBold : fontRegular;
		slotText.transform.localPosition = new Vector3 (
			slotText.transform.localPosition.x,
			slotText.transform.localPosition.y + (isPlayerInteraction ? 10.0f : 0.0f),
			slotText.transform.localPosition.z
		);

		NGUITools.SetActive(LastSlot, true);
		_grid.Reposition ();
		_sv.ResetPosition ();
		if (slotCount >= 5) {
			_sv.relativePositionOnReset = new Vector2 (0, 1);
		}
	}
    void UpdateScore()
    {
        MyScoreLabel.text = MyScore.ToString();
        OpponentScoreLabel.text = OpponentScore.ToString();
    }

	public void OnSkipClick()
	{
		ManoloLama.Instance.Skipping = !ManoloLama.Instance.Skipping;
		SincronizeSkkipingLabel();
	}

	public void SincronizeSkkipingLabel()
	{
		SkippingButtonLabel.text = ManoloLama.Instance.Skipping ? "AVANCE\nNORMAL" : "AVANCE\nRÁPIDO";
	}

    public void OnExitClick() {
        MyScore = 0;
        OpponentScore = 0;
        ManoloLama.Instance.Abort = true;
    }

	UITable _grid;
	UIScrollView _sv;
}
