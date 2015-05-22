using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
	class ModuleUpgradeCost : ModuleUpgradeMonoValue
	{
		public override void upgradeValue(Part p, float value)
		{
			print("[ModuleUpgrade] cost before : " + part.partInfo.partPrefab.mass);
			p.partInfo.cost += value;
			print("[ModuleUpgrade] cost after : " + part.partInfo.partPrefab.mass);
		}

		public override void restore(Part p, ConfigNode initialNode)
		{
			p.partInfo.cost = float.Parse(initialNode.GetValue("cost"));
		}
	}
}
