using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
	class ModuleUpgrade : PartModule
	{
		[KSPField]
		public string partName = null;

		public virtual void upgrade(List<string> allTechName)
		{

		}

		public virtual void restore(ConfigNode initialNode)
		{

		}

		public Part partToUpdate()
		{
			Part p = null;
			if (partName == null)
			{
				p = part.partInfo.partPrefab;
			}
			else
			{
				AvailablePart aPart = PartLoader.getPartInfoByName(partName);
				p = aPart.partPrefab;
			}
			return p;
		}

		public static void saveDictionnary<X>(Dictionary<string, X> dico, string name, ConfigNode root)
		{
			ConfigNode nodeDico = new ConfigNode(name);
			foreach (KeyValuePair<string, X> entry in dico)
			{
				nodeDico.AddValue(entry.Key, entry.Value);
			}
			root.AddNode(nodeDico);
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

	}
}
