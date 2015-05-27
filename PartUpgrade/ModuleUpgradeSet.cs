/*
PartUpgrader
Copyright (c) Merill, All rights reserved.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceRace
{
	//TODO: test
	class ModuleUpgradeSet : ModuleUpgrade
	{
		public List<KeyValuePair<string, string>> tech2value = new List<KeyValuePair<string, string>>();

		public override void upgrade(List<string> allTechName)
		{
			Part p = partToUpdate();
			print("[MUS] upgrade : " + tech2value.Count + " " + moduleName);
			for (int index = tech2value.Count - 1; index >= 0; index--)
			{
				KeyValuePair<string, string> entry = tech2value[index];
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
