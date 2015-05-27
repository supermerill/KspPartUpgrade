using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceRace
{
	//testing module. no useful utility
	class ModuleTest : PartModule
	{
		[KSPField(isPersistant=true)]
		public int testPersist = 10;

		[KSPField]
		public int test = 10;

		public int pasfield = -100;

		public override void OnLoad(ConfigNode node)
		{
			Debug.Log("[MT] onload");
			foreach (BaseField field in Fields)
			{
				Debug.Log("[MT] field " + field.name + " is persistant? " + field.isPersistant + " has value " + field.host);
			}
			base.OnLoad(node);
			foreach (BaseField field in Fields)
			{
				Debug.Log("[MT] field " + field.name + " is persistant? " + field.isPersistant + " has value " + field.host);
			}
			pasfield = test;
		}


		public override void OnSave(ConfigNode node)
		{
			Debug.Log("[MT] onSAVE " + pasfield);
			foreach (BaseField field in Fields)
			{
				Debug.Log("[MT] field " + field.name + " is persistant? " + field.isPersistant + " has value " + field.host);
			}
			base.OnSave(node);
			foreach (BaseField field in Fields)
			{
				Debug.Log("[MT] field " + field.name + " is persistant? " + field.isPersistant + " has value " + field.host);
			}
		}

	}
}
