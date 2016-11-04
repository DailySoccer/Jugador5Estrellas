using System;
using System.Collections.Generic;
using FootballStar.Common;
using ExtensionMethods;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;

namespace FootballStar.Manager.Model
{

	public class TierDefinition
	{
        public enum Teams : int {
            Training,
			RayoVallecano,
			Getafe,
			Villarreal,
			Sporting, 		//5
			Alcorcon,
			Oviedo,
			Valladolid,
			Leganes,
			Tenerife, 		//10
            RealMadrid,
            AtleticoMadrid,
            Albacete,
            Malaga,
            LosAngeles, 	//15
            Miami,
            PhoenixDragons,
            WashingtonEagles,
            AthleticClub,
            Sevilla, 		//20
            Barcelona,
            Espanyol,
            Munchen,
            Betis,
            Valencia,		//25
            CeltadeVigo,
            Zaragoza,
            ManchesterRed, 
            Istambul,
            Dortmund,		//30
            AAAAMsterdam,
            //Mallorca

        };

		public static string GetTeamName (int id) {
			return TeamNames[id].Replace(" ", "").Replace(".", "").Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
																  .Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "u");
		}

        public static string[] TeamNames =
        {
			"Training",
			"Rayo Vallecano",
			"Getafe C.F.",
			"Villarreal C.F.",
			"R. Sporting de Gijón",		//5
			"A.D. Alcorcón",			
			"R. Oviedo",
			"R. Valladolid C.F.",
			"C.D. Leganés",
			"C.D. Tenerife",			//10
            "Real Madrid C.F.",
            "Atlético de Madrid",
			"Albacete Balompié",
            "Málaga C.F.",
            "Los Angeles Bears",		//15
            "Miami S.A.",
            "Phoenix Dragons",
            "Washington Eagles",
            "Athletic Club",
            "Sevilla F.C.",				//20
            "F.C. Barcelona",
            "R.C.D. Espanyol",
            "Munchen",
            "R. Betis Balompié",
            "Valencia C.F.",			//25
            "R.C. Celta de Vigo",
            "Real Zaragoza",
            "Manchester Red",
            "Istambul",
            "Dortmund",					//30
			"AAAAMsterdam",
            //"R.C.D. Mallorca",

        };

		public static string[] TeamShortNames =
		{
            "TRN",
			"RAY",
			"GET",
			"CVF",
			"GIJ",	//5
			"ADA",
			"ROV",
			"VAD",
			"LEG",
			"TEN",	//10
            "RMD",
            "ATM",
			"ALB",
			"MLG",
			"LAB",	//15
			"MSA",
			"PHD",
			"WAS",
			"ATH",
			"SEV",	//20
			"FCB",
			"ESP",
			"MUN",
			"BET",
			"VAL",	//25
			"VIG",
			"ZAR",
			"RED",
			"IST",
			"DOR",	//30
			"XXX",
			//"MAL"			
		};
            

