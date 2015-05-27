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
	class ModuleUpgradeMass : ModuleUpgradeMonoValue
	{

		public override void upgradeValue(Part p, float value)
		{
			print("[ModuleUpgrade] mass before : " + part.partInfo.partPrefab.mass);
			p.partInfo.partPrefab.mass = value;
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
