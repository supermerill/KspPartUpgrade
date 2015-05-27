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
	//TODO: test
	public abstract class ModuleUpgradeSet : ModuleUpgrade
	{
		public List<KeyValuePair<string, string>> tech2value = new List<KeyValuePair<string, string>>();

		public override void Upgrade(List<string> allTechName)
		{
			Part p = partToUpdate();
			print("[MUS] upgrade : " + tech2value.Count + " " + moduleName);
			for (int index = tech2value.Count - 1; index >= 0; index--)
			{
				KeyValuePair<string, string> entry = tech2value[index];
				print("[MUS] upgrade tech : " + entry);
				if (allTechName.Contains(entry.Key) || HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX)
				{
					print("[MUS] upgrade !");
					upgradeValue(p, entry.Value);
					break;
				}
			}
		}

		public override void Restore(ConfigNode initialNode)
		{
			print("[MUS] Restore : " + tech2value.Count + " " + moduleName);
			Restore(partToUpdate(), initialNode);
		}

		public abstract void upgradeValue(Part p, string value);

		public virtual void Restore(Part p, ConfigNode node)
		{

		}

		public override void OnLoadIntialNode(ConfigNode node)
		{
			base.OnLoadIntialNode(node);
			loadDictionnary(tech2value, "TECH-VALUE", node);
		}
	}
}