        public static Teams[][] Segundas = new Teams[][] {
			new Teams[]{ Teams.Training },
			new Teams[]{ Teams.RayoVallecano, Teams.Albacete, Teams.RealMadrid, Teams.Zaragoza, Teams.Sevilla, Teams.Valencia, Teams.Leganes, Teams.Tenerife, Teams.Malaga},
			new Teams[]{ Teams.Getafe, Teams.Barcelona, Teams.LosAngeles, Teams.PhoenixDragons, Teams.Oviedo},
			new Teams[]{ Teams.Villarreal, Teams.Alcorcon, Teams.Dortmund},
			new Teams[]{ Teams.Sporting, Teams.AthleticClub, Teams.AtleticoMadrid, Teams.Malaga},		//5
			new Teams[]{ Teams.Alcorcon, Teams.Villarreal, Teams.Dortmund},
			new Teams[]{ Teams.Oviedo, Teams.Getafe, Teams.Barcelona, Teams.LosAngeles, Teams.PhoenixDragons},
			new Teams[]{ Teams.Valladolid, Teams.Espanyol, Teams.Malaga},
			new Teams[]{ Teams.Leganes, Teams.RayoVallecano, Teams.Albacete, Teams.RealMadrid, Teams.Zaragoza, Teams.Sevilla, Teams.Valencia, Teams.Tenerife, Teams.Malaga},
			new Teams[]{ Teams.Tenerife, Teams.RayoVallecano, Teams.Albacete, Teams.RealMadrid, Teams.Zaragoza, Teams.Sevilla, Teams.Valencia, Teams.Leganes, Teams.Malaga},		//10
			new Teams[]{ Teams.RealMadrid, Teams.RayoVallecano, Teams.Albacete, Teams.Zaragoza, Teams.Sevilla, Teams.Valencia, Teams.Leganes, Teams.Tenerife, Teams.Malaga},
			new Teams[]{ Teams.AtleticoMadrid, Teams.AthleticClub, Teams.Sporting, Teams.Malaga},
			new Teams[]{ Teams.Albacete},
			new Teams[]{ Teams.Malaga, Teams.Valladolid, Teams.Espanyol}, // Azules
			new Teams[]{ Teams.LosAngeles, Teams.Getafe, Teams.Barcelona, Teams.PhoenixDragons, Teams.Oviedo},		//15
			new Teams[]{ Teams.Miami},
			new Teams[]{ Teams.PhoenixDragons, Teams.Getafe, Teams.Barcelona, Teams.LosAngeles, Teams.Oviedo},
			new Teams[]{ Teams.WashingtonEagles,},
			new Teams[]{ Teams.AthleticClub, Teams.AtleticoMadrid, Teams.Sporting, Teams.Malaga}, // Rojiblancos
			new Teams[]{ Teams.Sevilla, Teams.RayoVallecano, Teams.Albacete, Teams.RealMadrid, Teams.Zaragoza, Teams.Valencia, Teams.Leganes, Teams.Tenerife, Teams.Malaga}, // Blancos	//20
			new Teams[]{ Teams.Barcelona,Teams.Getafe, Teams.LosAngeles, Teams.PhoenixDragons, Teams.Oviedo},
			new Teams[]{ Teams.Espanyol, Teams.Valladolid, Teams.Malaga},
			new Teams[]{ Teams.Munchen, Teams.AtleticoMadrid, Teams.Sporting, Teams.AthleticClub},
			new Teams[]{ Teams.Betis},
			new Teams[]{ Teams.Valencia,Teams.RayoVallecano, Teams.Albacete, Teams.RealMadrid, Teams.Zaragoza, Teams.Sevilla, Teams.Leganes, Teams.Tenerife, Teams.Malaga},		//25
			new Teams[]{ Teams.CeltadeVigo, Teams.Oviedo },
			new Teams[]{ Teams.Zaragoza, Teams.RayoVallecano, Teams.Albacete, Teams.RealMadrid, Teams.Sevilla, Teams.Valencia, Teams.Leganes, Teams.Tenerife, Teams.Malaga},
			new Teams[]{ Teams.ManchesterRed, Teams.AtleticoMadrid, Teams.Sporting, },
			new Teams[]{ Teams.Istambul, Teams.Alcorcon, Teams.Villarreal},
			new Teams[]{ Teams.Dortmund, Teams.Villarreal, Teams.Alcorcon},		//30
			new Teams[]{ Teams.AAAAMsterdam,},
		};

		public static bool UseSecondEquipation(int local, int visitor) {
			foreach (var eq in Segundas[local])
				if (eq == (Teams)visitor)
					return true;
			return false;
		}
		/*
		public static bool UseSecondEquipation(int local, int visitor) {
			foreach (var eq in Segundas[visitor])
				if (eq == (Teams)local)
					return true;
			return false;
		}
		*/
		public string Name;				// Segunda B
		public int    OwnTeamID;		// Real Madrid B
		public string OwnTeamName { get { return TierDefinition.TeamNames[OwnTeamID]; } }

