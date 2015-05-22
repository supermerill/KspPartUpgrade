using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceRace
{
	class ModuleUpgradeEngine : ModuleUpgrade
	{

		[KSPField]
		public int ispAdd = 0;

		[KSPField]
		public int thrustAdd = 0;

		public override void upgrade(List<string> allTechName)
		{

			//upgrade thrust
			//print("[SPACERACE]ModuleUpgradeThrust upgrade " + partName + ", " + thrustAdd);
			//AvailablePart aPart = PartLoader.getPartInfoByName(partName);
			//print("[SPACERACE]ModuleUpgradeThrust apart = " + aPart);
			PartModule pm = part.partInfo.partPrefab.Modules["ModuleEngines"];
			//print("[SPACERACE]ModuleUpgradeThrust PartModule = " + pm);
			ModuleEngines me = (ModuleEngines)pm;
			//print("[SPACERACE]ModuleUpgradeThrust ModuleEngines thrust = " + me.maxThrust);
			me.maxThrust += thrustAdd;
			//print("[SPACERACE]ModuleUpgradeThrust ModuleEngines thrust AFTER = " + me.maxThrust);

			//upgrade isp
			//print("[SPACERACE]ModuleUpgradeIsp realIsp before = " + me.realIsp);
			List<Keyframe> newCurve = new List<Keyframe>();
			foreach (Keyframe val in me.atmosphereCurve.Curve.keys)
			{
				//print("[SPACERACE]ModuleUpgradeIsp before = " + val.value);
				//print("[SPACERACE]ModuleUpgradeIsp after = " + val.value + ispAdd);
				newCurve.Add(new Keyframe(val.time, val.value + ispAdd));
			}
			me.atmosphereCurve = new FloatCurve(newCurve.ToArray());
			//print("[SPACERACE]ModuleUpgradeIsp realIsp after = " + me.realIsp);

			//if (me is ModuleBetterEngines)
			//{
			//	print("[SPACERACE]ModuleUpgradeThrust ModuleBetterEngines updateThrust!!");
			//	((ModuleBetterEngines)me).updateThrust();
			//}

			//upgrade fuel flow
			//print("[SPACERACE]ModuleUpgradeThrust Part minFuelFlow " + ((ModuleEngines)part.Modules["ModuleEngines"]).minFuelFlow);
			//print("[SPACERACE]ModuleUpgradeThrust Part maxFuelFlow " + ((ModuleEngines)part.Modules["ModuleEngines"]).maxFuelFlow);

			//print("[SPACERACE]ModuleUpgradeThrust Part isp vac " + me.atmosphereCurve.Curve.keys[0].value);
			me.maxFuelFlow = (float)(me.maxThrust / (9.80665 * me.atmosphereCurve.Curve.keys[0].value));

			//print("[SPACERACE]ModuleUpgradeThrust Part New maxFuelFlow " + me.maxFuelFlow);

			ConfigNode cn = new ConfigNode();
			me.OnSave(cn);
			me.OnLoad(cn);

			for (int i = 0; i < part.partInfo.moduleInfos.Count; i++)
			//foreach (AvailablePart.ModuleInfo info in aPart.moduleInfos)
			{
				AvailablePart.ModuleInfo info = part.partInfo.moduleInfos[i];
				//print("[SPACERACE]ModuleUpgradeThrust AvailablePart.ModuleInfo [" + info.moduleName + "]\n[" + info.primaryInfo + "]\n[" + info.info + "]");
				if (info.moduleName == "Engine")
				{
					//print("[SPACERACE]ModuleUpgradeThrust AvailablePart info update");
					info.primaryInfo = me.GetPrimaryField();
					info.info = me.GetInfo();
					info.onDrawWidget = me.GetDrawModulePanelCallback();
				}
			}
		}

	}
}
