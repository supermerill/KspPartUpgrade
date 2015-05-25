using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceRace
{
	class ModuleUpgradeResource : ModuleUpgradeMonoValue
	{
		[KSPField]
		public string resourceName;

		public override void upgradeValue(Part p, float value)
		{
			Debug.Log("[MUR] upgradeValue " + p.name + " rn=" + resourceName);
			if (resourceName == null || resourceName.Length == 0) return;
			foreach (PartResource pr in p.Resources)
			{
				Debug.Log("[MUR] has resource " + pr.resourceName + ", " + pr.maxAmount);
			}
			PartResource resource = p.Resources[resourceName];
			if (resource == null)
			{
				//create the resource
				resource = addResource(p, resourceName, value, value);
				Debug.Log("[MUR] addresource");
				if (resource == null)
				{
					Debug.LogError("Error: cannot add resource " + resourceName + " to " + p.name + " because this resource don't exist.");
					return;
				}
				AvailablePart.ResourceInfo newInfo = new AvailablePart.ResourceInfo();
				newInfo.resourceName = addSpaces(resource.resourceName);
				p.partInfo.resourceInfos.Add(newInfo);
				Debug.Log("[MUR] addresource info : " + newInfo.resourceName);
			}
			else
			{
				resource.maxAmount = (float)(resource.maxAmount + value);
				resource.amount = (float)(resource.amount + value);
				Debug.Log("[MUR] better amount ");
			}
			foreach (PartResource pr in p.Resources)
			{
				Debug.Log("[MUR] has resource (before) " + pr.resourceName + ", " + pr.maxAmount);
			}
			//reload partinfo
			updateInfo(p, resource);
		}

		public static string addSpaces(string str)
		{
			StringBuilder builder = new StringBuilder();
			bool first = true;
			foreach (char c in str)
			{
				if (!first && c < 'Z' && c > 'A')
				{
					builder.Append(' ');
				}
				builder.Append(c);
				first = false;
			}

			return builder.ToString();
		}

		public static string createPrimaryInfo(PartResource resource)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("<b>").Append(addSpaces(resource.resourceName)).Append(": </b>");
			builder.Append((decimal)resource.maxAmount);
			return builder.ToString();
		}

		public static void updateInfo(Part p, PartResource resource)
		{
			for (int i = 0; i < p.partInfo.resourceInfos.Count; i++)
			{
				AvailablePart.ResourceInfo info = p.partInfo.resourceInfos[i];
				Debug.Log("[MUR] info.resourceName = '" + info.resourceName + "'");
				//Debug.Log("[MUR] info.info = " + info.info);
				//Debug.Log("[MUR] info.primaryInfo = " + info.primaryInfo);
				//Debug.Log("[MUR] res.GetInfo = " + resource.GetInfo());
				//Debug.Log("[MUR] res.info.name = " + resource.info.name);
				//Debug.Log("[MUR] addSpaces(resource.resourceName) = '" + addSpaces(resource.resourceName) + "'");
				if (info.resourceName.Equals(addSpaces(resource.resourceName)))
				{
					//Debug.Log("[MUR] info.primaryInfo = " + info.primaryInfo);
					//Debug.Log("[MUR] info.info = " + info.info);
					info.primaryInfo = createPrimaryInfo(resource);
					info.info = createInfo(resource);
				}
			}
		}

		public static string createInfo(PartResource resource)
		{
			StringBuilder builder = new StringBuilder();
			decimal maxAmount = (decimal)resource.maxAmount;
			builder.Append("Amount: ").Append(maxAmount);
			builder.Append("\nMass: ").Append(((decimal)resource.info.density) * maxAmount);
			builder.Append("\tCost: ").Append(((decimal)resource.info.unitCost) * maxAmount);
			return builder.ToString();
		}

		public override void restore(Part p, ConfigNode initialNode)
		{
			Debug.Log("[MUR] restore " + p.name + " rn=" + resourceName);
			//get the resource node
			PartResource resource = p.Resources[resourceName];
			if (p == null) return;
			if (resource != null)
			{
				foreach (ConfigNode resourceNode in initialNode.GetNodes("RESOURCE"))
				{
					//Debug.Log("[MUR] restore  node name : '" + resourceNode.GetValue("name") + "' ? '" + resourceName + "'");
					if (resourceNode.GetValue("name") == resourceName)
					{
						//find!
						resource.maxAmount = double.Parse(resourceNode.GetValue("maxAmount"));
						resource.amount = double.Parse(resourceNode.GetValue("amount"));
						//reload partinfo
						updateInfo(p, resource);
						Debug.Log("[MUR] reset " + resource.resourceName + " resource at " + resource.amount + " / " + resource.maxAmount);
						return;
					}
				}
				//can't find : remove
				Debug.Log("[MUR] remove resource B (count) : " + p.Resources.Count + " ? " + p.Resources.list.Count);
				removeResource(resource, p);
				Debug.Log("[MUR] remove resource (count) : " + p.Resources.Count + " ? " + p.Resources.list.Count);

			}
			else
			{
				Debug.Log("[MUR] can't find the resource " + resourceName);
			}
		}

		// return maxmass
		public static PartResource addResource(Part p, string name, float amount, float maxAmount)
		{
			ConfigNode newPartResource = new ConfigNode("RESOURCE");
			newPartResource.AddValue("name", name);
			newPartResource.AddValue("amount", amount);
			newPartResource.AddValue("maxAmount", maxAmount);
			return p.AddResource(newPartResource);
		}

		public override void OnSave(ConfigNode node)
		{
			//save resource amount/max
			base.OnSave(node);
			PartResource resource = part.Resources[resourceName];
			if (resource == null)
			{
				node.AddValue("present", false);
			}
			else
			{
				node.AddValue("present", true);
				node.AddValue("amount", resource.amount);
				node.AddValue("maxAmount", resource.maxAmount);
			}

		}

		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
			Debug.Log("[MUR] load in ?" + HighLogic.LoadedScene);
			PartResource resource = part.Resources[resourceName];
			//in flight ?
			if (node.GetValue("present") != null && !HighLogic.LoadedSceneIsEditor)
			{ 
				if (bool.Parse(node.GetValue("present")))
				{
					if (resource == null)
					{
						resource = addResource(part, resourceName, float.Parse(node.GetValue("amount")),
							float.Parse(node.GetValue("maxAmount")));
					}
					else
					{
						resource.maxAmount = float.Parse(node.GetValue("maxAmount"));
						resource.amount = float.Parse(node.GetValue("amount"));
					}
				}
				else
				{
					if (resource != null)
					{
						//removeResource(resource, part);
						//can't remove gui / need to find a way
						//this work, anyway.
						Debug.Log("[MUR] load in flight (rem) " + resource.maxAmount);
						resource.enabled = false;
						resource.amount = 0;
						resource.maxAmount = 0;
					}
				}
			}
			else if (HighLogic.LoadedSceneIsEditor)
			{
				Debug.Log("[MUR] load in editor " + resource);
				Debug.Log("[MUR] load in editor " + part.partInfo);
				Debug.Log("[MUR] load in editor " + part.partInfo.partPrefab);
				Debug.Log("[MUR] load in editor " + resourceName);
				Debug.Log("[MUR] load in editor " + part.partInfo.partPrefab.Resources[resourceName]);
				//update from partPrefab
				//add : auto
				//remove: not supported
				//modify: \/
				PartResource resourcePrefab = part.partInfo.partPrefab.Resources[resourceName];
				Debug.Log("[MUR] load in editor " + resource.maxAmount + " => " + resourcePrefab.maxAmount);
				resource.maxAmount = 20;// resourcePrefab.maxAmount;
				resource.amount = 2;//resourcePrefab.amount;
				Debug.Log("[MUR] load in editor done! " + resource.maxAmount + " <=> " + resourcePrefab.maxAmount);
			}
		}

		private static void removeResource(PartResource resource, Part p)
		{
			//remove partinfo
			for (int i = 0; i < p.partInfo.resourceInfos.Count; i++)
			{
				AvailablePart.ResourceInfo info = p.partInfo.resourceInfos[i];
				try
				{
					//Debug.Log("[MUR] remove? info.resourceName = '" + info.resourceName + "'");
					//Debug.Log("[MUR] remove? resource.resourceName = '" + resource.resourceName + "'");
					//Debug.Log("[MUR] remove? addSpaces(resource.resourceName) = '" + addSpaces(resource.resourceName) + "'");
					if (info.resourceName.Equals(addSpaces(resource.resourceName)))
					{
						Debug.Log("[MUR] remove!!! info.resourceName = '" + info.resourceName + "'");
						p.partInfo.resourceInfos.Remove(info);
						break;
					}
				}
				catch (Exception e)
				{
					Debug.LogError("Error: " + e);
				}
			}
			//remove resource
			p.Resources.list.Remove(resource);
			Destroy(resource);
			//// don't know what it does, but the name is cool
			//p.Resources.UpdateList();
		}


	}
}
