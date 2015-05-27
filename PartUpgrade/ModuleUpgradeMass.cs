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
	public class ModuleUpgradeMass : ModuleUpgradeMonoValue
	{

		public override void upgradeValue(Part p, float value)
		{
			p.partInfo.partPrefab.mass = value;
		}

		public override void Restore(Part p, ConfigNode initialNode)
		{
			p.partInfo.partPrefab.mass = float.Parse(initialNode.GetValue("mass"));
		}

		public override void OnLoadInFlight(ConfigNode node)
		{
			base.OnLoadInFlight(node);
			//do not load at pre-prelaunch (vessel creation) to let IPartMassModifier modify the mass 
			//TODO: to something for KCT (as the mass is updated at vab "launch", because vessel creation is at rollout)
			if (vessel != null) { 
				string val = node.GetValue("mass");
				if (persitance && val != null)
				{
					part.mass = float.Parse(val);
				}
			}
		}

		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
			node.AddValue("mass", part.mass);
		}
		
	}
}
