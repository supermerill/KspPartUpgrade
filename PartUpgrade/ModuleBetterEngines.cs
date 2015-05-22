using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
	//don't work
	class ModuleBetterEngines : ModuleEngines
	{
		public void updateThrust()
		{
			base.ThrustUpdate();
		}

	}
}
