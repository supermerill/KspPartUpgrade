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
				//TODO: test
				//create the resource
				resource = addResource(p, resourceName, value);
				Debug.Log("[MUR] addresource ");
				if (resource == null)
				{
					Debug.LogError("Error: cannot add resource " + resourceName + " to " + p.name + " because this resource don't exist.");
					return;
				}
				AvailablePart.ResourceInfo newInfo = new AvailablePart.ResourceInfo();
				newInfo.resourceName = addSpaces(resource.resourceName);
				p.partInfo.resourceInfos.Add(newInfo);
			}
			else
			{
				//TODO: test
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
				//Debug.Log("[MUR] info.resourceName = '" + info.resourceName + "'");
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
			foreach (ConfigNode resourceNode in initialNode.GetNodes("RESOURCE"))
			{
				Debug.Log("[MUR] restore  node name : '" + resourceNode.GetValue("name") + "' ? '" + resourceName+"'");
				if (resourceNode.GetValue("name") == resourceName)
				{
					//find!
					resource.maxAmount = double.Parse(resourceNode.GetValue("maxAmount"));
					resource.amount = double.Parse(resourceNode.GetValue("amount"));
					//reload partinfo
					updateInfo(p, resource);
					Debug.Log("[MUR] reset resource at " + resource.amount + " / " + resource.maxAmount);
					return;
				}
			}
			//can't find : remove
			Debug.Log("[MUR] remove resource B (count) : " + p.Resources.Count + " ? " + p.Resources.list.Count);
			//remove partinfo
			for (int i = 0; i < p.partInfo.resourceInfos.Count; i++)
			{
				AvailablePart.ResourceInfo info = p.partInfo.resourceInfos[i];
				try { 
				Debug.Log("[MUR] remove? info.resourceName = '" + info.resourceName + "'");
				Debug.Log("[MUR] remove? resource.resourceName = '" + resource.resourceName + "'");
				Debug.Log("[MUR] remove? addSpaces(resource.resourceName) = '" + addSpaces(resource.resourceName) + "'");
				if (info.resourceName.Equals(addSpaces(resource.resourceName)))
				{
					Debug.Log("[MUR] remove!!! info.resourceName = '" + info.resourceName + "'");
					p.partInfo.resourceInfos.Remove(info);
				}
				}
				catch (Exception e)
				{
					Debug.LogError("Error: " + e);
				}
			}
			//remove resource
			p.Resources.list.Remove(resource);
			// don't know what it does, but the name is cool
			p.Resources.UpdateList();
			Debug.Log("[MUR] remove resource (count) : " + p.Resources.Count + " ? " + p.Resources.list.Count);
		}

		// return maxmass
		public static PartResource addResource(Part p, string name, float amount)
		{
			ConfigNode newPartResource = new ConfigNode("RESOURCE");
			newPartResource.AddValue("name", name);
			newPartResource.AddValue("amount", amount);
			newPartResource.AddValue("maxAmount", amount);
			return p.AddResource(newPartResource);
		}

	}
}
