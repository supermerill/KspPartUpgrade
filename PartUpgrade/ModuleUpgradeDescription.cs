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


namespace SpaceRace
{
	public class ModuleUpgradeDescription : ModuleUpgradeMonoString
	{

		public override void upgradeValue(Part p, string value)
		{
			p.partInfo.description = value;
		}

		public override void Restore(Part p, ConfigNode initialNode)
		{
			p.partInfo.description = initialNode.GetValue("description");
		}

		//public List<KeyValuePair<string, string>> tech2value = new List<KeyValuePair<string, string>>();

		//public override void Upgrade(List<string> allTechName)
		//{
		//	Part p = partToUpdate();
		//	for (int index = tech2value.Count - 1; index >= 0; index--)
		//	{
		//		KeyValuePair<string, string> entry = tech2value[index];
		//		if (allTechName.Contains(entry.Key) || HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX)
		//		{
		//			p.partInfo.description = entry.Value;
		//			break;
		//		}
		//	}
		//}

		//public override void Restore(ConfigNode initialNode)
		//{
		//	partToUpdate().partInfo.description = initialNode.GetValue("description");
		//}

		//public override void OnLoadIntialNode(ConfigNode node)
		//{
		//	base.OnLoadIntialNode(node);
		//	loadDictionnary(tech2value, "TECH-VALUE", node);
		//}
		
	}
}
