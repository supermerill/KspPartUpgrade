using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceRace
{
	class ModuleUpgradeSet : ModuleUpgrade
	{
		public Dictionary<string, string> tech2value = new Dictionary<string, string>();

		public override void upgrade(List<string> allTechName)
		{
			Part p = partToUpdate();
			print("[MUS] upgrade : " + tech2value.Count + " " + moduleName);
			foreach (KeyValuePair<string, string> entry in tech2value)
			{
				print("[MUS] upgrade tech : " + entry);
				if (allTechName.Contains(entry.Key))
				{
					print("[MUS] upgrade !");
					upgradeValue(p, entry.Value);
				}
			}
		}

		public override void restore(ConfigNode initialNode)
		{
			print("[MUS] restore : " + tech2value.Count + " " + moduleName);
			restore(partToUpdate(), initialNode);
		}

		public virtual void upgradeValue(Part p, string value)
		{

		}

		public virtual void restore(Part p, ConfigNode node)
		{

		}

		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
			loadDictionnary(tech2value, "TECH-VALUE", node);
		}
	}
}
