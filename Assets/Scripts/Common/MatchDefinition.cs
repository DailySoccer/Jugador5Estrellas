using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FootballStar.Common
{
	// Definicion estatica de un partido, el partido de base de datos, digamos
	public class MatchDefinition
	{
		public float        Difficulty;
		public string	    PlaySequence;						// "Tier1Friendly01", "Tier3Cup01"...
		public int          Reward;
        public int          MyID;
		//TODO: Modificar esto. He hecho una func. que devuelve el nombre desde tierdefinition
        public string       MyName { get { return Name(MyID); } }
        public string       MyBadgeName { get { return BadgeName(MyID); } }
        public int          OpponentID;
        //TODO: Modificar esto. He hecho una func. que devuelve el nombre desde tierdefinition
        public string       OpponentName { get { return Name(OpponentID); } }
        public string       OpponentBadgeName { get { return BadgeName(OpponentID); } }        
        public string       Nick;

        public static string Name(int id) { return FootballStar.Manager.Model.TierDefinition.TeamNames[id]; }
        public static string BadgeName(int id){
                return Name(id).Replace(" ", "").Replace(".", "").Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                                                                     .Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "u");
        }

        public int RewardAsNotWinner {
			get { return Reward / 5; }
		}
	} 
	
	[JsonObject(MemberSerialization.OptOut)]
	public class MatchResult
	{
		public static float PERC_ONE_STAR = 0;
		public static float PERC_TWO_STARS = 0.5f;
		public static float PERC_THREE_STARS = 1;
		// Puntuacion obtenida en cada una de las interacciones del partido
		public Statistic ScorePerInteractionSequence = new Statistic();// <-- Promedio de las jugadas

		public class Statistic
		{
			int _count;
			float _average;

			public Statistic()
			{
				_count = 0;
				_average = 0;
			}

			public float Average
			{
				get
				{
					return _average / _count;
				}
			}

			public void Add(float newStatistic)
			{
				_average += newStatistic;
				++_count;
			}
		}
		
		// Goles
		public int PlayerGoals { get; set; }  // Goles a favor
		public int OppGoals { get; set; } // Goles en contra.
		public int Stars { get; set;} // Estrellas conseguidas

		public void CalculateStars() {
			if (Stars < NumPrecisionBallsEndOfMatch)
				Stars = NumPrecisionBallsEndOfMatch;
		}

		public bool PlayerWon  { get { return PlayerGoals > OppGoals; } }
		public bool PlayerTied { get { return PlayerGoals == OppGoals; } }
		public bool PlayerLost { get { return PlayerGoals < OppGoals; } }

		static public readonly int FANS_PER_PRECISION_BALL = 1500;



		// Numero de pelotas al final del partido => Haciendo redondeos al alza
		public int NumPrecisionBallsEndOfMatch
		{
			get {
				float avgScore = AvgScoreEndOfMatch;
				int numBalls = 0;
				// Si perdemos o empatamos, siempre 0 balones
				if (PlayerWon) {
					if (avgScore <= PERC_ONE_STAR) numBalls = 0;
					else if (avgScore <= PERC_TWO_STARS)	   numBalls = 1;
					else if (avgScore <= PERC_THREE_STARS) numBalls = 2;
					else 					   numBalls = 3;
				}
                return numBalls;
			}
		}
		public int FansToAdd { get { return PlayerWon ? NumPrecisionBallsEndOfMatch * FANS_PER_PRECISION_BALL : 0; } }

		// Score suponiendo que el partido ha acabado, normalizado entre 0 y 1.
		// 0 es el mejor posible (seria el delay acumulado respecto al Perfect)
		private float AvgScoreEndOfMatch
		{
			get 
			{
				if (ScorePerInteractionSequence != null)
					return ScorePerInteractionSequence.Average;
				else
					return 0.0f;
			}
		}

        // Auxiliar para calcular un score dado una sequencia de interacciones so far y cuántas interacciones habra al final
        static float CalcAvgScoreSoFar(List<float> scoreSequence, int totalInteractionsCount) {
            float sum = 0;
            foreach (var score in scoreSequence) {
                sum += score;
            }
            var ret = sum / (float)totalInteractionsCount;
            return ret;
		}

        // Numero de pelotas entre 0 y 3, sin hacer redondeos.
        public static float CalcNumPrecisionBallsSoFar(List<float> scoreSequence, int totalInteractionsCount) {
			return CalcNumPrecisionBalls(CalcAvgScoreSoFar(scoreSequence, totalInteractionsCount));
		}

		// Puntuacion entre 0 y 3 para una sola interaccion. El score que entra es entre -1 y 1, directamente del QTE
		public static float CalcNumPrecisionBalls(float score) {
            var ret = Mathf.Abs(score) * 4.0f;
            return ret > 3.0f ? 3.0f : ret;
		}
		
		public MatchResult()
		{
		}

		public static void SetDifficultyPercentages(MatchBridge.Difficulty difficultyLevel)
		{
			switch (difficultyLevel) {
				case MatchBridge.Difficulty.EASY:
					PERC_ONE_STAR = 0.33f;
					PERC_TWO_STARS = 0.5f;
					PERC_THREE_STARS = 0.67f;
					break;
				case MatchBridge.Difficulty.MEDIUM:
					PERC_ONE_STAR = 0;
					PERC_TWO_STARS = 0.5f;
					PERC_THREE_STARS = 0.8f;
					break;
				case MatchBridge.Difficulty.HIGH:
					PERC_ONE_STAR = 0.99f;
					PERC_TWO_STARS = 0.99f;
					PERC_THREE_STARS = 0.99f;
					break;
				case MatchBridge.Difficulty.EXTREME:
					PERC_ONE_STAR = 0.99f;
					PERC_TWO_STARS = 0.99f;
					PERC_THREE_STARS = 0.99f;
					break;
			}
		}
	}
}

