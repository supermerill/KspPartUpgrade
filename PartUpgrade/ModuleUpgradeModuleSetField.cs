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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SpaceRace
{
	public class ModuleUpgradeModuleSetField : ModuleUpgradeSetField
	{

		public static HashSet<Type> persistedTypes = new HashSet<Type>(new Type[]{
			typeof(bool),typeof(byte),typeof(short),typeof(ushort),typeof(int),typeof(uint),
			typeof(long),typeof(ulong),typeof(float),typeof(double),typeof(string),
			typeof(Color),typeof(Color32),
			typeof(Matrix4x4),typeof(Quaternion),typeof(QuaternionD),
			typeof(Vector3),typeof(Vector3d),typeof(Vector4)
		});

		[KSPField]
		public string moduleName;

		public override void SetValue(Part p, string value)
		{
			//Debug.Log("[MUMSF] setvalue for " + moduleName + "." + fieldName + " to " + value);
			// get the module
			PartModule mod = p.Modules[moduleName];
			if (mod != null)
			{
				//Debug.Log("[MUMSF] mod finded");
				//set the field
				try
				{
					FieldInfo field = mod.GetType().GetField(fieldName);
					if (field != null && field.IsDefined(typeof(KSPField), true))
					{
						//Debug.Log("[MUMSF] field finded & correct");
						//Debug.log("[MUMSF] onload: " + field.FieldType.Name + " " + field.Name + " : " + field.GetValue(mod) + " => " + config.GetValue(field.Name));
						if (field.FieldType == typeof(string))
						{
							field.SetValue(mod, value);
						}
						else if (field.FieldType == typeof(long) || field.FieldType == typeof(ulong)
							|| field.FieldType == typeof(UInt64))
						{
							field.SetValue(mod, long.Parse(value));
						}
						else if (field.FieldType == typeof(int) || field.FieldType == typeof(uint)
							|| field.FieldType == typeof(UInt32))
						{
							field.SetValue(mod, int.Parse(value));
						}
						else if (field.FieldType == typeof(short) || field.FieldType == typeof(ushort))
						{
							field.SetValue(mod, short.Parse(value));
						}
						else if (field.FieldType == typeof(bool))
						{
							field.SetValue(mod, bool.Parse(value));
						}
						else if (field.FieldType == typeof(byte))
						{
							field.SetValue(mod, byte.Parse(value));
						}
						else if (field.FieldType == typeof(float))
						{
							field.SetValue(mod, float.Parse(value));
						}
						else if (field.FieldType == typeof(double))
						{
							field.SetValue(mod, double.Parse(value));
						}
						else if (field.FieldType == typeof(Color))
						{
							field.SetValue(mod, ConfigNode.ParseColor(value));
						}
						else if (field.FieldType == typeof(Color))
						{
							field.SetValue(mod, ConfigNode.ParseColor(value));
						}
						else if (field.FieldType == typeof(Color32))
						{
							field.SetValue(mod, ConfigNode.ParseColor32(value));
						}
						//how?
						//else if (field.FieldType == typeof(Enum))
						//{
						//	field.SetValue(mod, ConfigNode.ParseEnum(value));
						//}
						else if (field.FieldType == typeof(Matrix4x4))
						{
							field.SetValue(mod, ConfigNode.ParseMatrix4x4(value));
						}
						else if (field.FieldType == typeof(Quaternion))
						{
							field.SetValue(mod, ConfigNode.ParseQuaternion(value));
						}
						else if (field.FieldType == typeof(QuaternionD))
						{
							field.SetValue(mod, ConfigNode.ParseQuaternionD(value));
						}
						//vector3 or 2?
						else if (field.FieldType == typeof(Vector3))
						{
							field.SetValue(mod, ConfigNode.ParseVector3(value));
						}
						else if (field.FieldType == typeof(Vector3d))
						{
							field.SetValue(mod, ConfigNode.ParseVector3D(value));
						}
						else if (field.FieldType == typeof(Vector4))
						{
							field.SetValue(mod, ConfigNode.ParseVector4(value));
						}
					}
				}
				catch (Exception e)
				{
					//Debug.log("Warn: can't set the field " + field.Name + " : " + field.FieldType.Name + " with node value of " + value);
				}
			}
		}

		public override void OnSave(ConfigNode node)
		{
			//Debug.Log("[MUMSF] save for " + moduleName + "." + fieldName);
			// get the module
			PartModule mod = part.Modules[moduleName];
			if (mod != null)
			{
				//Debug.Log("[MUMSF] mod finded");
				//set the field
				try
				{
					FieldInfo field = mod.GetType().GetField(fieldName);
					if (field != null && field.IsDefined(typeof(KSPField), true))
					{
						//Debug.Log("[MUMSF] field finded, value is "+field.GetValue(part));
						node.AddValue(fieldName, field.GetValue(part));
					}
				}
				catch (Exception e)
				{
					Debug.LogError("[MUMSF] Error: " + e);
				}
			}
		}

	}
}
