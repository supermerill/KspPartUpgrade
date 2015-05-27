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

namespace SpaceRace
{
	public class ModuleUpgradeMonoValue : ModuleUpgrade
	{

		public List<KeyValuePair<string, float>> tech2value = new List<KeyValuePair<string, float>>();

		public override void Upgrade(List<string> allTechName)
		{
			Part p = partToUpdate();
			for (int index = tech2value.Count-1; index >=0 ; index--)
			{
				KeyValuePair<string, float> entry = tech2value[index];
				if (allTechName.Contains(entry.Key) || HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX)
				{
					upgradeValue(p, entry.Value);
					break;
				}
			}
		}

		public override void Restore(ConfigNode initialNode)
		{
			Restore(partToUpdate(), initialNode);
		}

		public virtual void upgradeValue(Part p, float value)
		{

		}

		public virtual void Restore(Part p, ConfigNode node)
		{

		}

		public override void OnLoadIntialNode(ConfigNode node)
		{
			base.OnLoadIntialNode(node);
			loadDictionnary(tech2value, "TECH-VALUE", node);
		}

		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
		}

	}
}
