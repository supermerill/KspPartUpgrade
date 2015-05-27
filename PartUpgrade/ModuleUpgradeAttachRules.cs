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
	class ModuleUpgradeAttachRules : ModuleUpgradeSet
	{
		// attachment rules: stack, srfAttach, allowStack, allowSrfAttach, allowCollision
		//attachRules = 1,1,1,1,0
		public override void upgradeValue(Part p, string value)
		{
			Debug.Log("[MUAR] upgradeValue : " + value);
			string oih = value.Replace(" ", "");
			string[] all = value.Replace(" ", "").Split(new char[]{','});
			if (all.Length > 0)
			{
				Debug.Log("[MUAR] stack : " + p.attachRules.stack + " => " + (all[0] == "1"));
				p.attachRules.stack = all[0] == "1";
			}
			else if (all.Length > 1)
			{
				Debug.Log("[MUAR] srfAttach : " + p.attachRules.srfAttach + " => " + (all[1] == "1"));
				p.attachRules.srfAttach = all[1] == "1";
			}
			else if (all.Length > 2)
			{
				Debug.Log("[MUAR] allowStack : " + p.attachRules.allowStack + " => " + (all[2] == "1"));
				p.attachRules.allowStack = all[2] == "1";
			}
			else if (all.Length > 3)
			{
				Debug.Log("[MUAR] allowSrfAttach : " + p.attachRules.allowSrfAttach + " => " + (all[3] == "1"));
				p.attachRules.allowSrfAttach = all[3] == "1";
			}
			else if (all.Length > 4)
			{
				Debug.Log("[MUAR] allowCollision : " + p.attachRules.allowCollision + " => " + (all[4] == "1"));
				p.attachRules.allowCollision = all[4] == "1";
			}
			
		}

		public override void restore(Part p, ConfigNode initialNode)
		{
			//p.partInfo.partPrefab.mass = float.Parse(initialNode.GetValue("attachRules"));
			upgradeValue(p, initialNode.GetValue("attachRules"));
		}

		//a bit useless. perhaps not with kis.
		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
			string val = node.GetValue("attachRules");
			if (val != null)
			{
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
