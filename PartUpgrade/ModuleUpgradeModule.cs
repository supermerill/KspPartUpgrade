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

		[KSPField(isPersistant = true)]
		public string type = "replace";

		[KSPField]
		public string tech;

		[KSPField(isPersistant = true)]
		public bool persistant = true;

		[KSPField(isPersistant = true)]
		public bool persited = false;

		public ConfigNode[] configModule;

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
						removeModule(p, p.Modules[config.GetValue("name")]);

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
				Debug.Log("[MUM] restore a module  " + config.GetValue("name") + " " + p.partInfo.moduleInfos.Count + "/ "
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
								Debug.Log("[MUM] set " + f.name + " ispersistant = " + f.isPersistant);
								//get the KSPField f the object
								//f.host.
								//Part p;
								//IConfigNode icn;
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
						Debug.Log("[MUM] can't MODIFY " + p.Modules.Count + " : " + e);
					}
				}
				else if (type.Equals("delete"))
				{
					try
					{
						//do not add if already here
						if (!p.Modules.Contains(config.GetValue("name")))
						{

							PartModule module = createModule(p, config);
							//TODO: test if null.

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
							removeModule(p, p.Modules[config.GetValue("name")]);
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

		public override void OnLoad(ConfigNode root)
		{
			Debug.Log("[MUM] load : " + root + " for " + configModule);
			if (configModule == null)
			{
				configModule = root.GetNodes("MODULE");
				//is not a partprefab?
				if (persistant && persited)
				{
					//put data from config node to modules
					try
					{
						Debug.Log("[MUM] on load ");
						//save the config nodes
						int numMod = -1;
						foreach (ConfigNode config in configModule)
						{
							numMod++;
							Debug.Log("[MUM] on load in flight  module name to save: " + config.GetValue("name")
								+ " => " + config.HasData + " " + config.CountValues);
							if (config.CountValues > 1)
							{
								//more than "name"
								PartModule mod = part.Modules[config.GetValue("name")];

								//need to del a new one?
								string isDel = config.GetValue("del");
								if (isDel != null && bool.Parse(isDel) && mod != null)
								{
									//this module mustn't be here
									removeModule(part, mod);
								}

								//need to recreate an old one?
								if (mod == null)
								{
									try
									{
										Debug.Log("[MUM] recreate a module from onload : " + config.GetValue("name"));
										Debug.Log("[MUM] my config : " + config);
										Debug.Log("[MUM] my part prefab : " + part.partInfo.partPrefab);
										//TODO: more robust
										foreach (ModuleUpgradeModule infoMod in part.partInfo.partPrefab.Modules.GetModules<ModuleUpgradeModule>())
										{
										Debug.Log("[MUM] search my mum : " + infoMod.moduleName+" "+infoMod.configModule
											+ ",,;;,, " + infoMod.configModule);
										}
										Debug.Log("[MUM] my part prefab module info : " + part.partInfo.partPrefab.Modules[part.Modules.IndexOf(this)]);
										Debug.Log("[MUM] my part prefab module info conf mod : "
											+ ((ModuleUpgradeModule)part.partInfo.partPrefab.Modules[part.Modules.IndexOf(this)]).configModule.Length);

										Debug.Log("[MUM] my part prefab conf : " + part.partInfo.partConfig);
										mod = createModule(part,
											((ModuleUpgradeModule)part.partInfo.partPrefab.Modules[part.Modules.IndexOf(this)]).configModule[numMod]);
										Debug.Log("[MUM] readd! : " + mod.moduleName);
									}
									catch (Exception e)
									{
										Debug.LogError("[MUM] fail to recreate a module (onload): " + e);
									}
								}

								try
								{
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
			Debug.Log("[MUM] extract : " + configModule.Length);
			// change external modle to their persisted data

		}

		public override void OnSave(ConfigNode node)
		{
			try
			{
				Debug.Log("[MUM] on save ");
				base.OnSave(node);
				//if (this.configModule == null)
				{
					PartModule pm2 = part.partInfo.partPrefab.Modules[part.Modules.IndexOf(this)];
					ModuleUpgradeModule mum = pm2 as ModuleUpgradeModule;
					Debug.Log("[MUM] retreive partmodule from partinfo : " + mum.configModule);

					this.configModule = mum.configModule;
					persited = true;
				}
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
										if (field.IsDefined(typeof(KSPField), true))
										{
											//TODO: save only value that can be retreived
											Debug.Log("[MUM] Onsave: " + field.Name + " => " + field.GetValue(mod));
											moduleNode.AddValue(field.Name, field.GetValue(mod));
										}
									}
									catch (Exception e)
									{
										Debug.LogError("error when aprsing fields : " + e);
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

		public static void removeModule(Part part, PartModule modToRemove)
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

		public static PartModule createModule(Part part, ConfigNode nodeToCreate)
		{
			string moduleName = nodeToCreate.GetValue("name");
			if (moduleName == null || part.Modules[moduleName] == null) return null;

			PartModule newMod = part.AddModule(moduleName);
			if (Awaken(newMod))
			{ // uses reflection to find and call the PartModule.Awake() private method
				newMod.Load(nodeToCreate);
			}

			//add info
			if (newMod.GetInfo().Length > 0)
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

	}
}
