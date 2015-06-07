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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceRace
{
	public class ModuleUpgradeEngine : ModuleUpgrade
	{

		public List<MUE_TechValue> allValues;

		public override void Upgrade(List<string> allTechName)
		{
			//find the last ok node
			for (int i = allValues.Count - 1; i >= 0; i--)
			{
				MUE_TechValue val = allValues[i];

				Part p = partToUpdate();
				//check tech (or SANDBOX mode)
				if (val.tech != null && (allTechName.Contains(val.tech) || HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX))
				{
					setIspThrustFuelFLow(p, val.minThrust, val.maxThrust, val.atmosphereIsp, val.vacuumIsp, val.heatProduction);
					majInfo(p, getModuleEngines(p));
					break;
				}
			}
		}


		public override void Restore(ConfigNode initialNode)
		{
			Part p = partToUpdate();
			//get engine node
			foreach (ConfigNode intialModuleNode in initialNode.GetNodes("MODULE"))
			{
				if (intialModuleNode.GetValue("name") == "ModuleEngines"
					|| intialModuleNode.GetValue("name") == "ModuleEnginesFX")
				{
					ConfigNode ispCurveNode = intialModuleNode.GetNode("atmosphereCurve");
					FloatCurve ispCurve = new FloatCurve();
					ispCurve.Load(ispCurveNode);
					setIspThrustFuelFLow(p,
						float.Parse(intialModuleNode.GetValue("minThrust")),
						float.Parse(intialModuleNode.GetValue("maxThrust")),
						ispCurve.Curve.keys[1].value,
						ispCurve.Curve.keys[0].value,
						float.Parse(intialModuleNode.GetValue("heatProduction"))
						);
					majInfo(p, getModuleEngines(p));
				}
			}
		}

		public override void OnLoadIntialNode(ConfigNode node)
		{
			base.OnLoadIntialNode(node);
			//load the partprefab infos
			foreach (ConfigNode techValue in node.GetNodes("TECH-VALUE"))
			{
				if (allValues == null) allValues = new List<MUE_TechValue>();
				allValues.Add(new MUE_TechValue(techValue));
			}
		}

		public override void OnLoadInFlight(ConfigNode node)
		{
			base.OnLoadInFlight(node);
			//info is here?
			if (node.GetValue("vacIsp") != null)
			{
				setIspThrustFuelFLow(part,
							float.Parse(node.GetValue("minThrust")),
							float.Parse(node.GetValue("maxThrust")),
							float.Parse(node.GetValue("atmoIsp")),
							float.Parse(node.GetValue("vacIsp")),
							float.Parse(node.GetValue("heatProduction"))
							);
			}
		}

		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
			//get module
			ModuleEngines mod = getModuleEngines(part);
			node.AddValue("minThrust", mod.minThrust);
			node.AddValue("maxThrust", mod.maxThrust);
			node.AddValue("minFuelFlow", mod.minFuelFlow);
			node.AddValue("maxFuelFlow", mod.maxFuelFlow);
			node.AddValue("atmoIsp", mod.atmosphereCurve.Curve.keys[1].value);
			node.AddValue("vacIsp", mod.atmosphereCurve.Curve.keys[0].value);
			node.AddValue("heatProduction", mod.heatProduction);
		}

		public static ModuleEngines getModuleEngines(Part p)
		{
			PartModule pm = null;
			if (p.Modules.Contains("ModuleEngines")) pm = p.Modules["ModuleEngines"];
			if (pm == null) pm = p.Modules["ModuleEnginesFX"];
			if (pm != null) return pm as ModuleEngines;
			Debug.LogError("[MUE] Error: can't find an engine in the part " + p.partName);
			return null;
		}

		public static void setIspThrustFuelFLow(Part p, float minThrust, float maxThrust,
			float atmoIsp, float vacuumIsp, float heatProduction)
		{
			//get module
			ModuleEngines me = getModuleEngines(p);
			Debug.Log("[MUE] setIspThrustFuelFLow with enine  " + me + " for part " + p.partName);

			//heat production
			if (heatProduction >= 0)
				me.heatProduction = heatProduction;


			// ----- change thrust (for info => trust is compute from ips & fuelflow)
			if (minThrust >= 0)
				me.minThrust = minThrust;
			if (maxThrust >= 0)
				me.maxThrust = maxThrust;
			Debug.Log("[MUE] setIspThrustFuelFLow thrust setted to  " + me.maxThrust);

			// ----  change isp
			List<Keyframe> newCurve = new List<Keyframe>();

			//add?
			//foreach (Keyframe val in me.atmosphereCurve.Curve.keys)
			//{
			//	newCurve.Add(new Keyframe(val.time, val.value + vacuumIsp));
			//}

			//set?
			//TODO: better tangent calculus
			int i = 0;
			newCurve.Add(new Keyframe(me.atmosphereCurve.Curve.keys[i].time,
				(vacuumIsp >= 0 ? vacuumIsp : me.atmosphereCurve.Curve.keys[i].value),
				me.atmosphereCurve.Curve.keys[i].inTangent, me.atmosphereCurve.Curve.keys[i].outTangent));
			i++;
			newCurve.Add(new Keyframe(me.atmosphereCurve.Curve.keys[i].time,
				(atmoIsp >= 0 ? atmoIsp : me.atmosphereCurve.Curve.keys[i].value),
				me.atmosphereCurve.Curve.keys[i].inTangent, me.atmosphereCurve.Curve.keys[i].outTangent));
			i++;
			//copy the other keys
			for (; i < me.atmosphereCurve.Curve.keys.Length; i++)
			{
				newCurve.Add(new Keyframe(me.atmosphereCurve.Curve.keys[i].time,
					me.atmosphereCurve.Curve.keys[i].value,
					me.atmosphereCurve.Curve.keys[i].inTangent,
					me.atmosphereCurve.Curve.keys[i].outTangent));
			}

			me.atmosphereCurve = new FloatCurve(newCurve.ToArray());
			Debug.Log("[MUE] setIspThrustFuelFLow isp0 setted to  " + me.atmosphereCurve.Curve.keys[0].value);
			Debug.Log("[MUE] setIspThrustFuelFLow isp1 setted to  " + me.atmosphereCurve.Curve.keys[1].value);
			if (me.atmosphereCurve.Curve.keys.Length > 2)
				Debug.Log("[MUE] setIspThrustFuelFLow isp2 setted to  " + me.atmosphereCurve.Curve.keys[2].value);

			// ------ change fuel flow 
			//TODO: use ModuleEngines.g? or 9.80665 ?
			me.minFuelFlow = (float)(me.minThrust / (9.80665 * me.atmosphereCurve.Curve.keys[0].value));
			me.maxFuelFlow = (float)(me.maxThrust / (9.80665 * me.atmosphereCurve.Curve.keys[0].value));
			Debug.Log("[MUE] setIspThrustFuelFLow minFuelFlow setted to  " + me.minFuelFlow);
			Debug.Log("[MUE] setIspThrustFuelFLow maxFuelFlow setted to  " + me.maxFuelFlow);

			//TODO: test if really needed (old code)
			ConfigNode cn = new ConfigNode();
			me.OnSave(cn);
			me.OnLoad(cn);
			Debug.Log("[MUE] setIspThrustFuelFLow save load");
		}

		public static void majInfo(Part p, ModuleEngines me)
		{

			//update info
			for (int i = 0; i < p.partInfo.moduleInfos.Count; i++)
			{
				AvailablePart.ModuleInfo info = p.partInfo.moduleInfos[i];
				if (info.moduleName == "Engine" || info.moduleName == "EngineFX")
				{
					info.primaryInfo = me.GetPrimaryField();
					info.info = me.GetInfo();
					info.onDrawWidget = me.GetDrawModulePanelCallback();
					break;
				}
			}
		}
	}

	public class MUE_TechValue
	{

		//[KSPField(isPersistant = true)]
		public string tech = null;

		//[KSPField]
		public float vacuumIsp = -1;

		//[KSPField]
		public float atmosphereIsp = -1;

		//[KSPField]
		public float maxThrust = -1;

		//[KSPField]
		public float minThrust = -1;

		public float heatProduction = -1;

		public MUE_TechValue(ConfigNode node)
		{
			tech = node.GetValue("tech");
			string tempValue = node.GetValue("minThrust");
			if (tempValue != null)
				minThrust = float.Parse(tempValue);
			tempValue = node.GetValue("maxThrust");
			if (tempValue != null)
				maxThrust = float.Parse(tempValue);
			tempValue = node.GetValue("atmosphereIsp");
			if (tempValue != null)
				atmosphereIsp = float.Parse(tempValue);
			tempValue = node.GetValue("vacuumIsp");
			if (tempValue != null)
				vacuumIsp = float.Parse(tempValue);
			tempValue = node.GetValue("heatProduction");
			if (tempValue != null)
				heatProduction = float.Parse(tempValue);
		}

	}
}
