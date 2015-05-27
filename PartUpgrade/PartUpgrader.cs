/*
PartUpgrader
Copyright (c) Merill, All rights reserved.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library.
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

		//private List<AvailablePart> partsToFetch;

		public void Start()
		{
			Debug.Log("[EU] START");
			allTechResearched = new List<string>();
			GameEvents.OnTechnologyResearched.Add(researchDone);
			GameEvents.onGameStateLoad.Add(loadSave);
			getAllTechnologies();
			reloadAndUpgrade();
			//GameEvents.onGameStateLoad;
			//RDController.Instance.nodes;
			//ResearchAndDevelopment.Instance.GetTechState
			//AssetBase.RnDTechTree

			Debug.Log("[EU] START END");
		}

		private void getAllTechnologies()
		{
			//maj currenttech
			allTechResearched.Clear();
			Debug.Log("[EU] reset list : " + allTechResearched.Count);
			ProtoScenarioModule protoScenario = HighLogic.CurrentGame.scenarios.Find(x => x.moduleName == "ResearchAndDevelopment");
			foreach (ConfigNode tech in protoScenario.GetData().GetNodes("Tech"))
			{
				string node = tech.GetValue("id");
				allTechResearched.Add(node);
				Debug.Log("[EU] has tech : " + node);

			}
		}

		public void loadSave(ConfigNode loadedNode)
		{
			print("[EU] LOAD");
			if (needReload())
			{
				print("[EU] LOAD AND RELOAD");
				getAllTechnologies();
				reloadAndUpgrade();
			}

			print("[EU] RE-LOAD END");
		}

		public bool needReload()
		{
			print("[EU] needReload ? ");
			List<string> researchedNow = new List<string>();
			researchedNow.AddRange(allTechResearched);
			ProtoScenarioModule protoScenario = HighLogic.CurrentGame.scenarios.Find(x => x.moduleName == "ResearchAndDevelopment");
			foreach (ConfigNode tech in protoScenario.GetData().GetNodes("Tech"))
			{
				string node = tech.GetValue("id");

				print("[EU] researchedNow! : " + node);
				if (allTechResearched.Contains(node))
				{
					researchedNow.Remove(node);
				}
				else
				{
					//a new researched note!
					print("[EU] NEW TECH");
					return true;
				}
			}

			//test if we load a revious save without an actuel tech
			if (researchedNow.Count == 0) return false;
			else return true;
		}
		//public bool needReload()
		//{
		//	List<RDNode> researchedNow = new List<RDNode>();
		//	researchedNow.AddRange(allTechResearched);
		//	print("[EU] researchedBefore = " + researchedNow.Count);
		//	try
		//	{
		//		print("[EU] rdNode CurrentGame = " +HighLogic.CurrentGame);
		//		if(HighLogic.CurrentGame!=null){
		//			//try
		//			//{
		//			//	print("[EU] rdNode scenarios = " + HighLogic.CurrentGame.scenarios);
		//			//	print("[EU] rdNode scenarios = " + HighLogic.CurrentGame.scenarios.Count);
		//			//	print("[EU] rdNode scenarios = " + HighLogic.CurrentGame.scenarios.Find(x => x.moduleName == "ResearchAndDevelopment"));
		//			//	ProtoScenarioModule protoScenario = HighLogic.CurrentGame.scenarios.Find(x => x.moduleName == "ResearchAndDevelopment");
		//			//	print("[EU] rdNode protoScenario = " + protoScenario.moduleName);
		//			//	print("[EU] rdNode protoScenario = " + protoScenario.moduleRef);
		//			//	print("[EU] rdNode protoScenario = " + protoScenario.GetData());

		//			//}
		//			//catch (Exception e)
		//			//{
		//			//	print("error: " + e);
		//			//}
		//		}
		//		print("[EU] rdNode cunt = " + FindObjectsOfType(typeof(RDNode)) + " = " + FindObjectsOfType(typeof(RDNode)).Count());
		//		print("[EU] rdTech cunt = " + FindObjectsOfType(typeof(RDTech)) + " = " + FindObjectsOfType(typeof(RDTech)).Count());
		//		print("[EU] RDController cunt = " + FindObjectsOfType(typeof(RDController)) + " = " + FindObjectsOfType(typeof(RDController)).Count());
		//		print("[EU] ResearchAndDevelopment cunt = " + FindObjectsOfType(typeof(ResearchAndDevelopment)) + " = " + FindObjectsOfType(typeof(ResearchAndDevelopment)).Count());


		//		//print("[EU] RDController.Instance = " + RDController.Instance);
		//		//print("[EU] RDController.Instance = " + RDController.Instance.nodes);
		//		//print("[EU] RDController.Instance = " + RDController.Instance.nodes.Count);

		//	}
		//	catch (Exception e)
		//	{
		//		print("exepction : e" + e);
		//	}

		//	print("[EU] rdNode cunt = " + FindObjectsOfType(typeof(RDNode)));
		//	foreach (RDNode node in FindObjectsOfType(typeof(RDNode)))
		//	{
		//		print("[EU] tech: " + node.name);
		//		if (node.tech != null && node.IsResearched)
		//		{
		//			print("[EU] researchedNow! : " + node.name);
		//			if (allTechResearched.Contains(node))
		//			{
		//				researchedNow.Remove(node);
		//			}
		//			else
		//			{
		//				//a new researched note!
		//				print("[EU] NEW TECH");
		//				return true;
		//			}
		//		}
		//	}

		//	print("[EU] less TECH? " + researchedNow.Count);
		//	//test if we load a revious save without an actuel tech
		//	if (researchedNow.Count == 0) return false;
		//	else return true;
		//}

		public void researchDone(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> research)
		{
			Debug.Log("[EU] researchDone : " + research.target);
			//obligé que l'on a changé de tech
			if (research.target == RDTech.OperationResult.Successful)
			{
				//allTechResearched.Add(research.host.title);
				allTechResearched.Add(research.host.techID);
				reloadAndUpgrade();
			}
		}

		//TODO: pass also all tech deleted... more efficient... maybe useful
		public void reloadAndUpgrade()
		{
			try
			{

				Debug.Log("[EU] reloadAndUpgrade");
				foreach (string techName in allTechResearched)
				{
					Debug.Log("[EU] HAS tech " + techName);
				}

				//maj part
				foreach (AvailablePart ap in PartLoader.LoadedPartsList)
				{
					bool firstUgrade = true;
					List<ModuleUpgrade> allUpgrader = new List<ModuleUpgrade>();
					foreach (PartModule pm in ap.partPrefab.Modules)
					{
						//Debug.Log("[EU] get all mu : " + pm.moduleName);
						if (pm != null && pm is ModuleUpgrade)
						{
							Debug.Log("[EU] OK ");
							allUpgrader.Add((ModuleUpgrade)pm);
						}
					}
					foreach (ModuleUpgrade pm in allUpgrader)
					{
						Debug.Log("[EU] part " + ap.name);
						Debug.Log("[EU] PartModule " + pm.moduleName);
						// reset before maj
						if (firstUgrade)
						{
							Debug.Log("[EU] firstUgrade ");
							firstUgrade = false;
							//reset
							//save cost in confignode
							if (ap.partConfig.GetValue("cost") != null)
							{
								ap.cost = float.Parse(ap.partConfig.GetValue("cost"));
							}
							else
							{
								Debug.Log("[EU] reload cost null > save current cost");
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
								Debug.Log("[EU] reload title null > save current title");
								ap.partConfig.AddValue("title", ap.title);
								ap.internalConfig.AddValue("title", ap.title);
							}

							Debug.Log("[EU] ap.partConfig=" + ap.partConfig);
							//ConfigNode tempNode = new ConfigNode();
							//ap.partPrefab.OnSave(tempNode);
							////print("[EU] ap.partPrefab Save=" + tempNode);

							//ap.partPrefab.OnLoad();
							//print("[EU] after reload mass=" + ap.partPrefab.mass);
							//tempNode = new ConfigNode();
							//ap.partPrefab.OnSave(tempNode);
							//print("[EU] after reload node save  =" + tempNode);

							//ap.partPrefab.mass = float.Parse(ap.partConfig.GetValue("mass"));
							//ap.title = (ap.partConfig.GetValue("title"));

							// reload (in reverse order, of course
							foreach (ModuleUpgrade pmReset in allUpgrader.Reverse<ModuleUpgrade>())
							{
								Debug.Log("[EU] reload " + pmReset.moduleName);
								try
								{
									pmReset.restore(ap.partConfig);
								}
								catch (Exception e)
								{
									print("Error when restore " + pm.moduleName + ", module n° "
										+ ap.partPrefab.Modules.IndexOf(pm) + " in part " + ap.name + ", " + ap.title);
								}
							}
						}

						Debug.Log("[EU] UPGRADE " + pm.moduleName);
						try
						{
							pm.upgrade(allTechResearched);
						}
						catch (Exception e)
						{
							print("Error when upgrading " + pm.moduleName + ", module n° "
								+ ap.partPrefab.Modules.IndexOf(pm) + " in part " + ap.name + ", " + ap.title);
						}
						Debug.Log("[EU] UPGRADE END" + pm.moduleName);

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
