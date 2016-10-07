//#define USE_FAKE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Audio;

using FootballStar.Match3D;
using FootballStar.Common;

public enum InteractiveType
{
    None,    
    Dribling,
    Pass,
    Shot,
    Init,
    Middle,
    End,
    All,
};

public class ManoloLama : MonoBehaviour
{
	private static string[] PassInteractive = { "Oportunidad de [ffa500]PASE[-] para {0}.", "Y lo consigue", "Pero la pierde" };
	private static string[] ShotInteractive = { "Oportunidad de [ffa500]TIRO[-] para {0}.", "GOOOOOL, un disparo perfecto", "Perdio una clara oportunidad" };
	private static string[] DribblingInteractive = { "Oportunidad de [ffa500]REGATE[-] para {0}.", "y se va solo", "Pero, el {1} recupera la posesion" };
	
	private static string[,] PassLogs = { { "Buena combinación del {0} saliendo desde atrás.", "Y avanza el delantero del {0} sólo", "Y finalmente se le escapa el balón" },
		{ "Monta la contra el {0}.", "Y avanza el delantero del {0} sólo", "Y finalmente se le escapa el balón" },
		{ "Monta la contra el {0}.", "Y avanza el delantero del {0} sólo", "Fuera de juego del {0}. Clarísimo esta vez." },
		{ "Monta la contra el {0}.", "Y avanza el delantero del {0} sólo", "Dudoso el fuera de juego del {0}" },
		{ "Monta la contra el {0}.", "Y avanza el delantero del {0} sólo", "Pero el defensa del {1} la corta" },
		{ "Falta del {0}.", "La señala el árbitro. Falta a favor del {0}", "Pero el colegiado no la pita" },
		{ "Falta del {0}.", "La señala el árbitro. Falta a favor del {0}", "Y tarjeta amarilla para el {0}. " },
		{ "Falta del {0}.", "Y tarjeta amarilla para el {0}", "Pero el colegiado no la pita" },
		{ "Corta el balón con la mano", "Tarjeta amarilla para el jugador del {0}", "Pero el colegiado no la pita" },
		{ "Le da el balón en el brazo", "Tarjeta amarilla para el jugador del {0}", "Pero el colegiado no la pita" },
		{ "Agarrón para cortar la jugada", "La señala el árbitro. Falta a favor del {0}", "Pero el colegiado no la pita" },
		{ "Dura entrada del {0}", "Roja directa, menuda entrada", "Pero el colegiado no la pita" },
		{ "Ahora mismo el centro del campo está atascado...", "...pero el  {0} logra verlo claro en esta ocasión", "...y encima pierde el balón el {0}" },
		{ "Monta la contra el {0}.", "Y avanza el delantero del {0} sólo", "Pero el defensa del {1} la corta" },
		{ "Roba ahora el {0}...", "..y logra montar una jugada peligrosa. Veremos a ver qué ocurre!", "...pero no llega a trenzar la jugada" },
		{ "Buena combinación del {0} saliendo desde atrás.", "Y logra montar una jugada peligrosa", "Pero aparece el capitán contrario y corta la jugada" },
		{ "Buena combinación del {0} saliendo desde atrás.", "Que bien juega ahora el {0}", "Pero ese último pase sobraba. Será saque de banda" },
		{ "Buena combinación del {0} saliendo desde atrás.", "Esto sí es fútbol. ¡Qué partidazo!", "Pero el defensa del {1} lee la jugada y la corta." },
		{ "Buena combinación del {0} saliendo desde atrás.", "Menudo rondo, más de 20 pases", "Pero la defensa del {1} despeja sin contemplaciones" },
		{ "Monta la contra el {0}.", "Y controlan el juego en campo contrario", "Pero finalmente se le escapa el balón" },
		{ "Monta la contra el {0}.", "Qué rápido circula ese balón entre líneas", "Fuera de juego del {0}. Clarísimo esta vez." },
		{ "Monta la contra el {0}.", "Qué bien se mueve el mediapunta del {0} ", "Dudoso el fuera de juego del {0}" },
		{ "Monta la contra el {0}.", "En dos pases avanzan 70 metros", "Pero el defensa del {1} la corta" },
		{ "Falta del {0}.", "El árbitro advierte al jugador", "No la vio el colegiado. Parecía clara" },
		{ "Falta del {0}.", "Clarísima. La señala el árbitro.", "Y tarjeta amarilla para el {0}. " },
		{ "Falta del {0}.", "Y tarjeta amarilla para el {0}", "Logra cortar la jugada el {1}" },
		{ "Corta el balón con la mano", "Tarjeta amarilla para el jugador del {0}", "Pero el colegiado no la pita" },
		{ "Le da el balón en el brazo", "Pudo ser involuntario, pero lo pitó.", "Pero el colegiado deja seguir" },
		{ "Agarrón para cortar la jugada", "La señala el árbitro. Falta a favor del {0}", "Que le cuesta la tarjeta amarilla" },
		{ "Dura entrada del {0}", "Roja directa, menuda entrada", "No la pita. Se equivocó el colegiado" },
		{ "Dura entrada del {0}", "Se hizo daño el jugador del {1}", "Pero el colegiado deja seguir" },
		{ "Lanzamiento directo del {1}", "¡UYYY! Rozando el larguero", "A la barrera" },
		{ "Lanzamiento directo del {1}", "¡Al palo! Que efecto llevaba ese balón", "Directamente fuera" },
		{ "Lanzamiento directo del {1}", "¡UYYY! Por poco", "Pero el portero ataja sin problemas" },
		{ "Lanzamiento directo del {1}", "A la grada. Alto ese golpeo", "Y despeja el portero." },
		{ "Golpea con la pierna derecha", "A la grada. Alto ese golpeo", "Despeja el portero con los puños." },
		{ "Golpea con la pierna derecha", "¡UYYY! Qué precisión, pero se va fuera el balón", "Pero el portero detiene sin problemas" },
		{ "Golpea con la pierna derecha", "Le dió mal. Se lamenta el jugador", "Y despeja el portero." },
		{ "Dura entrada del {0}", "Y tarjeta amarilla para el {0}", "Pero el colegiado deja seguir" },};
	
