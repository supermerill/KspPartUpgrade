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
	public abstract class ModuleUpgrade : PartModule
	{

		/**
		 * Set it to false to desactivate persistance
		 * */
		[KSPField(isPersistant = true)]
		public bool persitance = true;

		/*
		 * Method to override to upgrade your part.
		 * Please use the tech name list to check what upgrade to use.
		 * */
		public abstract void Upgrade(List<string> allTechName);

		/*
		 * Method to override to Restore your part before an upgrade.
		 * Use the ConfigNode of your part to get your Restore data.
		 * */
		public abstract void Restore(ConfigNode initialNode);

		/*
		 * Method to override to load complex data stored in cfg.
		 * */
		public virtual void OnLoadIntialNode(ConfigNode node) { }

		/*
		 * Method to override to load persited data. It's only called if persistant and if the vessel is in flight.
		 * */
		public virtual void OnLoadInFlight(ConfigNode node) { }


		// utility method, old code from when i think i could use stub part to upgrade the real part.
		//you can use "this.part" directly in your code.
		public Part partToUpdate()
		{
			return part.partInfo.partPrefab;
		}

		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
			//load part prefab
			if (HighLogic.LoadedScene == GameScenes.LOADING)
			{
				OnLoadIntialNode(node);
			}

			//load only in flight and if persitance is set to true
			// if a subclass doesn't want to load at vessel creation, he can use "if(vessel!=null)"
			if (HighLogic.LoadedSceneIsFlight && persitance)
			{
				OnLoadInFlight(node);
			}
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
