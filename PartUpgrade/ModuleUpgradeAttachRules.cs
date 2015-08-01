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
	public class ModuleUpgradeAttachRules : ModuleUpgradeSet
	{
		// attachment rules: stack, srfAttach, allowStack, allowSrfAttach, allowCollision
		//attachRules = 1,1,1,1,0
		public override void upgradeValue(Part p, string value)
		{
			//Debug.log("[MUAR] upgradeValue : " + value);
			string oih = value.Replace(" ", "");
			string[] all = value.Replace(" ", "").Split(new char[] { ',' });
			if (all.Length > 0)
			{
				//Debug.log("[MUAR] stack : " + p.attachRules.stack + " => " + (all[0] == "1"));
				p.attachRules.stack = all[0] == "1";
			}
			if (all.Length > 1)
			{
				//Debug.log("[MUAR] srfAttach : " + p.attachRules.srfAttach + " => " + (all[1] == "1"));
				p.attachRules.srfAttach = all[1] == "1";
			}
			if (all.Length > 2)
			{
				//Debug.log("[MUAR] allowStack : " + p.attachRules.allowStack + " => " + (all[2] == "1"));
				p.attachRules.allowStack = all[2] == "1";
			}
			if (all.Length > 3)
			{
				//Debug.log("[MUAR] allowSrfAttach : " + p.attachRules.allowSrfAttach + " => " + (all[3] == "1"));
				p.attachRules.allowSrfAttach = all[3] == "1";
			}
			if (all.Length > 4)
			{
				//Debug.log("[MUAR] allowCollision : " + p.attachRules.allowCollision + " => " + (all[4] == "1"));
				p.attachRules.allowCollision = all[4] == "1";
			}
			
		}

		public override void Restore(Part p, ConfigNode initialNode)
		{
			//p.partInfo.partPrefab.mass = float.Parse(initialNode.GetValue("attachRules"));
			upgradeValue(p, initialNode.GetValue("attachRules"));
		}

		//a bit useless. perhaps not with kis.
		public override void OnLoadInFlight(ConfigNode node)
		{
			base.OnLoadInFlight(node);
			string val = node.GetValue("attachRules");
			if (val != null)
			{
				//Can be used by kis/kas things.
				upgradeValue(part, val);
			}
		}

		public override void OnSave(ConfigNode node)
		{
			// attachment rules: stack, srfAttach, allowStack, allowSrfAttach, allowCollision
			base.OnSave(node);
			node.AddValue("attachRules",
				(part.attachRules.stack ? "1," : "0,")
				+ (part.attachRules.srfAttach ? "1," : "0,")
				+ (part.attachRules.allowStack ? "1," : "0,")
				+ (part.attachRules.allowSrfAttach ? "1," : "0,")
				+ (part.attachRules.allowCollision ? "1" : "0"));
		}
		
	}
}
