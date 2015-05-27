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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SpaceRace
{
	class ModuleUpgradeModule : ModuleUpgrade
	{

		public static HashSet<Type> persistedTypes = new HashSet<Type>(new Type[]{
			typeof(bool),typeof(byte),typeof(short),typeof(ushort),typeof(int),typeof(uint),
			typeof(long),typeof(ulong),typeof(float),typeof(double),typeof(string),
			typeof(Color),typeof(Color32),
			typeof(Matrix4x4),typeof(Quaternion),typeof(QuaternionD),
			typeof(Vector3),typeof(Vector3d),typeof(Vector4)
		});

		[KSPField(isPersistant = true)]
		public string type = "replace";

		[KSPField(isPersistant = true)]
		public string tech;

		//[KSPField(isPersistant = true)]
		//public bool persistant = true;

		//this module is in a persisted vessel? (in flight?)
		//true if the MODULE in confignode contains persistant data
		//flse if the MODULE in confignode contains data for initialisation
		[KSPField(isPersistant = true)]
		public bool persisted = false;

		//An id field to create to not make me crazy.
		[KSPField(isPersistant = true)]
		public int id = 0;

		public ConfigNode[] configModule;

		//called only in the prefab
		public override void upgrade(List<string> allTechName)
		{
			if (moduleName == null || moduleName.Length == 0 || tech == null || !allTechName.Contains(tech)) return;
			Part p = partToUpdate();

			foreach (ConfigNode config in configModule)
			{
				Debug.Log("[MUM] upgrade a module  " + p.name + " , " + p.partInfo.moduleInfos.Count + "/ " + p.Modules.Count + " , " + type);
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
						createModule(p, config);
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
						removeModuleAndInfo(p, p.Modules[config.GetValue("name")]);

					}
					catch (Exception e)
					{
						Debug.Log("[MUM] can't REMOVE " + e);
					}
				}
				Debug.Log("[MUM] relance ");
			}
		}

		//called only in the prefab
		public override void restore(ConfigNode initialNode)
		{

			Part p = partToUpdate();

			Debug.Log("[MUM] restore '" + configModule + "' for initialNode node :" + initialNode);
			foreach (ConfigNode config in configModule)
			{
				string moduleUpgradeName = config.GetValue("name");
				//Debug.Log("[MUM] restore a module  " + config.GetValue("name") + " " + p.partInfo.moduleInfos.Count + "/ "
				//	+ p.Modules.Count + " , " + type + " persisted=" + persisted);
				if (type.Equals("replace"))
				{
					try
					{
						//get the module
						//ConfigNode node = new ConfigNode();
						PartModule mod = p.Modules[moduleUpgradeName];
						//make all fields persistant => doesn't work
						//Debug.Log("[MUM] make all fields persistant " + mod.Fields.Count);
						////if (persistant)
						//{
						//	foreach (BaseField f in mod.Fields)
						//	{
						//		Debug.Log("[MUM] before " + f.name + " ispersistant = " + f.isPersistant);
						//		//do NOTHING
						//		f.isPersistant = true;
						//		Debug.Log("[MUM] set " + f.name + " ispersistant = " + f.isPersistant);
						//		//get the KSPField f the object
						//		//f.host.
						//		//Part p;
						//		//IConfigNode icn;
						//	}
						//}

						//get the node in the initialNode
						foreach (ConfigNode intialModuleNode in initialNode.GetNodes("MODULE"))
						{
							Debug.Log("[MUM] search module " + intialModuleNode.GetValue("name"));
							if (intialModuleNode.GetValue("name") == moduleUpgradeName)
							{
								Debug.Log("[MUM] restore: replace values in " + moduleUpgradeName);
								mod.Load(intialModuleNode);
								//Debug.Log("[MUM] load done ");
							}
						}
						//Debug.Log("[MUM] after (info) : " + mod.GetInfo());
						for (int i = 0; i < part.partInfo.moduleInfos.Count; i++)
						{
							AvailablePart.ModuleInfo info = part.partInfo.moduleInfos[i];
							//Debug.Log("[MUM] searc info : " + info.moduleName + " =?= " + mod.moduleName.Replace("Module", ""));
							if (info.moduleName.Equals(mod.moduleName.Replace("Module", "")))
							{
								info.info = mod.GetInfo();
							}
						}
					}
					catch (Exception e)
					{
						Debug.Log("[MUM] can't MODIFY " + p.Modules.Count + " : " + e);
					}
				}
				else if (type.Equals("delete"))
				{
					try
					{
						//do not add if already here
						if (!p.Modules.Contains(moduleUpgradeName))
						{

							Debug.Log("[MUM] restore : delete " + moduleUpgradeName);
							PartModule module = createModule(p, config);
							//TODO: test if null.
							if (module == null)
							{
								Debug.LogError("[MUM] Error : can't recreate the partmodule " + moduleUpgradeName);
							}

							//DONT DO THAT \/, it mess the name/index map of Parmodulelist
							//swap all module to be at the right place
							// Totally useless. But i do it. In case of a mod use the index.
							//PartModule previousModule = module;
							//PartModule nextModule = null;
							//for (int i = 0; i < p.Modules.Count; i++)
							//{
							//	nextModule = p.Modules[i];
							//	p.Modules[i] = previousModule;
							//	previousModule = nextModule;
							//}

						}
						else
						{
							Debug.Log("[MUM] not create from restore delete because module already here");
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
						if (p.Modules.Contains(moduleUpgradeName))
						{
							Debug.Log("[MUM] delete : " + type);
							removeModuleAndInfo(p, p.Modules[moduleUpgradeName]);
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
			foreach (PartModule pm in part.partInfo.partPrefab.Modules)
			{
				if (pm == this)
				{
					Debug.Log("[MUM] RESTORE: FIND ME! ");
				}
			}
		}

		//called in the prefab & in the part clones
		public override void OnLoad(ConfigNode root)
		{
			base.OnLoad(root);
			//Debug.Log("[MUM] load : " + root + " for " + configModule + " in editor? " + HighLogic.LoadedSceneIsEditor);
			//don't load if in editor: it's ok to maj
			if (HighLogic.LoadedSceneIsEditor) return;
			if (configModule == null)
			{
				Debug.Log("[MUM] load configModule is null");
				//is a partprefab?
				if (!persisted)
				{
					Debug.Log("[MUM] load prefab");
					configModule = root.GetNodes("MODULE");
				}
				else
				{
					Debug.Log("[MUM] load configModule is persisted");
					//put data from config node to modules
					try
					{
						configModule = root.GetNodes("MODULE");
						//save the config nodes
						int numMod = -1;
						foreach (ConfigNode config in configModule)
						{
							string moduleUpgradeName = config.GetValue("name");
							numMod++;
							Debug.Log("[MUM] on load in flight  module name to save: " + moduleUpgradeName
								+ " => " + config.HasData + " " + config.CountValues);
							if (config.CountValues > 1)
							{
								//more than "name"

								//need to del a new one?
								string isDel = config.GetValue("del");
								if (isDel != null && bool.Parse(isDel))
								{
									Debug.Log("[MUM] Delete it");
									if (part.Modules.Contains(moduleUpgradeName))
									{
										//this module mustn't be here
										part.RemoveModule(part.Modules[moduleUpgradeName]);
									}
									else
									{
										Debug.LogError("[MUM] Error in loading a not-upgraded upgraded part: can't erase a module with name " +
											moduleUpgradeName + " as it's not in the current part.");
									}
									//i didn't need to load it, as it's deleted.
									continue;
								}

								//need to recreate an old one?
								if ( !part.Modules.Contains(moduleUpgradeName))
								{
									try
									{
										Debug.Log("[MUM] recreate a module from onload : " + moduleUpgradeName);
										Debug.Log("[MUM] search in prefab the init values : " + part.partInfo.partPrefab.Modules.Count);
										//go to part prefab configModule to get the module config
										foreach (ModuleUpgradeModule infoMod in part.partInfo.partPrefab.Modules.GetModules<ModuleUpgradeModule>())
										{
											//check if the same (tech & id)
											if (infoMod.tech == tech && infoMod.type == type && infoMod.id == id)
											{
												Debug.Log("[MUM] find the good moduleupgrade! : " + infoMod.id);
												//find the good node
												foreach (ConfigNode config2 in infoMod.configModule)
												{
													Debug.Log("[MUM] search a node : " + config2.GetValue("name"));
													if (config2.GetValue("name") == moduleUpgradeName)
													{
														//find! 
														//create the module (without the partinfo)
														Debug.Log("[MUM] find a good node!");
														PartModule mud = createModule(part, config2, false);
														Debug.Log("[MUM] find a good node! " + mud);
														Debug.Log("[MUM] find a good node! " + mud.moduleName);
														Debug.Log("[MUM] find a good node! " + moduleUpgradeName);
														Debug.Log("[MUM] find a good node!" + part.Modules.Contains(moduleUpgradeName));
														Debug.Log("[MUM] find a good node!" + part.Modules[moduleUpgradeName]);
														Debug.Log("[MUM] find a good node!" + part.Modules.IndexOf(mud));

														break;
													}
												}
											}
										}
										//the loading occur a bit below.
										Debug.Log("[MUM] readd! : " + moduleUpgradeName);
									}
									catch (Exception e)
									{
										Debug.LogError("[MUM] fail to recreate a module (onload): " + e);
									}
								}

								try
								{
									Debug.Log("[MUM] can load upgrade? : " + part.Modules.Contains(moduleUpgradeName));
									if (part.Modules.Contains(moduleUpgradeName))
									{
										//this module mustn't be here
										PartModule mod = part.Modules[moduleUpgradeName];
										foreach (FieldInfo field in mod.GetType().GetFields())
										{
											try
											{
												if (field.IsDefined(typeof(KSPField), true))
												{
													Debug.Log("[MUM] onload: " + field.FieldType.Name + " " + field.Name + " : "
														+ field.GetValue(mod) + " => " + config.GetValue(field.Name));
													if (field.FieldType == typeof(string))
													{
														field.SetValue(mod, config.GetValue(field.Name));
													}
													else if (field.FieldType == typeof(long) || field.FieldType == typeof(ulong)
														|| field.FieldType == typeof(UInt64))
													{
														field.SetValue(mod, long.Parse(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(int) || field.FieldType == typeof(uint)
														|| field.FieldType == typeof(UInt32))
													{
														field.SetValue(mod, int.Parse(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(short) || field.FieldType == typeof(ushort))
													{
														field.SetValue(mod, short.Parse(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(bool))
													{
														field.SetValue(mod, bool.Parse(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(byte))
													{
														field.SetValue(mod, byte.Parse(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(float))
													{
														field.SetValue(mod, float.Parse(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(double))
													{
														field.SetValue(mod, double.Parse(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(Color))
													{
														field.SetValue(mod, ConfigNode.ParseColor(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(Color))
													{
														field.SetValue(mod, ConfigNode.ParseColor(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(Color32))
													{
														field.SetValue(mod, ConfigNode.ParseColor32(config.GetValue(field.Name)));
													}
													//how?
													//else if (field.FieldType == typeof(Enum))
													//{
													//	field.SetValue(mod, ConfigNode.ParseEnum(config.GetValue(field.Name)));
													//}
													else if (field.FieldType == typeof(Matrix4x4))
													{
														field.SetValue(mod, ConfigNode.ParseMatrix4x4(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(Quaternion))
													{
														field.SetValue(mod, ConfigNode.ParseQuaternion(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(QuaternionD))
													{
														field.SetValue(mod, ConfigNode.ParseQuaternionD(config.GetValue(field.Name)));
													}
													//vector3 or 2?
													else if (field.FieldType == typeof(Vector3))
													{
														field.SetValue(mod, ConfigNode.ParseVector3(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(Vector3d))
													{
														field.SetValue(mod, ConfigNode.ParseVector3D(config.GetValue(field.Name)));
													}
													else if (field.FieldType == typeof(Vector4))
													{
														field.SetValue(mod, ConfigNode.ParseVector4(config.GetValue(field.Name)));
													}
												}
											}
											catch (Exception e)
											{
												Debug.Log("Warn: can't set the field " + field.Name + " : " + field.FieldType.Name
													+ " with node value of " + config.GetValue(field.Name));
											}
										}
									}
									else
									{
										Debug.LogError("[MUM] Error in loading an mostly upgraded part: can't find the module with name " 
											+ moduleUpgradeName);
									}

								}
								catch (Exception e)
								{
									Debug.LogError("error when load getting mod : " + e);
								}
							}
						}
					}
					catch (Exception e)
					{
						Debug.LogError("fail load!!! " + e);
					}
				}
			}
			//Debug.Log("[MUM] extract : " + (configModule == null ? "null" : "" + configModule.Length));
			// change external modle to their persisted data

		}

		//called in the part clones (a partprefab is never "saved")
		public override void OnSave(ConfigNode node)
		{
			try
			{
				Debug.Log("[MUM] on save " + id + " => " + this.configModule);
				base.OnSave(node);
				//no persisted data yet?
				if (this.configModule == null)
				{
					//get perssited data from partprefab
					//PartModule pm2 = part.partInfo.partPrefab.Modules[part.Modules.IndexOf(this)];
					ModuleUpgradeModule mum = null;
					foreach (ModuleUpgradeModule infoMod in part.partInfo.partPrefab.Modules.GetModules<ModuleUpgradeModule>())
					{
						if (infoMod.tech == tech && infoMod.type == type && infoMod.id == id)
						{
							mum = infoMod;
							break;
						}
					}
					if (mum == null)
					{
						Debug.LogError("[MUM] can't find partmodule from info.");
						return;
					}
					Debug.Log("[MUM] retreive partmodule from partinfo : " + mum.configModule);

					this.configModule = mum.configModule;
					persisted = true;
				}
				Debug.Log("[MUM] on save confignode? " + id + " => " + this.configModule);
				//save the config nodes
				foreach (ConfigNode config in configModule)
				{
					Debug.Log("[MUM] on save in flight  module name to save: " + config.GetValue("name"));
					{
						try
						{
							PartModule mod = part.Modules[config.GetValue("name")];
							ConfigNode moduleNode = node.AddNode("MODULE");
							if (mod == null)
							{
								//create, i think, so no data!
								moduleNode.AddValue("name", config.GetValue("name"));
								moduleNode.AddValue("del", true);
							}
							else
							{
								moduleNode.AddValue("name", mod.moduleName);
								foreach (FieldInfo field in mod.GetType().GetFields())
								{
									try
									{
										if (field.IsDefined(typeof(KSPField), true) && persistedTypes.Contains(field.FieldType))
										{
											//TODO: save only value that can be retreived
											Debug.Log("[MUM] Onsave: " + field.Name + " => " + field.GetValue(mod));

											moduleNode.AddValue(field.Name, field.GetValue(mod));
										}
									}
									catch (Exception e)
									{
										Debug.LogWarning("[MUM] can't persist field : " + field.Name);
									}
								}
							}
						}
						catch (Exception e)
						{
							Debug.LogError("error when getting mod : " + e);
						}
					}
				}
				Debug.Log("[MUM] Onsave saved : " + configModule);
			}
			catch (Exception e)
			{
				Debug.LogError("fail save!!! " + e);
			}
		}

		//to call only on partprefab, as it erase the partinfo
		public static void removeModuleAndInfo(Part part, PartModule modToRemove)
		{
			if (modToRemove == null) return;
			part.RemoveModule(modToRemove);
			if (modToRemove.GetInfo().Length > 0)
			{
				List<AvailablePart.ModuleInfo> allInfos = new List<AvailablePart.ModuleInfo>();
				allInfos.AddRange(part.partInfo.moduleInfos);
				foreach (AvailablePart.ModuleInfo infoToDel in allInfos)
				{
					if (infoToDel.moduleName.Equals(modToRemove.moduleName.Replace("Module", "")))
					{
						part.partInfo.moduleInfos.Remove(infoToDel);
						break;
					}
				}
			}
		}

		//to call only on partprefab, as it create the partinfo
		public static PartModule createModule(Part part, ConfigNode nodeToCreate, bool withModuleInfo = true)
		{
			string moduleName = nodeToCreate.GetValue("name");
			if (moduleName == null) return null;

			PartModule newMod = part.AddModule(moduleName);
			if (Awaken(newMod))
			{ // uses reflection to find and call the PartModule.Awake() private method
				newMod.Load(nodeToCreate);

				//add info
				if (withModuleInfo && newMod.GetInfo().Length > 0)
				{
					//create
					AvailablePart.ModuleInfo info = new AvailablePart.ModuleInfo();
					info.moduleName = newMod.moduleName.Replace("Module", "");
					info.info = newMod.GetInfo();
					//add
					part.partInfo.moduleInfos.Add(info);
					//sort info
					part.partInfo.moduleInfos.Sort((o1, o2) => o1.moduleName.CompareTo(o2.moduleName));
				}
				return newMod;
			}
			else
			{
				Debug.Log("create a module done & awake KO ");
				return null;
			}
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