	private static string[,] ShotLogs = { { "Prueba el {0} desde la frontal", "Toca en un defensor y GOOOL", "Al palo. Qué oportunidad" },
		{ "Prueba el {0} desde la frontal", "GOOOOOL. Se la tragó el portero", "Pero, se va alto" },
		{ "Balón al hueco del {0}", "Remata de primeras, y GOOOOOL", "Pero no encuentra compañero" },
		{ "Balón al hueco del {0}", "Remata totalmente solo, y GOOOOOL", "Y el delantero no llega por poco" },
		{ "Pase de la muerte del {0}", "Remata de primeras, y GOOOOOL", "Pero no encuentra compañero" },
		{ "Pase de la muerte del {0}", "Remata totalmente solo, y GOOOOOL", "Y el delantero no llega por poco" },
		{ "Lanzamiento directo del {0}", "GOOOOOL, menuda cantada", "Directamente fuera" },
		{ "Lanzamiento directo del {0}", "GOOOOOL, un disparo perfecto", "Y despeja el portero" },
		{ "Lanzamiento directo del {0}", "GOOOOOOL, por toda la escuadra", "A la barrera" },
		{ "Balón al interior del área", "El defensa despeja mal, GOOOL. En propia puerta", "Despeja la defensa" },
		{ "Balón al interior del área", "Remata dentro del área pequeña y GOOOOOOL", "Y despeja el portero" },
		{ "Prueba el {0} desde la frontal", " Muy ajustado y ….¡GOOOL!", "Al palo. Qué oportunidad" }, 
		{ "Prueba el {0} desde la frontal", "GOOOOOL. Que bien golpeó el balón. Precioso Tanto", "Pero metió la pierna un defesa" },
		{ "Balón al hueco del {0}", "Remata de primeras, y GOOOOOL", "Que se pierde por la línea de fondo" },
		{ "Balón al hueco del {0}", "Perfecto movimiento del delantero y …. GOOOOOL", "Y el delantero no llega por poco" },
		{ "Pase de la muerte del {0}", "Remata de primeras, y GOOOOOL", "Pero nadie acompañaba la jugada" }, 
		{ "Pase de la muerte del {0}", "GOOOOOL. Qué golazo. Remató a placer", "Le da mal. Perdonó esta vez el {0}" },
		{ "Balón a la frontal del área {0}", "GOOOOOL, menuda cantada del portero", "Directamente fuera" },
		{ "Centro medido al segundo palo del {0}", "GOOOOOL,", "Y despeja el portero" },
		{ "Lanzamiento directo del {0}", "GOOOOOOL, botó delante del portero y entró", "A la barrera" },
		{ "Balón al interior del área", "El defensa despeja mal, GOOOL. En propia puerta", "Despeja la defensa" },
		{ "Balón al interior del área", "Remata dentro del área pequeña y GOOOOOOL", "Y despeja el portero" },
		{ "Balón al hueco del {0}", "Remata totalmente solo, y GOOOOOL", "Y el delantero no llega por poco" },
		{ "Zurdazo desde la frontal del {0}", "GOOOOOL", "Al segundo anfiteatro" }};
	
