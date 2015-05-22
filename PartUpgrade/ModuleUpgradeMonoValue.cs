using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
	class ModuleUpgradeMonoValue : ModuleUpgrade
	{

		public Dictionary<string, float> tech2value = new Dictionary<string, float>();

		public override void upgrade(List<string> allTechName)
		{
			Part p = partToUpdate();
			print("[MU] upgrade : " + tech2value.Count+" "+moduleName);
			foreach (KeyValuePair<string, float> entry in tech2value)
			{
				print("[MU] upgrade tech : " + entry);
				if (allTechName.Contains(entry.Key))
				{
					print("[MU] upgrade !");
					upgradeValue(p, entry.Value);
				}
			}
		}

		public override void restore(ConfigNode initialNode)
		{
			print("[MU] restore : " + tech2value.Count + " " + moduleName);
			restore(partToUpdate(), initialNode);
		}

		public virtual void upgradeValue(Part p, float value)
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

		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
			//saveDictionnary(tech2value, "TECH-VALUE", node);
		}

	}
}
