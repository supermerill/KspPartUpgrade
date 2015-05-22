using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
	class ModuleUpgradeMass : ModuleUpgradeMonoValue
	{

		public override void upgradeValue(Part p, float value)
		{
			print("[ModuleUpgrade] mass before : " + part.partInfo.partPrefab.mass);
			p.partInfo.partPrefab.mass += value;
			print("[ModuleUpgrade] mass after : " + part.partInfo.partPrefab.mass);
		}

		public override void restore(Part p, ConfigNode initialNode)
		{
			p.partInfo.partPrefab.mass = float.Parse(initialNode.GetValue("mass"));
		}

		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
			string val = node.GetValue("mass");
			if (val != null)
			{
				part.mass = float.Parse(val);
			}
		}

		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
			node.AddValue("mass", part.mass);
		}
		
	}
}
