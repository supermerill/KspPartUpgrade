using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceRace
{
	class ModuleUpgradeTitle : ModuleUpgrade
	{

		[KSPField]
		public string newTitle = "";

		[KSPField]
		public string addSuffix = "";

		[KSPField]
		public string tech = "";


		public override void upgrade(List<string> allTechName)
		{
				if (allTechName.Contains(tech))
				{
					Part p = partToUpdate();
					Debug.Log("[MU] upgrade title tech : " + tech + " : " + p.partInfo.title+" + "+addSuffix + " . " + newTitle);
					//AvailablePart aPart = PartLoader.getPartInfoByName(partName);
					if (newTitle != "")
					{
						p.partInfo.title = newTitle;
					}
					if (addSuffix != "")
					{
						p.partInfo.title += addSuffix;
					}
					Debug.Log("[MU] upgrade title after : " + tech + " : " + p.partInfo.title);
					//TODO: redo the partinfo
				}
		}

		public override void restore(ConfigNode initialNode)
		{
			partToUpdate().partInfo.title = initialNode.GetValue("title");
		}

	}
}
