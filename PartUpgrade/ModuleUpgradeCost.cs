using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Text;

namespace SpaceRace
{
	public class ModuleUpgradeCost : ModuleUpgradeMonoValue
	{
		public override void upgradeValue(Part p, float value)
		{
			p.partInfo.cost = value;
		}

		public override void Restore(Part p, ConfigNode initialNode)
		{
			p.partInfo.cost = float.Parse(initialNode.GetValue("cost"));
		}

		//don't need to save/load part in flight, as we can't and it's not useful (maybe if recover...)
	}
}
