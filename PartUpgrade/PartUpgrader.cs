/*
Copyright 2015 Merill (merill@free.fr)

This file is part of PartUpgrader.
PartUpgrader is free software: you can redistribute it and/or modify it 
under the terms of the GNU General Public License as published by 
the Free Software Foundation, either version 3 of the License, 
or (at your option) any later version.

PartUpgrader is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty 
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License 
along with PartUpgrader. If not, see http://www.gnu.org/licenses/.

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Strategies;
using Strategies.Effects;

namespace SpaceRace
{
	[KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
	public class PartUpgrader : MonoBehaviour
	{

		private List<string> allTechResearched;

		public void Start()
		{
			allTechResearched = new List<string>();
			GameEvents.OnTechnologyResearched.Add(researchDone);
			GameEvents.onGameStateLoad.Add(loadSave);
			getAllTechnologies();
			reloadAndUpgrade();
		}

		private void getAllTechnologies()
		{
			//maj currenttech
			allTechResearched.Clear();
			if (HighLogic.CurrentGame.Mode != Game.Modes.SANDBOX)
			{
				ProtoScenarioModule protoScenario = HighLogic.CurrentGame.scenarios.Find(x => x.moduleName == "ResearchAndDevelopment");
				foreach (ConfigNode tech in protoScenario.GetData().GetNodes("Tech"))
				{
					string node = tech.GetValue("id");
					allTechResearched.Add(node);
				}
			}
		}

		public void loadSave(ConfigNode loadedNode)
		{
			if (needReload())
			{
				getAllTechnologies();
				reloadAndUpgrade();
			}
		}

		public bool needReload()
		{
			if (HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX)
			{
				//don't need to reload a sandbox!
				return false;
			}

			List<string> researchedNow = new List<string>();
			researchedNow.AddRange(allTechResearched);
			ProtoScenarioModule protoScenario = HighLogic.CurrentGame.scenarios.Find(x => x.moduleName == "ResearchAndDevelopment");

			foreach (ConfigNode tech in protoScenario.GetData().GetNodes("Tech"))
			{
				string node = tech.GetValue("id");

				if (allTechResearched.Contains(node))
				{
					researchedNow.Remove(node);
				}
				else
				{
					//a new researched note!
					return true;
				}
			}

			//test if we load a revious save without an actuel tech
			if (researchedNow.Count == 0) return false;
			else return true;
		}

		public void researchDone(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> research)
		{
			//obligé que l'on a changé de tech
			if (research.target == RDTech.OperationResult.Successful)
			{
				//allTechResearched.Add(research.host.title);
				allTechResearched.Add(research.host.techID);
				reloadAndUpgrade();
			}
		}

		public void reloadAndUpgrade()
		{
			try
			{

				//Debug.log("[EU] reloadAndUpgrade"); foreach (string techName in allTechResearched){Debug.Log("[EU] HAS tech " + techName);}

				//maj part
				foreach (AvailablePart ap in PartLoader.LoadedPartsList)
				{
					bool firstUgrade = true;
					List<ModuleUpgrade> allUpgrader = new List<ModuleUpgrade>();
					foreach (PartModule pm in ap.partPrefab.Modules)
					{
						////Debug.log("[EU] get all mu : " + pm.moduleName);
						if (pm != null && pm is ModuleUpgrade)
						{
							allUpgrader.Add((ModuleUpgrade)pm);
						}
					}
					foreach (ModuleUpgrade pm in allUpgrader)
					{
						// reset before maj
						if (firstUgrade)
						{
							//Debug.log("[EU] Restore all parts for part " + ap.name);
							firstUgrade = false;
							//reset
							//save cost in confignode
							if (ap.partConfig.GetValue("cost") != null)
							{
								ap.cost = float.Parse(ap.partConfig.GetValue("cost"));
							}
							else
							{
								//Debug.log("[EU] reload cost null > save current cost");
								ap.partConfig.AddValue("cost", ap.cost);
								ap.internalConfig.AddValue("cost", ap.cost);
							}
							//same for title
							if (ap.partConfig.GetValue("title") != null)
							{
								ap.title = ap.partConfig.GetValue("title");
							}
							else
							{
								//Debug.log("[EU] reload title null > save current title");
								ap.partConfig.AddValue("title", ap.title);
								ap.internalConfig.AddValue("title", ap.title);
							}

							//Debug.log("[EU] ap.partConfig=" + ap.partConfig);

							// reload (in reverse order, of course)
							foreach (ModuleUpgrade pmReset in allUpgrader.Reverse<ModuleUpgrade>())
							{
								//Debug.log("[EU] reload " + pmReset.moduleName);
								try
								{
									//try to restore from last tech or from current techs?
									//currently, from last tech.
									pmReset.Restore(ap.partConfig);
								}
								catch (Exception e)
								{
									Debug.LogError("Error when Restore " + pmReset.moduleName + ", module n° "
										+ ap.partPrefab.Modules.IndexOf(pmReset) + " in part " + ap.name + ", " + ap.title + ": " + e);
								}
							}
						}

						//Debug.log("[EU] Upgrade PartModule " + pm.moduleName + " for part " + ap.name);
						try
						{
							pm.Upgrade(allTechResearched);
						}
						catch (Exception e)
						{
							Debug.LogError("Error when upgrading " + pm.moduleName + ", module n° "
								+ ap.partPrefab.Modules.IndexOf(pm) + " in part " + ap.name + ", " + ap.title + ": " + e);
						}

					}

				}
			}
			catch (Exception e)
			{
				print("Exception e " + e);
			}
		}
	}
}
