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
	class ModuleUpgradeCategory : ModuleUpgradeMonoString
	{
		PartCategories? category = null;

		public override void upgradeValue(Part p, string value)
		{
			if (part.partInfo != null && part.partInfo.partPrefab == part)
			{
				if (category == null) category = part.partInfo.category;
				p.partInfo.category = (PartCategories)ConfigNode.ParseEnum(typeof(PartCategories), value);
			}
		}

		public override void Restore(Part p, ConfigNode initialNode)
		{
			if (part.partInfo != null && part.partInfo.partPrefab == part)
			{
				if (category == null) category = part.partInfo.category;
				p.partInfo.category = category.HasValue ? category.Value : PartCategories.none;
			}
		}

	}
}
