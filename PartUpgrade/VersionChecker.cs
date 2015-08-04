using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace PartUpgrade
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	class VersionChecker : MonoBehaviour
	{

		public virtual void Start()
		{
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			IEnumerable<AssemblyLoader.LoadedAssembly> partUpgradeAssemblies = from a in AssemblyLoader.loadedAssemblies
										where a.assembly.GetName().Name == currentAssembly.GetName().Name
										//orderby a.path ascending
										orderby System.Diagnostics.FileVersionInfo.GetVersionInfo(a.path).FileVersion descending, a.path ascending
										select a;

			//foreach (AssemblyLoader.LoadedAssembly assemblyContainer in eligible)
			//{
			//	Debug.Log("[PartUpgrade] Assembly loaded FileVersion: "
			//		+ System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyContainer.path).FileVersion);
			//	Debug.Log("[PartUpgrade] Assembly loaded Location: "
			//						+ assemblyContainer.assembly.Location);
			//	Debug.Log("[PartUpgrade] Assembly loaded dllName: " + assemblyContainer.dllName);
			//	Debug.Log("[PartUpgrade] Assembly loaded name: " + assemblyContainer.name);
			//	Debug.Log("[PartUpgrade] Assembly loaded path: " + assemblyContainer.path);
			//	Debug.Log("[PartUpgrade] Assembly loaded versionMM: " + assemblyContainer.versionMajor+" . "
			//		+ assemblyContainer.versionMinor);
			//}

			AssemblyLoader.LoadedAssembly first = partUpgradeAssemblies.First();

			//remove 0.3 (as it's with 1.0.1 FileVersion & Product version)
			//osef.
			//if (first.path.Contains("PartUpgrade_999799_beta_v0.3.dll"))
			//{
			//	first.
			//	var list = partUpgradeAssemblies.ToList();
			//	if (list.Count > 1)
			//	{
			//		first = list[1];
			//	}
			//}



			//if (System.Diagnostics.FileVersionInfo.GetVersionInfo(currentAssembly.Location).FileVersion
			//	!= System.Diagnostics.FileVersionInfo.GetVersionInfo(eligible.First().assembly.Location).FileVersion)
			if (currentAssembly.Location != first.path)
			{
				//Debug.LogError("[PartUpgrade] Warning: Wrong PartUprade is loaded : v "
				//	+ System.Diagnostics.FileVersionInfo.GetVersionInfo(currentAssembly.Location).FileVersion
				//	+ " instead of v " +
				//System.Diagnostics.FileVersionInfo.GetVersionInfo(first.path).FileVersion);
				Debug.LogError("[PartUpgrade] Warning: Wrong PartUprade is loaded : v"
					+ System.Diagnostics.FileVersionInfo.GetVersionInfo(currentAssembly.Location).ProductVersion
					+ " instead of v" +
				System.Diagnostics.FileVersionInfo.GetVersionInfo(first.path).ProductVersion);
			}
		}

	}
}
