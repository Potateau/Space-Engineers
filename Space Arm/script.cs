public Program()
{
    //runtime needs to be as frequent as possible else delay in inputs
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}


public void Main(string argument, UpdateType updateSource)
{
    //Vertical Axis => Y
    //Forward Axis => Z
    //Side Axis => X

    //All COCKPITS
    List<IMyCockpit> allCockpits = new List<IMyCockpit>();
    allCockpits.Clear();
    allCockpits = CreateCockpitList();

    //ALL PISTONS
    List<IMyPistonBase> allPistonBases = new List<IMyPistonBase>();
    allPistonBases.Clear();
    allPistonBases = CreatePistonBaseList();

    //ALL HINGES AND ROTORS
    List<IMyMotorStator> allHingesAndRotors = new List<IMyMotorStator>();
    allHingesAndRotors.Clear();
    allHingesAndRotors = CreateHingeAndRotorList();

    //for all cockpits
    for(int i=0; i < allCockpits.Count; i++)
    {
        //To start the custom data of the cockpits will be used define each space arm and the pistons associated with it
        //if the cockpit custom data declares it is for a spacearm and name is not just "spacearm"
        //This is done as the pistons, hinges, and rotors connect to the cockpit by checking if their custom data contains the custom data of the cockpit.  If the cockpit custom data was just "spacearm" it would controll all pistons setup as spacearms
        if (allCockpits[i].CustomData.ToLower().Replace(" ", "").Contains("spacearm")&& allCockpits[i].CustomData.Replace(" ","").Length>8)
        {
            //The cockpit custom data indicates it should be used as a space arm so disable the cockpit's ability to control the ship as much as possible
            DisableCockpitFromControllingShip(allCockpits[i]);
            allCockpits[i].CustomName = "Cockpit - " + allCockpits[i].CustomData;
            //allCockpits[i].SetCustomName("Cockpit - " + allCockpits[i].CustomData);

            //for all pistons
            for (int j = 0; j < allPistonBases.Count; j++)
            {
                //checking all pistons to see if they are meant to be connected to the cockpit to act as a space arm
                if (allPistonBases[j].CustomData.ToLower().Replace(" ", "").Contains(allCockpits[i].CustomData.ToLower().Replace(" ","")))
                {
                    allPistonBases[j].CustomName = "Piston - " + allCockpits[i].CustomData;
                    //piston is associated with the cockpit so have the piston controlled be user movement inputs to the cockpit
                    MovePistons(allCockpits[i],allPistonBases[j]);
                }
            }

            //for all hinges and rotors   
            for (int k = 0; k < allHingesAndRotors.Count; k++)
            {
                //checking all hinges to see if they are meant to be connected to the cockpit to act as a space arm.  Technically rotors could be included
                if (allHingesAndRotors[k].CustomData.ToLower().Replace(" ","").Contains(allCockpits[i].CustomData.ToLower().Replace(" ","")))
                {
                    allHingesAndRotors[k].CustomName = "Rotor - " + allCockpits[i].CustomData;
                    MoveHingesAndRotors(allCockpits[i], allHingesAndRotors[k]);
                }
            }
        }
    }
}

public List<IMyCockpit> CreateCockpitList()
{
    //creating the local variable
    List<IMyCockpit> allCockpits_InFunction = new List<IMyCockpit>();
    //getting all cockpits in all grids connected to the programmable block
    GridTerminalSystem.GetBlocksOfType<IMyCockpit>(allCockpits_InFunction);
    return allCockpits_InFunction;
}
public List<IMyPistonBase> CreatePistonBaseList()
{
    List<IMyPistonBase> allPistonBases_InFunction = new List<IMyPistonBase>();
    GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(allPistonBases_InFunction);
    return allPistonBases_InFunction;
}
public List<IMyMotorStator> CreateHingeAndRotorList()
{
    List<IMyMotorStator> allHingesAndRotors_InFunction = new List<IMyMotorStator>();
    GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(allHingesAndRotors_InFunction);
    return allHingesAndRotors_InFunction;
}
public void DisableCockpitFromControllingShip(IMyCockpit cockpit_InFunction)
{
    cockpit_InFunction.ControlThrusters = false;
    cockpit_InFunction.ControlWheels = false;
    cockpit_InFunction.IsMainCockpit = false;
}
public void MovePistons(IMyCockpit cockpit_InFunction,IMyPistonBase pistonBase_InFunction)
{
    //Vertical Axis => Y
    //Longitudinal Axis => Z
    //Lateral Axis => X

    //checking declared orientation of pistons and assigning movement direction based on orientation with the cockpit
    if (pistonBase_InFunction.CustomData.ToLower().Contains("vertical"))
    {
        if (pistonBase_InFunction.CustomData.Contains("-1"))
        {
            pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.Y*-1;
        }
        else
        {
            pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.Y;
        }
    }
    else if (pistonBase_InFunction.CustomData.ToLower().Contains("longitudinal"))
    {
        if (pistonBase_InFunction.CustomData.Contains("-1"))
        {
            pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.Z;
        }
        else
        {
            pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.Z * -1;
        }
    }
    else if (pistonBase_InFunction.CustomData.ToLower().Contains("lateral"))
    {
        if (pistonBase_InFunction.CustomData.Contains("-1"))
        {
            pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.X*-1;
        }
        else
        {
            pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.X;
        }
    }            
}
public void MoveHingesAndRotors(IMyCockpit cockpit_InFunction, IMyMotorStator motorStator_InFunction)
{
    //Pitch => X
    //Yaw => Y
    if (motorStator_InFunction.CustomData.ToLower().Contains("pitch"))
    {
        if (motorStator_InFunction.CustomData.Contains("-1"))
        {
            motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RotationIndicator.X * -1 / 2;
        }
        else
        {
            motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RotationIndicator.X / 2;
        }
    }
    else if (motorStator_InFunction.CustomData.ToLower().Contains("yaw"))
    {
        if (motorStator_InFunction.CustomData.Contains("-1"))
        {
            motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RotationIndicator.Y * -1 / 2;
        }
        else
        {
            motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RotationIndicator.Y / 2;
        }
    }
    else if (motorStator_InFunction.CustomData.ToLower().Contains("roll"))
    {
        if (motorStator_InFunction.CustomData.Contains("-1"))
        {
            motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RollIndicator * -1 * 2;
        }
        else
        {
            motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RollIndicator * 2;
        }
    }
}