	private static string[,] DribblingLogs = { { "Intento de regate del {0}", "Gran regate, se queda solo", "Pero el defensa corta el balon" },
		{ "Intento de regate del {0}", "Supera a su marcador", "Pero se dejo atras el balón" },
		{ "Intento de regate del {0} ", "Lo consigue, y continua el ataque", "Pero no tiene éxito" },
		{ "Encara al defensor el delantero de {0}", "Y lo deja atrás. ¡Qué rápido es!", "Pero el defensa estuvo rápido" },
		{ "Avanza con el cuero el central del {0}", "Que inicia muy bien la jugada de ataque.", "Pero se hace un lío y pierde el balón" },
		{ "Gran movimiento del delantero del {0} ", "Que recibe un pase perfecto.", "Pero no llega al pase, demasiado fuerte" },
		{ "Autopase del jugador del {0}", "Qué velocidad. Avanza sólo.", "Pero se le va el balón" },
		{ "Intenta irse de su defensor", "Gran regate, se queda solo", "Pero pierde el balon" }};

	public class Momento
	{
		public int mTiempo;
		public bool mLocal;
		public InteractiveType mType;
		public bool mUser;
		public bool mUserCopy;
		public bool mExito;
		public int mNivel;
		public float mRandom;
		public float mRandom2;

		public Momento(int tiempo, InteractiveType type = InteractiveType.None)
		{
			mTiempo = tiempo;
			mType = type;
			mLocal = false;
			mUserCopy = false;
			mUser = false;
			mExito = false;
			mNivel = 0;
			mRandom = 0;
		}

		public override string ToString()
		{
			return string.Format("t: {0} {1} {2} {3} {4}", mTiempo, mType, mLocal ? "Local" : "Visitante", mUser ? "MAN" : "AUT", mExito);
		}
	}
	List<Momento> mMoments = new List<Momento>();

	public static ManoloLama Instance
	{
		get {
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<ManoloLama>();
			}
			return _instance; }
	}

	private static ManoloLama _instance;

	public GameObject Panel;
	public float Size = 0.1f;
	public float MessageTime = 1;
	public int max = 8;

	bool _Abort = false;
	public bool Abort
	{
		get { return _Abort; }
		set { _Abort = value; }
	}

	bool _Skipping = false;
	public bool Skipping
	{
		get { return _Skipping; }
		set
		{
			//GameObject find1 = GameObject.Find("Button Saltar");
			/*if (find1 != null)
			{
				Transform find2 = find1.transform.Find("Label");
				if (find2 != null)
				{
					UILabel label = find2.GetComponent<UILabel>();
					if (label != null)
					{
						label.text = value ? "AVANCE\nNORMAL" : "AVANCE\nRÁPIDO";
					}
				}
			}*/
			_Skipping = value;
		}
	}

	bool mFinPartido = false;
	bool mSegundaParte = false;
#if USE_FAKE
	MatchManager mMatchManager;
