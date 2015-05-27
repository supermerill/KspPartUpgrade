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
	//TOOD: doesn't work yet.
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
					Debug.Log("[MUT] upgrade title tech : " + tech + " : " + p.partInfo.title+" + "+addSuffix + " . " + newTitle);
					//AvailablePart aPart = PartLoader.getPartInfoByName(partName);
					if (newTitle != "")
					{
						p.partInfo.title = newTitle;
					}
					if (addSuffix != "")
					{
						p.partInfo.title += addSuffix;
					}
					Debug.Log("[MUT] upgrade title after : " + tech + " : " + p.partInfo.title);
					//TODO: redo the partinfo
				}
		}

		public override void restore(ConfigNode initialNode)
		{
			partToUpdate().partInfo.title = initialNode.GetValue("title");
		}



		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
			foreach(BaseField field in part.Fields)
			{
				Debug.Log("[MUT] field" + field.name + " " + field.guiName + " => " + field.host);
			}
		}

	}
}
