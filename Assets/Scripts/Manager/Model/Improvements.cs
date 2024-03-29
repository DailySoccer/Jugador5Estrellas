using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FootballStar.Manager.Model
{
	public class ImprovementsDefinition
	{
		static public void LoadBackgroundsForCategory(ImprovementCategory category)
		{
			if (category.Background != null)
			{
				var theBackground = Resources.Load(category.Background, typeof(Texture2D)) as Texture2D;
				var theBackgroundWhenOwned = Resources.Load(category.Background + category.OwnedSuffix, typeof(Texture2D)) as Texture2D;
				
				for (int c = 0; c < category.Items.Length; c++)
				{
					category.Items[c].Background 			= theBackground;
					category.Items[c].BackgroundWhenOwned 	= theBackgroundWhenOwned;
				}
			}
			else
			{
				for (int c = 0; c < category.Items.Length; c++)
				{
					var textureName = category.BackgroundPrefix + (c + 1).ToString("D2");
					category.Items[c].Background = Resources.Load(textureName, typeof(Texture2D)) as Texture2D;
					if (category.Items[c].Background == null)
						Debug.Log("Improvement Background Didn't Load... " + textureName);

					//Algunos fondos cambian si los poseemos
					var textureWhenOwnedName = category.BackgroundPrefix + (c + 1).ToString("D2") + category.OwnedSuffix;
					if (category.OwnedSuffix != "") {
						category.Items[c].BackgroundWhenOwned = Resources.Load(textureWhenOwnedName, typeof(Texture2D)) as Texture2D;
						if (category.Items[c].BackgroundWhenOwned == null)
							Debug.Log("Improvement Background when owned Didn't Load... " + textureWhenOwnedName);
					}
				}
			}
		}
		
		// Este approach es experimental y en contraste con el de Match/Sponsors
		static public ImprovementItem[] AllImprovementItems
		{
			get 
			{
				if (mAllImprovementItems != null)
					return mAllImprovementItems;
				
				var helper = new List<ImprovementItem>();
				
				helper.AddRange(GymCategory.Items);
				helper.AddRange(BlackboardCategory.Items);
				helper.AddRange(TechniqueCategory.Items);
				helper.AddRange(LockerRoomCategory.Items);
				helper.AddRange(EventsCategory.Items);
				helper.AddRange(PropertiesCategory.Items);
				
				mAllImprovementItems = helper.ToArray();
				
				return mAllImprovementItems;
			}
		}
		
		static public ImprovementItem GetImprovementItemByID(int id)
		{
			var items = AllImprovementItems;
			
			foreach (var item in items)
			{
				if (item.ImprovementItemID == id)
					return item;
			}
			return null;
		}
		
		static private ImprovementItem[] mAllImprovementItems;
				
		
		static public ImprovementCategory GymCategory = new ImprovementCategory()
		{
			Description = "MEJORANDO EL GIMNASIO ENTRENARÁS MEJOR TU FÍSICO.",
			BackgroundPrefix = "TrainingGymBackground",
			Items = new ImprovementItem[]
			{ 
			  new ImprovementItem() { Name = "Entrenamiento de velocidad", Price =  1606, VisionDiff = 0f, PowerDiff = 0.059f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 0 },
			  new ImprovementItem() { Name = "Entrenamiento de potencia", Price =  2759, VisionDiff = 0f, PowerDiff = 0.029f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 1 },
			  new ImprovementItem() { Name = "Entrenamiento de resistencia", Price =  3995, VisionDiff = 0f, PowerDiff = 0.029f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 2 },
			  new ImprovementItem() { Name = "Entrenamiento TRX", Price =  8154, VisionDiff = 0f, PowerDiff = 0.022f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 3 },
			  new ImprovementItem() { Name = "Entrenamiento de alto rendimiento", Price = 11253, VisionDiff = 0f, PowerDiff = 0.029f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 4 },
			}
		};
		
		static public ImprovementCategory BlackboardCategory = new ImprovementCategory()
		{
			Description = "APRENDE NUEVAS TÁCTICAS Y MEJORA TU VISIÓN DE JUEGO",
			Background = "TrainingBlackboardBackground",
			Items = new ImprovementItem[]
			{ 
				new ImprovementItem() { Name = "Catenacio", 		Price =  1634, VisionDiff = 0.022f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 10 },
				new ImprovementItem() { Name = "El Fuera de juego", Price =  2815, VisionDiff = 0.037f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 11 },
				new ImprovementItem() { Name = "Ataque total",     	Price =  4080, VisionDiff = 0.051f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 12 },
				new ImprovementItem() { Name = "El contragolpe",    Price =  8338, VisionDiff = 0.066f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 13 },
				new ImprovementItem() { Name = "Tiki-Taka",         Price = 11508, VisionDiff = 0.074f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 14 },
			}
		};
		
		static public ImprovementCategory TechniqueCategory = new ImprovementCategory()
		{
			Description = "APRENDE LOS TRUCOS DE LOS MEJORES Y MEJORA TU TÉCNICA",
			BackgroundPrefix = "TrainingTechniqueBackground",
			Items = new ImprovementItem[]
			{ 
				new ImprovementItem() { Name = "Pack de técnicas básicas",      Price =  1770, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.059f, MotivationDiff = 0f, ImprovementItemID = 20 },
				new ImprovementItem() { Name = "Pack de técnicas avanzadas", 	Price =  3094, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.044f, MotivationDiff = 0f, ImprovementItemID = 21 },
				new ImprovementItem() { Name = "Pack de técnicas semi-pro",     Price =  4511, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.051f, MotivationDiff = 0f, ImprovementItemID = 22 },
				new ImprovementItem() { Name = "Pack de técnicas profesional",  Price =  9254, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.015f, MotivationDiff = 0f, ImprovementItemID = 23 },
				new ImprovementItem() { Name = "Pack de técnicas Crack Skill",  Price = 12788, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.022f, MotivationDiff = 0f, ImprovementItemID = 24 },
			}
		};
		
		static public ImprovementCategory LockerRoomCategory = new ImprovementCategory()
		{
			Description = "MEJORAR TU EQUIPACIÓN Y AUMENTARÁ TU RENDIMIENTO GLOBAL",
			BackgroundPrefix = "TrainingLockerRoomBackground",
			Items = new ImprovementItem[]
			{ 
				new ImprovementItem() { Name = "Botas Generation 3",Price =  1198,  VisionDiff = 0f, 	PowerDiff = 0.029f, TechniqueDiff= 0f,		MotivationDiff =     0f,ImprovementItemID = 30 },
				new ImprovementItem() { Name = "Botas Pro",     	Price =  1928,  VisionDiff = 0f,    PowerDiff =     0f, TechniqueDiff= 0.022f,	MotivationDiff =     0f,ImprovementItemID = 31 },
				new ImprovementItem() { Name = "Botas Next Gen",    Price =  3057,  VisionDiff = 0f, 	PowerDiff = 0.015f, TechniqueDiff= 0.007f, 	MotivationDiff =     0f,ImprovementItemID = 32 },
				new ImprovementItem() { Name = "Botas Hi-Tech",     Price =  6533,  VisionDiff = 0f, 	PowerDiff = 0.015f, TechniqueDiff= 0.015f, 	MotivationDiff = 0.015f,ImprovementItemID = 33 },
				new ImprovementItem() { Name = "Botas Evolution",   Price = 22788,  VisionDiff = 0f, 	PowerDiff = 0.022f, TechniqueDiff= 0.015f, 	MotivationDiff = 0.015f,ImprovementItemID = 34 },
			}
		};
		
		static public ImprovementCategory EventsCategory = new ImprovementCategory()
		{
			Description = "MEJORA TU MOTIVACIÓN Y AUMENTARÁ TU RENDIMIENTO GLOBAL.",
			BackgroundPrefix = "LifeEventsBackground",
			Items = new ImprovementItem[]
			{ 
				new ImprovementItem() { Name = "Firma de autógrafos",               Price =  759, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 40 },
			 	new ImprovementItem() { Name = "Rueda de prensa",                   Price = 1031, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 41 },
				new ImprovementItem() { Name = "Partido benéfico",                  Price = 1841, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.022f, ImprovementItemID = 42 },
				new ImprovementItem() { Name = "Entrenamiento a puertas abiertas",  Price = 4692, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.029f, ImprovementItemID = 43 },
				new ImprovementItem() { Name = "Entrega de premios",       			Price = 8012, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.022f, ImprovementItemID = 44 },
			}
		};
		
		static public ImprovementCategory PropertiesCategory = new ImprovementCategory()
		{
			Description = "MEJORA TU MOTIVACIÓN Y AUMENTARÁ TU RENDIMIENTO GLOBAL.",
			BackgroundPrefix = "LifePropertiesBackground",
			OwnedSuffix = "Sold",
			Items = new ImprovementItem[]
			{ 
				new ImprovementItem() { Name = "Ático",            Price =  1497, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 50 },
				new ImprovementItem() { Name = "Residencia exclusiva",          Price =  2535, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 51 },
				new ImprovementItem() { Name = "Casa de campo",                 Price =  3650, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.022f, ImprovementItemID = 52 },
				new ImprovementItem() { Name = "Villa en la playa",             Price =  7422, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 53 },
				new ImprovementItem() { Name = "Chalet en estación de esquí",   Price = 10230, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 54 },
			}
		};
		
		static public ImprovementCategory VehiclesCategory = new ImprovementCategory()
		{
			Description = "MEJORA TU MOTIVACIÓN Y AUMENTARÁ TU RENDIMIENTO GLOBAL.",
			BackgroundPrefix = "LifeVehiclesBackground",
			Items = new ImprovementItem[]
			{ 
				//Precio nuevo de los coches:
				new ImprovementItem() { Name = "Utilitario",    Price =  351, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 60 },
				new ImprovementItem() { Name = "Motocicleta",   Price =  557, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 61 },
				new ImprovementItem() { Name = "SUV",           Price =  940, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 62 },
				new ImprovementItem() { Name = "Deportivo",     Price = 3177, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 63 },
				new ImprovementItem() { Name = "Clásico",       Price = 8905, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 64 },

			}
		};
	} 
	 
	public class ImprovementCategory
	{
		public string Description { get; set; }
		public string Background { get; set; }
		public string BackgroundPrefix { get; set; }
		public string OwnedSuffix { get; set; }
		public ImprovementItem[] Items { get; set; }
	}

	public class ImprovementItem
	{
		public int ImprovementItemID;
		public string Name;
		public int Price;
		public float VisionDiff;
		public float PowerDiff;
		public float TechniqueDiff;
		public float MotivationDiff;
		public Texture2D Background;
		public Texture2D BackgroundWhenOwned;
	}
	
	[JsonObject(MemberSerialization.OptOut)]
	public class Improvements
	{
		public List<int> PurchasedImprovementItemIDs  = new List<int>();
		
		public Improvements()
		{
		}
							
		public void BuyImprovement(ImprovementItem theItem)
		{
			if (IsItemAlreadyPurchased(theItem))
				throw new Exception("WTF 3033 - Item already bought");
						
			PurchasedImprovementItemIDs.Add(theItem.ImprovementItemID);
		}
		
		public bool IsItemAlreadyPurchased(ImprovementItem theItem)
		{
			return PurchasedImprovementItemIDs.Contains(theItem.ImprovementItemID);
		}
	}
}