#endif
	MatchBridge mMatchBridge;
	AudioInGameController mAudioGameController;
	public InteractiveType mInteractiveType = InteractiveType.None;
	CustomEnumerator<Momento> mMomentIT;

	void Awake()
	{
		//Instance = this;
	}

	public void Init(MatchBridge.Difficulty difficultyLevel)
	{
		mAudioGameController = GameObject.FindGameObjectWithTag("GameModel").GetComponent<AudioInGameController>();
		mFinPartido = false;
		mSegundaParte = false;
		Skipping = false;
		_Abort = false;
		_Skipping = false;

		var gameModel = GameObject.FindGameObjectWithTag("MatchManager");
#if USE_FAKE
		mMatchManager = gameModel.GetComponent<MatchManager>();
#endif
		mMatchBridge = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MatchBridge>();

		// Se determina el número de interacciones del jugador
		var interactivas = Random.Range(4, 6);
		// Se determina el número de interacciones exitosas requeridas.
		var exitos = Mathf.Max(1, (int)(interactivas * PercentagePass(difficultyLevel)));
		// Se calculan los goles contrarios totales = interacciones exitosas requeridas -1
		var golesContrarios = exitos - 1;
		// Se determinan los pasos de narración y sus minutos(min por determinar, máx 30, cada 3 - 5 minutos)
		mMoments.Clear();
		int minuto = 0;
		mMoments.Add(new Momento(minuto, InteractiveType.Init));
		do
		{
			minuto += Random.Range(3, 5);
			mMoments.Add(new Momento(minuto, InteractiveType.None));
			if (minuto >= 45 && !mSegundaParte)
			{
				mSegundaParte = true;
				minuto = 45;
				mMoments.Add(new Momento(minuto, InteractiveType.Middle));
			}
			else if (minuto > 90 && !mFinPartido)
			{
				mMoments.Add(new Momento(minuto, InteractiveType.End));
				break;
			}
		} while (true);
		//////////////////////////////////////////////////////
		var total = mMoments.Count - 3; // Quito el inicio, medio tiempo y final.
												  // Se determina en qué pasos marcarán los contrarios
		MakeMoments(golesContrarios, InteractiveType.Shot, false, false, true);
		// Se determina en qué pasos de la narración saltan las interacciones de usuario, sabiendo que tiene que haber al menos un paso de narración libre después de cada una de las interacciones.
		MakeMoments(interactivas, InteractiveType.All, true, true, false);
		// El resto se ponen como interacciones no efectivas.
		var resto = total - golesContrarios - interactivas;
		var tmp = resto / 2;
		MakeMoments(tmp, InteractiveType.All, false, false, false);
		resto -= tmp;
		MakeMoments(resto, InteractiveType.All, true, false, false);

		// Para cada interacción de usuario
		// Me quedan oportunidades (restantes >= exitosas requeridas) 
		// Decido si es oportunidad de gol.
		// SI no es oportunidad de gol, y resuelvo con éxito, reservo slot de tiempo posterior para marcar gol en narración
		// Si no me quedan oportunidades, no puede ser oportunidad de gol y no reservo slot para marcar gol en la narración aunque la haga con éxito.
		mSegundaParte = false;
		mMomentIT = new CustomEnumerator<Momento>(mMoments);
	}

	void MakeMoments(int moments, InteractiveType type, bool local, bool user, bool success)
	{
		List<Momento> freeMom = null;
		freeMom = mMoments.FindAll(e => e.mType == InteractiveType.None);
		bool oneShot = false;
		while (moments > 0)
		{
			int idx = Random.Range(0, freeMom.Count - 1);
			freeMom[idx].mLocal = local;
			freeMom[idx].mUser = user;
			freeMom[idx].mRandom = user ? 0.0f : Random.value;
			freeMom[idx].mRandom2 = Random.value;

			if (type != InteractiveType.All)
			{
				freeMom[idx].mType = type;
			}
			else
			{
				if (moments == 1 && !oneShot)
					freeMom[idx].mType = InteractiveType.Shot;
				else
				{
					freeMom[idx].mType = (InteractiveType)Random.Range((int)InteractiveType.Dribling, (int)InteractiveType.Shot + 1);
					if (freeMom[idx].mType == InteractiveType.Shot) oneShot = true;
				}
			}

			freeMom[idx].mExito = success;
			freeMom.RemoveAt(idx);
			moments--;
		}
	}

	Momento getFreeMoment()
	{
		List<Momento> free = mMoments.FindAll(e => e.mType == InteractiveType.None);
		return free[Random.Range(0, free.Count - 1)];
	}

	private int _previousIndex = -1;
	private bool _previousInteractive = false;
	System.Text.StringBuilder StringBuilder = new System.Text.StringBuilder();
	// Use this for initialization
	IEnumerator InsertPanel(Momento cur, bool reevaluate)
	{
		string tiempo = "";
		if (cur.mTiempo > 90 || (cur.mTiempo > 45 && !mSegundaParte))
		{
			if (cur.mTiempo > 90)
			{
				tiempo = string.Format("90+{0}", (cur.mTiempo - 90));
			}
			else
			{
				tiempo = string.Format("45+{0}", (cur.mTiempo - 45));
			}
		}
		else
		{
			tiempo = string.Format("{0}", cur.mTiempo);
		}

		NarratorMessageType type;
		if (cur.mType >= InteractiveType.Init)
		{
			type = NarratorMessageType.Descriptive;
		}
		else
		{
			type = cur.mLocal ? NarratorMessageType.LocalPlayerMessage : NarratorMessageType.VisitantPlayerMessage;
		}
		string action = "";
		string reaction = "";

		if (cur.mUser)
		{
			switch (cur.mType)
			{
				case InteractiveType.Pass:
					action = PassInteractive[0];
					break;
				case InteractiveType.Shot:
					action = ShotInteractive[0];
					break;
				case InteractiveType.Dribling:
					action = DribblingInteractive[0];
					break;
			}
			_previousInteractive = true;
		}
		else if (_previousInteractive)
		{
			_previousInteractive = false;
			switch (cur.mType)
			{
				case InteractiveType.Pass:
					reaction = PassInteractive[cur.mExito ? 1 : 2];
					break;
				case InteractiveType.Shot:
					reaction = ShotInteractive[cur.mExito ? 1 : 2];
					break;
				case InteractiveType.Dribling:
					reaction = DribblingInteractive[cur.mExito ? 1 : 2];
					break;
			}
		}
		else
		{
			switch (cur.mType)
			{
				case InteractiveType.Init:
					action = "Comienza el partido.";
					break;
				case InteractiveType.Middle:
					action = "Comienza la segunda parte.";
					break;
				case InteractiveType.End:
					action = "Finaliza el partido.";
					break;
				case InteractiveType.Pass:
					_previousIndex = new System.Random().Next(0, PassLogs.GetLength(0));
					action = PassLogs[_previousIndex,0];
					reaction = PassLogs[_previousIndex, cur.mExito ? 1 : 2];
					//if (cur.mRandom < 0.5f)
					//{ // Pase
					//	action = Response("Pase entre líneas del {0}.", "{0} intenta un pase.", cur.mRandom2);
					//	if (!cur.mExito)
					//	{
					//		reaction = Response("Pero falla.", "Pero no lo consigue.", "Y pierde el control.");
					//	}
					//	else
					//	{
					//		reaction = Response("Y continúa la jugada.", "Y se acerca a puerta.", "Y encara a la defensa.");
					//	}
					//}
					//else
					//{ // Saque
					//	action = Response("El {0} saca de banda.", "Saque de banda del {0}.", cur.mRandom2);
					//	if (!cur.mExito)
					//	{
					//		reaction = Response("Pero recupera el {1}.", "Pero el {1} corta el saque.");
					//	}
					//	else
					//	{
					//		reaction = Response("Y controla el balón. ", "Y monta el ataque.");
					//	}
					//}
					break;

				case InteractiveType.Shot:
					_previousIndex = new System.Random().Next(0, ShotLogs.GetLength(0));
					action = ShotLogs[_previousIndex, 0];
					reaction = ShotLogs[_previousIndex, cur.mExito ? 1 : 2];
					//if (cur.mRandom < 0.33f)
					//{// TIRO
					//	action = Response("{0} tira a puerta.", "Disparo a puerta del {0}.", "{0} prueba un tiro a puerta.", cur.mRandom2);
					//	if (!cur.mExito)
					//	{
					//		reaction = Response("Pero no lo consigue.", "Pero falla.", "Pero no tiene éxito.");
					//	}
					//	else
					//	{
					//		reaction = Response("Buen disparo… GOOOOL!", "GOOOOL!", "GOOOOL! El portero no pudo hacer nada.");
					//	}
					//}
					//else if (cur.mRandom < 0.66f)
					//{ // ASISTENCIAS
					//	action = Response("Pase entre líneas del {0}.", "Gran asistencia del {0}.", cur.mRandom2);
					//	if (!cur.mExito)
					//	{
					//		reaction = Response("Pero el {1} intercepta el pase.", "Pero el {1} logra cortar.");
					//	}
					//	else
					//	{
					//		reaction = Response("Que logra rematar y GOOOOL!", "Que controla, tira y GOOOOL!");
					//	}
					//}
					//else
					//{ // CÓRNER 
					//	action = "El {0} saca de esquina";
					//	if (!cur.mExito)
					//	{
					//		reaction = Response("Pero el {1} despeja el peligro.", "Pero el balón se va fuera.");
					//	}
					//	else
					//	{
					//		reaction = Response("GOOOL! Gran remate de cabeza.", "Controla dentro del área y GOOOOL!");
					//	}
					//}
					break;
				case InteractiveType.Dribling:
					_previousIndex = new System.Random().Next(0, DribblingLogs.GetLength(0));
					action = DribblingLogs[_previousIndex, 0];
					reaction = DribblingLogs[_previousIndex, cur.mExito ? 1 : 2];
					//action = Response("Intento de regate del {0}.", "El jugador del {0} intenta un regate.", cur.mRandom2);
					//if (!cur.mExito)
					//{
					//	reaction = Response("Y no lo consigue.", "Y pierde el balón.");
					//}
					//else
					//{
					//	reaction = Response("Gran regate, se queda solo.", "Lo consigue, sigue la jugada.");
					//}
					break;
			}
		}
		
		string a = cur.mLocal ? cur.mUser ? mMatchBridge.CurrentMatchDefinition.Nick : mMatchBridge.CurrentMatchDefinition.MyName : mMatchBridge.CurrentMatchDefinition.OpponentName;
		string b = cur.mLocal ? mMatchBridge.CurrentMatchDefinition.OpponentName : mMatchBridge.CurrentMatchDefinition.MyName;



		if (!reevaluate)
		{
			StringBuilder.Length = 0;
			StringBuilder.Append(action);
			NarratorUI.Instance.AddMessage(tiempo, string.Format(StringBuilder.ToString(), a, b), false, type, false);
			if (!Skipping || cur.mUser)
			{
				if (cur.mUser)
					mAudioGameController.PlayMatchStartSound();
				yield return StartCoroutine(WaitForSeconds(MessageTime));
			}
		}
		if (!string.IsNullOrEmpty(reaction) && (!cur.mUser || reevaluate))
		{
			StringBuilder.Length = 0;
			StringBuilder.Append(reaction);
			NarratorUI.Instance.AddMessage(null, string.Format(StringBuilder.ToString(), a, b), cur.mExito && cur.mType == InteractiveType.Shot, type, false);
			if (!cur.mUserCopy && cur.mExito && cur.mType == InteractiveType.Shot)
				GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>().Gol();

			if (!Skipping || reevaluate || (cur.mExito && cur.mType == InteractiveType.Shot))
				yield return StartCoroutine(WaitForSeconds(MessageTime));
		}
	}

	IEnumerator WaitForSeconds(float seconds)
	{
		float waitTime = Time.realtimeSinceStartup + seconds;
		return Helper.WaitCondition(() => { return Time.realtimeSinceStartup > waitTime || Abort; });
	}

	string Response(string r1, string r2, float random) { return random > 0.5f ? r1 : r2; }
	string Response(string r1, string r2) { return Response(r1, r2, Random.value); }
	string Response(string r1, string r2, string r3, float random) { return random < 0.33f ? r1 : random < 0.66f ? r2 : r3; }
	string Response(string r1, string r2, string r3) { return Response(r1, r2, r3, Random.value); }

	bool mReevaluate = false;

	public void Reevaluate(bool result)
	{
		var cur = mMomentIT.Current;
		cur.mUser = false;
		cur.mUserCopy = true;
		cur.mExito = result;
		mReevaluate = true;

		// Tenemos que ver si la jugada no era de gol pero era correcta, para dejar una marca para la siguiente jugada no interactiva convertirla en gol!
		if (result && (cur.mType == InteractiveType.Pass || cur.mType == InteractiveType.Dribling))
		{
			CustomEnumerator<Momento> copy = new CustomEnumerator<Momento>(mMomentIT);
			bool res = false;
			while (copy.MoveNext())
			{
				var copycur = copy.Current;
				if (copycur.mLocal && !copycur.mUser && (copycur.mType == InteractiveType.Pass || copycur.mType == InteractiveType.Dribling))
				{
					copycur.mType = InteractiveType.Shot;
					copycur.mExito = true;
					res = true;
					break;
				}
			}
			if (!res) // No se ha resulto la jugada!!!!
				InExtemisShot();
		}

	}

	void InExtemisShot()
	{
		var last = mMoments[mMoments.Count - 2];
		// Tengo que meter una inextremis.
		var newTime = last.mTiempo + 1;
		mMoments.Insert(mMoments.Count - 1, new Momento(newTime, InteractiveType.Shot) { mLocal = true, mExito = true });
		mMoments[mMoments.Count - 1].mTiempo = newTime;
	}

	public bool MatchFinished
	{
		get { return mMomentIT == null || mMomentIT.Finished(); }
	}

	public IEnumerator Narrador()
	{
		mInteractiveType = InteractiveType.End;
		do
		{
			if (Abort)
			{
				mInteractiveType = InteractiveType.End;
				break;
			}
			//            mInteractiveType = InteractiveType.Shot;
			//            break;

			var cur = mMomentIT.Current;
			if (cur.mType == InteractiveType.Middle) mSegundaParte = true;
			yield return StartCoroutine(InsertPanel(cur, mReevaluate));
			mReevaluate = false;
			if (cur.mLocal && cur.mUser)
			{
#if USE_FAKE
                mMatchManager.MatchResult.ScorePerInteractionSequence.Add(Random.value);
                Reevaluate(Random.value > 0.5f);
                yield return new WaitForSeconds(1.0f);
#else
				mInteractiveType = cur.mType;
				break;
#endif
			}
		} while (mReevaluate || mMomentIT.MoveNext());
	}
	
	private class CustomEnumerator<T>
		where T : class
	{
		private List<T> _List;
		private int _IndCurrent;

		public CustomEnumerator(List<T> list)
		{
			_List = list;
			_IndCurrent = 0;
		}

		public CustomEnumerator(CustomEnumerator<T> other)
		{
			_List = other._List;
			_IndCurrent = other._IndCurrent;
		}

		public T Current
		{
			get
			{
				if (_List != null && _IndCurrent < _List.Count)
					return _List[_IndCurrent];
				else
					return null;
			}
		}

		public bool MoveNext()
		{
			if (_List != null && _IndCurrent < _List.Count - 1)
			{
				++_IndCurrent;
				return true;
			}
			else
				return false;
		}

		public bool Finished()
		{
			return _IndCurrent >= _List.Count - 1;
		}
	}

	public float PercentagePass(MatchBridge.Difficulty difficultyLevel)
	{
		float result = 0;
		switch (difficultyLevel) {
			case MatchBridge.Difficulty.EASY:
				result = 0.33f;
				break;
			case MatchBridge.Difficulty.MEDIUM:
				result = 0.50f;
				break;
			case MatchBridge.Difficulty.HIGH:
				result = 1;
				break;
			case MatchBridge.Difficulty.EXTREME:
				result = 1;
				break;
		}
		return result;
	}
}
