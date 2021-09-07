public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}
public void Main(string argument, UpdateType updateSource)
{
    //all names in the script are general but can be renamed for every space arm connected to the programmable block
    //if any of the blocks are replaced you need to recompile the script to work properly

    //defining and finding the space arm cockpit
    IMyCockpit cockpit;
    cockpit = null;
    cockpit = GridTerminalSystem.GetBlockWithName("Cockpit Space Arm") as IMyCockpit;

    //making sure the space arm does not move the ship
    cockpit.ControlThrusters = false;
    cockpit.ControlWheels = false;
    cockpit.IsMainCockpit = false;
    //cockpit.ControlGyros = false; cannot find a way to auto set the gyro control to false

    //defining all the required pistons
    IMyPistonBase forwardPiston;
    forwardPiston = null;
    IMyPistonBase sidePiston;
    sidePiston = null;
    IMyPistonBase verticalPiston;
    verticalPiston = null;

    //finding the forward moving piston and assigning it to follow the movement from the cockpit
    forwardPiston = GridTerminalSystem.GetBlockWithName("Piston forward") as IMyPistonBase;
    forwardPiston.Velocity = cockpit.MoveIndicator.Z * -1;
    //finding the side moving piston and assigning it to follow the side movement from the cockpit
    sidePiston = GridTerminalSystem.GetBlockWithName("Piston side") as IMyPistonBase;
    sidePiston.Velocity = cockpit.MoveIndicator.X;
    //finding the vertical moving piston and assigning it to follow the vertical movement from the cockpit
    verticalPiston = GridTerminalSystem.GetBlockWithName("Piston vertical") as IMyPistonBase;
    verticalPiston.Velocity = cockpit.MoveIndicator.Y;
}
