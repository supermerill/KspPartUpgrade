using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
	class ModuleUpgradeMaxTemp : ModuleUpgradeMonoValue
	{

		public override void upgradeValue(Part p, float value)
		{
			p.maxTemp += value;
		}

		public override void restore(Part p, ConfigNode initialNode)
		{
			p.maxTemp = float.Parse(initialNode.GetValue("maxTemp"));
		}

		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
			string val = node.GetValue("maxTemp");
			if (val != null)
			{
				part.maxTemp = float.Parse(val);
			}
		}

		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
			node.AddValue("maxTemp", part.maxTemp);
		}
		
	}
}
