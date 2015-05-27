# PartUpgrade
This project is about upgrading parts when researching tech nodes in kerbal space program.
## how it work
PartUpgrade work with PartModule.
If you want to make a part upgradable, you add the desired partModule to it.

When your game is loading (or you research a new tech node) this program check if it need to make an upgrade pass. When upgrading a part, it reset the part prefab to its initial state and call each Upgrade-PartModule to apply the right upgrade.

Each instance of the part already in flight is not reset/upgraded. Its old data are persisted in its config node and loaded when the vessel is loaded.

## Making configurations with PartUpgrade
Go to the wiki. There are examples of ModuleManager configuration to add Upgrades to your parts.

## Making new PartModule for PartUpgrade
You can look at ModueUpgradeMass for a simple module that update a part field.  
You can look at ModuleUpgradeEngine to a more complex module that update a specific module inside a part.  

When subclassing PartUpgrade, your module is called when an " upgrade pass occur". It's a call to restore with the part config node, and after a call to upgrade with all the current researched technologies.  
If you want to not upgrade all the flying part at the same time, you should define also the onload/onsave method to persist/load the not-upgraded data (unless it's in the editor).
#### License
Source code: GPL
The released dll is distributed as CC BY-NC 4.0


by Merill
