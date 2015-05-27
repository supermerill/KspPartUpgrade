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
	abstract class ModuleUpgrade : PartModule
	{

		/*
		 * Method to override to upgrade your part.
		 * Please use the tech name list to check what upgrade to use.
		 * */
		public abstract virtual void upgrade(List<string> allTechName);

		/*
		 * Method to override to restore your part before an upgrade.
		 * Use the ConfigNode of your part to get your restore data.
		 * */
		public abstract virtual void restore(ConfigNode initialNode);


		// utility method, old code from when i think i could use stub part to upgrade the real part.
		//you can use part directly in your code if you wish, it must be the same.
		public Part partToUpdate()
		{
			return part.partInfo.partPrefab;
		}

		//utility save/load methods

		public static void saveDictionnary<X>(Dictionary<string, X> dico, string name, ConfigNode root)
		{
			ConfigNode nodeDico = new ConfigNode(name);
			foreach (KeyValuePair<string, X> entry in dico)
			{
				nodeDico.AddValue(entry.Key, entry.Value);
			}
			root.AddNode(nodeDico);
		}

		public static void loadDictionnary(List<KeyValuePair<string, float>> dico, string name, ConfigNode root)
		{
			if (root.HasNode(name))
			{
				if (root.GetNode(name) != null)
				{
					ConfigNode nodeDico = root.GetNode(name);
					if (nodeDico.values != null)
						foreach (ConfigNode.Value val in nodeDico.values)
						{
							if (val != null)
							{
								dico.Add(new KeyValuePair<string,float>(val.name, float.Parse(val.value)));
							}
						}
				}
			}
		}

		public static void loadDictionnary(List<KeyValuePair<string, string>> dico, string name, ConfigNode root)
		{
			if (root.HasNode(name))
			{
				if (root.GetNode(name) != null)
				{
					ConfigNode nodeDico = root.GetNode(name);
					if (nodeDico.values != null)
						foreach (ConfigNode.Value val in nodeDico.values)
						{
							if (val != null)
							{
								dico.Add(new KeyValuePair<string,string>(val.name, val.value));
							}
						}
				}
			}
		}


		public static void loadDictionnary(Dictionary<string, float> dico, string name, ConfigNode root)
		{
			if (root.HasNode(name))
			{
				if (root.GetNode(name) != null)
				{
					ConfigNode nodeDico = root.GetNode(name);
					if (nodeDico.values != null)
						foreach (ConfigNode.Value val in nodeDico.values)
						{
							if (val != null)
							{
								dico[val.name] = float.Parse(val.value);
							}
						}
				}
			}
		}

		public static void loadDictionnary(Dictionary<string, decimal> dico, string name, ConfigNode root)
		{
			if (root.HasNode(name))
			{
				if (root.GetNode(name) != null)
				{
					ConfigNode nodeDico = root.GetNode(name);
					if (nodeDico.values != null)
						foreach (ConfigNode.Value val in nodeDico.values)
						{
							if (val != null)
							{
								dico[val.name] = decimal.Parse(val.value);
							}
						}
				}
			}
		}

		public static void loadDictionnary(Dictionary<string, string> dico, string name, ConfigNode root)
		{
			if (root.HasNode(name))
			{
				if (root.GetNode(name) != null)
				{
					ConfigNode nodeDico = root.GetNode(name);
					if (nodeDico.values != null)
						foreach (ConfigNode.Value val in nodeDico.values)
						{
							if (val != null)
							{
								dico[val.name] = val.value;
							}
						}
				}
			}
		}

	}
}
