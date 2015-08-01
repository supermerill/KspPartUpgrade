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
using System.Reflection;
using UnityEngine;

namespace SpaceRace
{
	public class ModuleUpgradeReflexion : ModuleUpgradeMonoString
	{
		[KSPField]
		public string fieldName;

		public List<KeyValuePair<string, string>> tech2value = new List<KeyValuePair<string, string>>();

		public override void upgradeValue(Part p, string value)
		{
			SetValue(p, value);
		}

		//public override void Upgrade(List<string> allTechName)
		//{
		//	Part p = partToUpdate();
		//	for (int index = tech2value.Count-1; index >=0 ; index--)
		//	{
		//		KeyValuePair<string, string> entry = tech2value[index];
		//		if (allTechName.Contains(entry.Key) || HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX)
		//		{
		//			SetValue(p, entry.Value);
		//			break;
		//		}
		//	}
		//}

		//public override void Restore(ConfigNode initialNode)
		//{
		//	Restore(partToUpdate(), initialNode);
		//}

		public virtual void SetValue(Part p, string value)
		{
			FieldInfo field = typeof(Part).GetField(fieldName);
			if (field.FieldType == typeof(string))
			{
				field.SetValue(p, value);
			}
			else if (field.FieldType == typeof(long) || field.FieldType == typeof(ulong)
				|| field.FieldType == typeof(UInt64))
			{
				field.SetValue(p, long.Parse(value));
			}
			else if (field.FieldType == typeof(int) || field.FieldType == typeof(uint)
				|| field.FieldType == typeof(UInt32))
			{
				field.SetValue(p, int.Parse(value));
			}
			else if (field.FieldType == typeof(short) || field.FieldType == typeof(ushort))
			{
				field.SetValue(p, short.Parse(value));
			}
			else if (field.FieldType == typeof(bool))
			{
				field.SetValue(p, bool.Parse(value));
			}
			else if (field.FieldType == typeof(byte))
			{
				field.SetValue(p, byte.Parse(value));
			}
			else if (field.FieldType == typeof(float))
			{
				field.SetValue(p, float.Parse(value));
			}
			else if (field.FieldType == typeof(double))
			{
				field.SetValue(p, double.Parse(value));
			}
			else if (field.FieldType == typeof(Color))
			{
				field.SetValue(p, ConfigNode.ParseColor(value));
			}
			else if (field.FieldType == typeof(Color))
			{
				field.SetValue(p, ConfigNode.ParseColor(value));
			}
			else if (field.FieldType == typeof(Color32))
			{
				field.SetValue(p, ConfigNode.ParseColor32(value));
			}
			//how?
			//else if (field.FieldType == typeof(Enum))
			//{
			//	field.SetValue(p, ConfigNode.ParseEnum(value));
			//}
			else if (field.FieldType == typeof(Matrix4x4))
			{
				field.SetValue(p, ConfigNode.ParseMatrix4x4(value));
			}
			else if (field.FieldType == typeof(Quaternion))
			{
				field.SetValue(p, ConfigNode.ParseQuaternion(value));
			}
			else if (field.FieldType == typeof(QuaternionD))
			{
				field.SetValue(p, ConfigNode.ParseQuaternionD(value));
			}
			//vector3 or 2?
			else if (field.FieldType == typeof(Vector3))
			{
				field.SetValue(p, ConfigNode.ParseVector3(value));
			}
			else if (field.FieldType == typeof(Vector3d))
			{
				field.SetValue(p, ConfigNode.ParseVector3D(value));
			}
			else if (field.FieldType == typeof(Vector4))
			{
				field.SetValue(p, ConfigNode.ParseVector4(value));
			}
		}

		public override void Restore(Part p, ConfigNode node)
		{
			SetValue(p, node.GetValue(fieldName));
		}

		public override void OnLoadIntialNode(ConfigNode node)
		{
			base.OnLoadIntialNode(node);
			loadDictionnary(tech2value, "TECH-VALUE", node);
		}

		public override void OnLoadInFlight(ConfigNode node)
		{
			base.OnLoadInFlight(node);
			//do not load at pre-prelaunch (vessel creation) to let IPartMassModifier modify the mass 
			//TODO: to something for KCT (as the mass is updated at vab "launch", because vessel creation is at rollout)
			if (vessel != null)
			{
				string val = node.GetValue(fieldName);
				if (persitance && val != null)
				{
					SetValue(part, val);
				}
			}
		}

		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
			FieldInfo field = typeof(Part).GetField(fieldName);
			node.AddValue(fieldName, field.GetValue(part));
		}

	}
}
