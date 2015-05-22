using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SpaceRace
{
	class ModuleUpgradeModule : ModuleUpgrade
	{

		[KSPField]
		public string type = "replace";

		[KSPField]
		public string tech;

		[KSPField]
		public bool persistant = true;

		public ConfigNode[] configModule;

		public override void upgrade(List<string> allTechName)
		{
			if (moduleName == null || moduleName.Length == 0 || tech == null || !allTechName.Contains(tech)) return;
			Part p = partToUpdate();

			foreach (ConfigNode config in configModule)
			{
				Debug.Log("[MUM] upgrade a module  "+p.name+" , " + p.partInfo.moduleInfos.Count + "/ " + p.Modules.Count + " , " + type);
				//get the module
				if (type.Equals("replace"))
				{
					try
					{
						//get the module
						//ConfigNode node = new ConfigNode();
						PartModule mod = p.Modules[config.GetValue("name")];
						//mod.Save(node);
						//Debug.Log("[MUM] before : " + node);
						mod.Load(config);
						//node = new ConfigNode(); mod.Save(node);
						//Debug.Log("[MUM] after : " + node);
						Debug.Log("[MUM] after (info) : " + mod.GetInfo());
						for (int i = 0; i < part.partInfo.moduleInfos.Count; i++)
						{
							AvailablePart.ModuleInfo info = part.partInfo.moduleInfos[i];
							Debug.Log("[MUM] searc info : " + info.moduleName + " =?= " + mod.moduleName.Replace("Module", ""));
							if (info.moduleName.Equals(mod.moduleName.Replace("Module", "")))
							{
								info.info = mod.GetInfo();
								Debug.Log("[MUM] replace!");
							}
						}
					}
					catch (Exception e)
					{
						Debug.Log("[MUM] can't MODIFY " + e);
					}
				}
				else if (type.Equals("create"))
				{
					try
					{
						Debug.Log("[MUM] create : " + type + " : " + p);
						//thanks to ialdabaoth for this
						PartModule newMod = part.AddModule(config.GetValue("name"));
						if (Awaken(newMod))
						{ // uses reflection to find and call the PartModule.Awake() private method
							newMod.Load(config);
						}
						Debug.Log("[MUM] create newMod : " + newMod);
						Debug.Log("[MUM] create now add info ? : " + p.Modules[config.GetValue("name")].GetInfo());

						//add info
						if (newMod.GetInfo().Length > 0)
						{
							AvailablePart.ModuleInfo info = new AvailablePart.ModuleInfo();
							info.moduleName = newMod.moduleName.Replace("Module", "");
							info.info = newMod.GetInfo();
							p.partInfo.moduleInfos.Add(info);
						}
					}
					catch (Exception e)
					{
						Debug.Log("[MUM] can't ADD " + e);
					}
				}
				else if (type.Equals("delete"))
				{
					try
					{
						Debug.Log("[MUM] delete : " + type);
						PartModule mod = p.Modules[config.GetValue("name")];
						p.RemoveModule(p.Modules[config.GetValue("name")]);
						Debug.Log("[MUM] delete now remove info ? : " + mod.GetInfo());
						if (mod.GetInfo().Length > 0)
						{
							List<AvailablePart.ModuleInfo> allInfos = new List<AvailablePart.ModuleInfo>();
							allInfos.AddRange(part.partInfo.moduleInfos);
							foreach (AvailablePart.ModuleInfo infoToDel in allInfos)
							{
								if (infoToDel.moduleName.Equals(mod.moduleName.Replace("Module", "")))
								{
									part.partInfo.moduleInfos.Remove(infoToDel);
									break;
								}
							}
						}

					}
					catch (Exception e)
					{
						Debug.Log("[MUM] can't REMOVE " + e);
					}
				}
				Debug.Log("[MUM] relance ");
			}
		}

		public override void restore(ConfigNode initialNode)
		{
			if (moduleName == null || moduleName.Length == 0) return;
			Part p = partToUpdate();

			Debug.Log("[MUM] restore ");
			foreach (ConfigNode config in configModule)
			{
				Debug.Log("[MUM] restore a module  " + config .GetValue("name")+ " " + p.partInfo.moduleInfos.Count + "/ "
					+ p.Modules.Count + " , " + type + " persistant=" + persistant);
				if (type.Equals("replace"))
				{
					try
					{
						//get the module
						//ConfigNode node = new ConfigNode();
						PartModule mod = p.Modules[config.GetValue("name")];
						//make all fields persistant
						Debug.Log("[MUM] make all fields persistant " + mod.Fields.Count);
						if (persistant)
						{
							foreach (BaseField f in mod.Fields)
							{
								Debug.Log("[MUM] before " + f.name + " ispersistant = " + f.isPersistant);
								//do NOTHING
								f.isPersistant = true;
								Debug.Log("[MUM] set "+f.name+" ispersistant = "+f.isPersistant);
								//get the KSPField f the object
								//f.host.
								//Part p;
								//IConfigNode icn;
							}
							try { 
							Debug.Log("[MUM] make mod.snapshot? " + mod.snapshot.moduleRef);
							Debug.Log("[MUM] make mod.snapshot persistant? " + mod.snapshot.moduleRef.Fields.Count);
							foreach (BaseField f in mod.snapshot.moduleRef.Fields)
							{
								Debug.Log("[MUM] moduleref " + f.name + " ispersistant = " + f.isPersistant);
							}
							}
							catch (Exception e)
							{
								Debug.LogError("[MUM]: can't acces mod.snapshot " + e);
							}

							Debug.Log("[MUM] try to get KSPField");
							Debug.Log("[MUM] try to get mod.GetType()" + mod.GetType());
							Debug.Log("[MUM] try to get mod.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)=" + mod.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));

							{
								FieldInfo[] infos = mod.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
								Debug.Log("[MUM] try to get infos:" + infos.Length);
								foreach (FieldInfo fi in infos)
								{
									Debug.Log("KSPField search for FIELD  " + fi.Name);
									foreach (object attr in fi.GetCustomAttributes(typeof(KSPField), true))
									{
										Debug.Log("KSPField is Defined : " + attr);
										if (attr is KSPField)
										{
											KSPField kf = (KSPField)attr;
											Debug.Log("KSPField is persistant?  : " + kf.isPersistant);
											kf.isPersistant = true;
											Debug.Log("KSPField is persistant after?  : " + kf.isPersistant);
										}
									}
								}
							}
							{
								Debug.Log("[MUM] THIRD KSPFIELD");
								AttributeCollection ac = TypeDescriptor.GetAttributes(mod);

								Debug.Log("KSPField AttributeCollection " + ac.Count);
								foreach (Attribute att in ac)
								{
									Debug.Log("KSPField fetch " + att);
									//DataEntityAttribute  -- ur attribute class name
									if (att is KSPField)
									{
										KSPField da = att as KSPField;
										Debug.Log("KSPField is persitant : " + da.isPersistant);  //initially it shows MESSAGE_STAGING
										da.isPersistant = true;
									}
								}


								//Check the changed value
								AttributeCollection acc = TypeDescriptor.GetAttributes(mod);

								foreach (var att in ac)
								{
									if (att is KSPField)
									{
										KSPField da = att as KSPField;
										Debug.Log("KSPField is verify : " + da.isPersistant); //now it shows Test_Message_Staging
									}
								}
							}
							{
								Debug.Log("[MUM] SECOND KSPFIELD");
								FieldInfo[] infos = mod.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
								Debug.Log("[MUM] try to get infos:" + infos.Length);
								foreach (FieldInfo fi in infos)
								{
									Debug.Log("KSPField search for FIELD  " + fi.Name);
									foreach (object attr in fi.GetCustomAttributes(typeof(KSPField), true))
									{
										Debug.Log("KSPField is Defined : " + attr);
										if (attr is KSPField)
										{
											KSPField kf = (KSPField)attr;
											Debug.Log("KSPField is persistant?  : " + kf.isPersistant);
											kf.isPersistant = true;
											Debug.Log("KSPField is persistant after?  : " + kf.isPersistant);
										}
									}
								}
							}
						}

						//get the node in the initialNode
						foreach (ConfigNode intialModuleNode in initialNode.GetNodes("MODULE"))
						{
							Debug.Log("[MUM] search module " + intialModuleNode.GetValue("name"));
							if (intialModuleNode.GetValue("name") == config.GetValue("name"))
							{
								Debug.Log("[MUM] search find!!! ");
								mod.Load(intialModuleNode);
								Debug.Log("[MUM] load done ");
							}
						}
						Debug.Log("[MUM] after (info) : " + mod.GetInfo());
						for (int i = 0; i < part.partInfo.moduleInfos.Count; i++)
						{
							AvailablePart.ModuleInfo info = part.partInfo.moduleInfos[i];
							Debug.Log("[MUM] searc info : " + info.moduleName + " =?= " + mod.moduleName.Replace("Module", ""));
							if (info.moduleName.Equals(mod.moduleName.Replace("Module", "")))
							{
								info.info = mod.GetInfo();
								Debug.Log("[MUM] replace!");
							}
						}
					}
					catch (Exception e)
					{
						Debug.Log("[MUM] can't MODIFY " + p.Modules.Count+ " : "+e);
					}
				}
				else if (type.Equals("delete"))
				{
					try
					{
						//do not add if already here
						if (!p.Modules.Contains(config.GetValue("name")))
						{
							Debug.Log("[MUM] create (from restore of delete) : " + type);
							//thanks to ialdabaoth for this
							PartModule module = p.AddModule(config.GetValue("name"));
							if (Awaken(module))
							{ // uses reflection to find and call the PartModule.Awake() private method
								//get from initialnode
								foreach (ConfigNode initModulenode in initialNode.GetNodes("MODULE"))
								{
									if (initModulenode.GetValue("name") == config.GetValue("name"))
									{
										module.Load(initModulenode);
										break;
									}
								}
							}
							//swap all module to be at the right place
							// Totally useless. But i do it. In case of a mod use the index.
							PartModule previousModule = module;
							PartModule nextModule = null;
							for (int i = 0; i < p.Modules.Count; i++)
							{
								nextModule = p.Modules[i];
								p.Modules[i] = previousModule;
								previousModule = nextModule;
							}

							//add info
							if (module.GetInfo().Length > 0)
							{
								//create
								AvailablePart.ModuleInfo info = new AvailablePart.ModuleInfo();
								info.moduleName = module.moduleName.Replace("Module", "");
								info.info = module.GetInfo();
								//add
								p.partInfo.moduleInfos.Add(info);
								//sort
								p.partInfo.moduleInfos.Sort((o1, o2) => o1.moduleName.CompareTo(o2.moduleName));

							}
						}
						else
						{
							Debug.Log("[MUM] not create from restore delete debause already here");
						}
					}
					catch (Exception e)
					{
						Debug.Log("[MUM] can't ADD (from restore of delete) " + e);
					}
				}
				else if (type.Equals("create"))
				{
					try
					{
						//do not del if not here
						if (p.Modules.Contains(config.GetValue("name")))
						{
							Debug.Log("[MUM] delete : " + type);
							PartModule mod = p.Modules[config.GetValue("name")];
							p.RemoveModule(p.Modules[config.GetValue("name")]);
							Debug.Log("[MUM] delete now remove info ? : " + mod.GetInfo());
							if (mod.GetInfo().Length > 0)
							{
								List<AvailablePart.ModuleInfo> allInfos = new List<AvailablePart.ModuleInfo>();
								allInfos.AddRange(part.partInfo.moduleInfos);
								foreach (AvailablePart.ModuleInfo infoToDel in allInfos)
								{
									if (infoToDel.moduleName.Equals(mod.moduleName.Replace("Module", "")))
									{
										part.partInfo.moduleInfos.Remove(infoToDel);
										break;
									}
								}
							}
						}
						else
						{
							Debug.Log("[MUM] do not del (retore create) because it's not inside");
						}
					}
					catch (Exception e)
					{
						Debug.Log("[MUM] can't REMOVE (from restore of create) " + p.Modules.Count);
					}
				}
			}
		}

		public override void OnLoad(ConfigNode root)
		{
			Debug.Log("[MUM] load : " + root);
			configModule = root.GetNodes("MODULE");
			Debug.Log("[MUM] extract : " + configModule.Length);
		}


		//needed? i didn't think
		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
		}

		//thanks to ialdabaoth for this
		//it's calling the Awake() method of a PartModule
		public static bool Awaken(PartModule module)
		{
			// thanks to Mu and Kine for help with this bit of Dark Magic. 
			// KINEMORTOBESTMORTOLOLOLOL
			if (module == null)
				return false;
			object[] paramList = new object[] { };
			MethodInfo awakeMethod = typeof(PartModule).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
			if (awakeMethod == null)
				return false;

			awakeMethod.Invoke(module, paramList);
			return true;
		}

	}
}
