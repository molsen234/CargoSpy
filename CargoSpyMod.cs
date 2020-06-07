using ICities;

namespace CargoSpy
{
    // Considerations:
    // - How to deal with trains, planes, cargo stations/airports. Trains can carry multiple resource types. Planes maybe too?
    // - UIMgr is base class, I never make an instance. Should I?
    // - Can  track statistics for buildings? In and out over a period? Without patching game?
    // - What happens when a cargo vehicle despawns? Can I track and report this?
    // - Some trucks seem to visit multiple destinations, how does that work? Can I show this?
    public class CargoSpyMod : IUserMod

    {
        // This allows the mod to be displayed in the C:S Mod Content Manager.
        public string Name
        {
            get
            { 
                return "Cargo Spy v0.02";
            }
        }
        public string Description
        {
            get
            {
                return "Displays a live list of cargo vehicles, per cargo type.";
            }
        }
    }
    public class CargoSpyLoading : LoadingExtensionBase
    {
        private LoadMode _mode;
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame && mode != LoadMode.NewGameFromScenario) return;
            _mode = mode;  // Save mode, as it is not passed to OnLevelUnLoading() below
            // Now set up your stuff - possibly from cofig file
            //TruckPanel.Initialize();
            //TruckPanel.Instance.myRefreshInterval = 3f;
            //TruckPanel.Instance.UpdateTruckTable();
        }
        public override void OnLevelUnloading()
        {
            if (_mode != LoadMode.NewGame && _mode != LoadMode.LoadGame && _mode != LoadMode.NewGameFromScenario) return;
            //TruckPanel.Instance.DestroyAll();
            // Now tear down your stuff
        }
    }
}