        public MatchDefinition[] FriendlyMatchesDefs;
		public MatchDefinition[] LeagueMatchesDefs;
		public MatchDefinition[] CupMatchesDefs;
		public MatchDefinition[] EuroMatchesDefs;
		
		public MatchDefinition GetMatchByKindAndID(Match.eMatchKind kind, int matchID)
		{
			if (kind == Match.eMatchKind.FRIENDLY)
				return FriendlyMatchesDefs[matchID];
			else if (kind == Match.eMatchKind.LEAGUE)
				return LeagueMatchesDefs[matchID];
			else if (kind == Match.eMatchKind.CUP)
				return CupMatchesDefs[matchID];
						
			return EuroMatchesDefs[matchID];
				return CupMatchesDefs[matchID];
						
			return EuroMatchesDefs[matchID];
		}
		public static int ownID = 0;

        public static TierDefinition[] AllTiers = new TierDefinition[]
		{
			#region TIERS
			new TierDefinition()
			{
				Name = "Tier 1",
				OwnTeamID = ownID,
				
				FriendlyMatchesDefs = new MatchDefinition[]
				{
					new MatchDefinition()
					{
						OpponentID = (int)Teams.AAAAMsterdam,
						PlaySequence = null,					// Nombre autogenerado: TierXFriendlyXX
						Reward = 3000,
						Difficulty = 0.5f,
					},
				},
				LeagueMatchesDefs = new MatchDefinition[]
				{
					new MatchDefinition()
					{
						OpponentID = (int)Teams.LosAngeles,
						PlaySequence = null,					// Nombre autogenerado: TierXLeagueXX
						Reward = 3000,
						Difficulty = 0.5f,
					},
				},
				CupMatchesDefs = new MatchDefinition[] {},		// Locked
				EuroMatchesDefs = new MatchDefinition[]	{}		// Locked
			},
			
			//
			// TIER 2
			//
			new TierDefinition()
			{
				Name = "Tier 2",
				OwnTeamID = ownID,
				FriendlyMatchesDefs = new MatchDefinition[]
				{
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.AAAAMsterdam,
						PlaySequence = null,					// Nombre autogenerado: TierXFriendlyXX
						Reward = 3000,
						Difficulty = 0.5f,
					}
				},
				LeagueMatchesDefs = new MatchDefinition[]
				{
					new MatchDefinition()
					{
						OpponentID = (int)Teams.LosAngeles,
						PlaySequence = null,					// Nombre autogenerado: TierXLeagueXX
						Reward = 3000,
						Difficulty = 0.5f,
					},
				},
				CupMatchesDefs = new MatchDefinition[]
				{
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.AAAAMsterdam,
						PlaySequence = null,
						Reward = 3000,
						Difficulty = 0.5f,
					},
				},
				EuroMatchesDefs = new MatchDefinition[] {}
			},
			#endregion
			//
			// TIER 3
			//
			new TierDefinition()
			{
				Name = "RMD",
				OwnTeamID = ownID,
				#region Amistosos               
				// FRIENDLY
				FriendlyMatchesDefs = new MatchDefinition[]
				{
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Malaga,
						PlaySequence = null,					// Nombre autogenerado: TierXFriendlyXX
						Reward = 500, 
						Difficulty = 0.0f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.LosAngeles,
						PlaySequence = null,
						Reward = 500,
						Difficulty = 0.0f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Miami,
						PlaySequence = null,
						Reward = 500,
						Difficulty = 0.03f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.PhoenixDragons,
						PlaySequence = null,
						Reward = 500,
						Difficulty = 0.03f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.WashingtonEagles,
						PlaySequence = null,
						Reward = 500,
						Difficulty = 0.08f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.AthleticClub,
						PlaySequence = null,
						Reward = 500,
						Difficulty = 0.08f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Sevilla,
						PlaySequence = null,
						Reward = 500,
						Difficulty = 0.10f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Barcelona,
						PlaySequence = null,
						Reward = 500,
						Difficulty = 0.13f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Espanyol,
						PlaySequence = null,
						Reward = 500,
						Difficulty = 0.18f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Munchen,
						PlaySequence = null,
						Reward = 500,
						Difficulty = 0.18f,
					},					
				},
				#endregion
				#region Liga               
				// LEAGUE
				LeagueMatchesDefs = new MatchDefinition[]
				{
					new MatchDefinition()
					{
						OpponentID = (int)Teams.AthleticClub,
						PlaySequence = null,
						Reward = 570,
						Difficulty = 0.08f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.AtleticoMadrid,
						PlaySequence = null,
						Reward = 660,
						Difficulty = 0.13f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.RayoVallecano,
						PlaySequence = null,
						Reward = 750,
						Difficulty = 0.18f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Getafe,
						PlaySequence = null,
						Reward = 840,
						Difficulty = 0.23f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Villarreal,
						PlaySequence = null,
						Reward = 930,
						Difficulty = 0.28f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Valencia,
						PlaySequence = null,
						Reward = 1020,
						Difficulty = 0.30f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.RealMadrid,
						PlaySequence = null,
                        Reward = 1380,
                        Difficulty = 0.46f,
                    },
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Sporting,
						PlaySequence = null,
						Reward = 1200,
						Difficulty = 0.38f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Sevilla,
						PlaySequence = null,
						Reward = 1290,
						Difficulty = 0.40f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Barcelona,
						PlaySequence = null,
						Reward = 1380,
						Difficulty = 0.46f,
					},
					
					new MatchDefinition()
					{
						OpponentID =(int)Teams.AthleticClub,
						PlaySequence = null,
						Reward = 2470,
						Difficulty = 0.50f,
					},
					new MatchDefinition()
					{
						OpponentID =(int)Teams.AtleticoMadrid,
						PlaySequence = null,
						Reward = 2810,
						Difficulty = 0.53f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.RayoVallecano,
						PlaySequence = null,
						Reward = 3150,
						Difficulty = 0.53f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Getafe,
						PlaySequence = null,
						Reward = 3490,
						Difficulty = 0.56f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Villarreal,
						PlaySequence = null,
						Reward = 3830,
						Difficulty = 0.70f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Valencia,
						PlaySequence = null,
						Reward = 4170,
						Difficulty = 0.75f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.RealMadrid,
						PlaySequence = null,
						Reward = 4510,
						Difficulty = 0.78f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Sporting,
						PlaySequence = null,
						Reward = 4850,
						Difficulty = 0.80f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Sevilla,
						PlaySequence = null,
						Reward = 5190,
						Difficulty = 0.85f,
					},
					new MatchDefinition()
					{
						OpponentID = (int)Teams.Barcelona,
						PlaySequence = null,
						Reward = 10000,
						Difficulty = 0.90f,
					},				
				},
				#endregion
				#region Copa               
				// CUP				
				CupMatchesDefs = new MatchDefinition[]
				{
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Zaragoza,
						PlaySequence = null,
						Reward = 650,
						Difficulty = 0.30f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Zaragoza,
						PlaySequence = null,
						Reward = 1875,
						Difficulty = 0.40f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Sevilla,
						PlaySequence = null,
						Reward = 2375,
						Difficulty = 0.45f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Sevilla,
						PlaySequence = null,
						Reward = 4125,
						Difficulty = 0.54f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.CeltadeVigo,
						PlaySequence = null,
						Reward = 4625,
						Difficulty = 0.58f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.CeltadeVigo,
						PlaySequence = null,
						Reward = 5000,
						Difficulty = 0.85f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Barcelona,
						PlaySequence = null,
						Reward = 8000,
						Difficulty = 1.05f,
					},
				},
				#endregion
				#region Europe
				// Europe
				EuroMatchesDefs = new MatchDefinition[]
				{
					
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.ManchesterRed,
						PlaySequence = null,
						Reward = 1875,
						Difficulty = 0.45f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.ManchesterRed,
						PlaySequence = null,
						Reward = 2375,
						Difficulty = 0.78f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Istambul,
						PlaySequence = null,
						Reward = 4625,
						Difficulty = 0.80f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Istambul,
						PlaySequence = null,
						Reward = 5000,
						Difficulty = 0.85f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Dortmund,
						PlaySequence = null,
						Reward = 5250,
						Difficulty = 0.90f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Dortmund,
						PlaySequence = null,
						Reward = 5250,
						Difficulty = 0.95f,
					},
					new MatchDefinition() 
					{
						OpponentID = (int)Teams.Munchen,
						PlaySequence = null,
						Reward = 50000,
						Difficulty = 1.11f,
					},
				},
				#endregion
			}
		};
	}
	
	[JsonObject(MemberSerialization.OptOut)]
	public class Tier
	{	
		// Partido de liga a partir del cual desbloqueamos Europa en el ultimo Tier
		public static readonly int EURO_MATCH_UNLOCK_LEAGUE_IDX = 9;
		public TierDefinition Definition { get { return TierDefinition.AllTiers[mTierID]; } }
		
		public List<Match> FriendlyMatches { get { return mFriendlyMatches;} }
		public List<Match> LeagueMatches { get { return mLeagueMatches;} }
		public List<Match> CupMatches { get { return mCupMatches; } }
		public List<Match> EuroMatches { get { return mEuroMatches; } }
		
		public League League { get { return mLeague; } }
		public SponsorsPerTier Sponsors { get { return mSponsors; } }
		
		public MatchBrowser MatchBrowser { get { return mMatchBrowser; } }	// View-Model

		public bool AreCupMatchesAvailable {
			get { return CupMatches.Count > 0; }
		}

		public bool AreEuroMatchesAvailable {
			get {
				bool bRet = false;

				if (EuroMatches.Count > 0 && MatchBrowser.LastLeagueIdx >= EURO_MATCH_UNLOCK_LEAGUE_IDX)
					bRet = true;

				return bRet;
			}
		}
		
		public Tier()
		{
		}
		
		public Tier(Player player, int tierID)
		{
			mTierID = tierID;
			
			mLeague = new League(mTierID);
			mSponsors = new SponsorsPerTier(player, mTierID);			
			mMatchBrowser = new MatchBrowser(player, this);
            TierDefinition.ownID = player.SelectedTeamId;
            PrecreateMatches();
		}
		
		private void PrecreateMatches()
		{
			// Precreamos los partidos, uno por definicion
			Definition.FriendlyMatchesDefs.ForEachWithIndex((matchDef, matchIndex) =>
			{
				mFriendlyMatches.Add(new Match(mTierID, Match.eMatchKind.FRIENDLY, matchIndex));
			});
			
			Definition.LeagueMatchesDefs.ForEachWithIndex((matchDef, matchIndex) =>
			{
				mLeagueMatches.Add(new Match(mTierID, Match.eMatchKind.LEAGUE, matchIndex));
			});
			
			Definition.CupMatchesDefs.ForEachWithIndex((matchDef, matchIndex) =>
			{
				mCupMatches.Add(new Match(mTierID, Match.eMatchKind.CUP, matchIndex));
			});
			
			Definition.EuroMatchesDefs.ForEachWithIndex((matchDef, matchIndex) =>
			{
				mEuroMatches.Add(new Match(mTierID, Match.eMatchKind.EURO, matchIndex));
			});
		}
		
		private int mTierID;
	
		private League mLeague;
		private SponsorsPerTier mSponsors;		
		private MatchBrowser mMatchBrowser;
	
		private List<Match> mFriendlyMatches = new List<Match>();
		private List<Match> mLeagueMatches = new List<Match>();
		private List<Match> mCupMatches = new List<Match>();
		private List<Match> mEuroMatches = new List<Match>();
	}
}

