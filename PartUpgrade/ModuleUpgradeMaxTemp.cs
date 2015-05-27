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
	class ModuleUpgradeMaxTemp : ModuleUpgradeMonoValue
	{

		public override void upgradeValue(Part p, float value)
		{
			p.maxTemp = value;
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
