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

    RunSpaceArms(allCockpits, allPistonBases, allHingesAndRotors);

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
public void RunSpaceArms(List<IMyCockpit> allCockpits_InFunction, List<IMyPistonBase> allPistonBases_InFunction, List<IMyMotorStator> allHingesAndRotors_InFunction)
{
    //for all cockpits
    for (int i = 0; i < allCockpits_InFunction.Count; i++)
    {
        //To start the custom data of the cockpits will be used define each space arm and the pistons associated with it
        //if the cockpit custom data declares it is for a spacearm and name is not just "spacearm"
        //This is done as the pistons, hinges, and rotors connect to the cockpit by checking if their custom data contains the custom data of the cockpit.  If the cockpit custom data was just "spacearm" it would controll all pistons setup as spacearms
        if (allCockpits_InFunction[i].CustomData.ToLower().Replace(" ", "").Contains("spacearm") && allCockpits_InFunction[i].CustomData.Replace(" ", "").Length > 8)
        {
            //The cockpit custom data indicates it should be used as a space arm so disable the cockpit's ability to control the ship as much as possible
            DisableCockpitFromControllingShip(allCockpits_InFunction[i]);

            //checking display name of cockpit
            if (allCockpits_InFunction[i].CustomName.Contains(allCockpits_InFunction[i].CustomData))
            {
                //do nothing
            }
            //add space arm name to display name
            else
            {
                allCockpits_InFunction[i].CustomName = allCockpits_InFunction[i].CustomName + allCockpits_InFunction[i].CustomData;
            }

            //for all pistons
            for (int j = 0; j < allPistonBases_InFunction.Count; j++)
            {
                //checking all pistons to see if they are meant to be connected to the cockpit to act as a space arm
                if (allPistonBases_InFunction[j].CustomData.ToLower().Replace(" ", "").Contains(allCockpits_InFunction[i].CustomData.ToLower().Replace(" ", "")))
                {
                    //checking display name of piston
                    if (allPistonBases_InFunction[j].CustomName.Contains(allCockpits_InFunction[i].CustomData))
                    {
                        //do nothing
                    }
                    //add space arm name to display name
                    else
                    {
                        allPistonBases_InFunction[j].CustomName = allPistonBases_InFunction[j].CustomName + allCockpits_InFunction[i].CustomData;
                    }
                    //piston is associated with the cockpit so have the piston controlled be user movement inputs to the cockpit
                    MovePistons(allCockpits_InFunction[i], allPistonBases_InFunction[j]);
                }
            }
            //for all hinges and rotors   
            for (int k = 0; k < allHingesAndRotors_InFunction.Count; k++)
            {
                //checking all hinges to see if they are meant to be connected to the cockpit to act as a space arm.  Technically rotors could be included
                if (allHingesAndRotors_InFunction[k].CustomData.ToLower().Replace(" ", "").Contains(allCockpits_InFunction[i].CustomData.ToLower().Replace(" ", "")))
                {
                    //checking display name of piston
                    if (allHingesAndRotors_InFunction[k].CustomName.Contains(allCockpits_InFunction[i].CustomData))
                    {
                        //do nothing
                    }
                    //add space arm name to display name
                    else
                    {
                        allHingesAndRotors_InFunction[k].CustomName = allHingesAndRotors_InFunction[k].CustomName + allCockpits_InFunction[i].CustomData;
                    }
                    //hinge or rotor is associated with the cockpit so have the piston controlled be user movement inputs to the cockpit
                    MoveHingesAndRotors(allCockpits_InFunction[i], allHingesAndRotors_InFunction[k]);
                }
            }
        }
    }
}
public void DisableCockpitFromControllingShip(IMyCockpit cockpit_InFunction)
{
    cockpit_InFunction.ControlThrusters = false;
    cockpit_InFunction.ControlWheels = false;
    cockpit_InFunction.IsMainCockpit = false;
    cockpit_InFunction.SetValueBool("ControlGyros", false);
}
public void MovePistons(IMyCockpit cockpit_InFunction, IMyPistonBase pistonBase_InFunction)
{
    //Vertical Axis => Y
    //Longitudinal Axis => Z
    //Lateral Axis => X

    //getting the input arguement to the programmable block so players can custom set the sensitivity 
    string terminalRunArguement = "";
    float terminalRunArguementNumber = 1;
    terminalRunArguement = Me.TerminalRunArgument;

    bool validSensitivity = float.TryParse(terminalRunArguement, out terminalRunArguementNumber);

    //the programmable block contains a valid sensitivity input in the terminal run arguement so adjust the sensitivity accordingly
    if(validSensitivity)
    {
        //checking declared orientation of pistons and assigning movement direction based on orientation with the cockpit
        if (pistonBase_InFunction.CustomData.ToLower().Contains("vertical"))
        {
            if (pistonBase_InFunction.CustomData.Contains("-1"))
            {
                pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.Y * -1 * terminalRunArguementNumber;
            }
            else
            {
                pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.Y * terminalRunArguementNumber;
            }
        }
        else if (pistonBase_InFunction.CustomData.ToLower().Contains("longitudinal"))
        {
            if (pistonBase_InFunction.CustomData.Contains("-1"))
            {
                pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.Z * terminalRunArguementNumber;
            }
            else
            {
                pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.Z * -1 * terminalRunArguementNumber;
            }
        }
        else if (pistonBase_InFunction.CustomData.ToLower().Contains("lateral"))
        {
            if (pistonBase_InFunction.CustomData.Contains("-1"))
            {
                pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.X * -1 * terminalRunArguementNumber;
            }
            else
            {
                pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.X * terminalRunArguementNumber;
            }
        }
    }
    //no valid input found in the terminal run arguement so use default sensitivity
    else
    {
        //checking declared orientation of pistons and assigning movement direction based on orientation with the cockpit
        if (pistonBase_InFunction.CustomData.ToLower().Contains("vertical"))
        {
            if (pistonBase_InFunction.CustomData.Contains("-1"))
            {
                pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.Y * -1;
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
                pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.X * -1;
            }
            else
            {
                pistonBase_InFunction.Velocity = cockpit_InFunction.MoveIndicator.X;
            }
        }
    }
}
public void MoveHingesAndRotors(IMyCockpit cockpit_InFunction, IMyMotorStator motorStator_InFunction)
{
    //Pitch => X
    //Yaw => Y

    //getting the input arguement to the programmable block so players can custom set the sensitivity 
    string terminalRunArguement = "";
    float terminalRunArguementNumber = 1;
    terminalRunArguement = Me.TerminalRunArgument;

    bool validSensitivity = float.TryParse(terminalRunArguement, out terminalRunArguementNumber);

    //the programmable block contains a valid sensitivity input in the terminal run arguement so adjust the sensitivity accordingly
    if (validSensitivity)
    {
        if (motorStator_InFunction.CustomData.ToLower().Contains("pitch"))
        {
            if (motorStator_InFunction.CustomData.Contains("-1"))
            {
                motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RotationIndicator.X * -1 / 2 * terminalRunArguementNumber;
            }
            else
            {
                motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RotationIndicator.X / 2 * terminalRunArguementNumber;
            }
        }
        else if (motorStator_InFunction.CustomData.ToLower().Contains("yaw"))
        {
            if (motorStator_InFunction.CustomData.Contains("-1"))
            {
                motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RotationIndicator.Y * -1 / 2 * terminalRunArguementNumber;
            }
            else
            {
                motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RotationIndicator.Y / 2 * terminalRunArguementNumber;
            }
        }
        else if (motorStator_InFunction.CustomData.ToLower().Contains("roll"))
        {
            if (motorStator_InFunction.CustomData.Contains("-1"))
            {
                motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RollIndicator * -1 * 2 * terminalRunArguementNumber;
            }
            else
            {
                motorStator_InFunction.TargetVelocityRPM = cockpit_InFunction.RollIndicator * 2 * terminalRunArguementNumber;
            }
        }
    }
    //no valid input found in the terminal run arguement so use default sensitivity
    else
    {
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
}
