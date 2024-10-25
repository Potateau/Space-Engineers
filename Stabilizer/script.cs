//credit to JoeTheDestroyer from https://forum.keenswh.com/threads/aligning-ship-to-planet-gravity.7373513/ for script 
//further edits by Potateau

//To Do
//Add speed limit functionality

string REMOTE_CONTROL_NAME = ""; //Set name for remote control to orient on,
                                 //leave blank to use first one found
double CTRL_COEFF = 0.8; //Set lower if overshooting, set higher to respond quicker
int LIMIT_GYROS = 1; //Set to the max number of gyros to use
                     //(Using less gyros than you have allows you to still steer while
                     // leveler is operating.)

IMyRemoteControl rc;
List<IMyGyro> gyros;



public Program()
{
    //fast runtime to keep the ship stable
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}

public void Main(string argument, UpdateType updateSource)
{

    //Initial setup
    if (rc == null)
    {
        setup();
    }

    //SET THE TOLERANCE
    //Same tolerance for all angles
    double angleTolerance = 0.01;//adjusts the angle tolerance to align to gravity
    //checking if there is a terminal run argument to adjust the alignment tolerance
    //getting the input arguement to the programmable block so players can custom set the sensitivity 
    string terminalRunArguement = "";
    double terminalRunArguementNumber = angleTolerance; //set the default to not accidentally change behaviour
    terminalRunArguement = Me.TerminalRunArgument;
    bool validSensitivity = double.TryParse(terminalRunArguement, out terminalRunArguementNumber);
    //if there is a valid number in the run arguement then set the tolerance to that value
    if (validSensitivity)
    {
        angleTolerance = terminalRunArguementNumber;
    }


    //The orientation matrix of the grid
    Matrix orientationMatrix;
    rc.Orientation.GetMatrix(out orientationMatrix);

    //The local down vector of the grid - This points where the "bottom" of the ship is pointing, we want to align this to the gravity vector
    Vector3D downVector = orientationMatrix.Down;
    //The gravity vector
    Vector3D gravityVector = rc.GetNaturalGravity();



    foreach (var gyro in gyros)
    {
        //gyro.Orientation.GetMatrix(out orientationMatrix);

        //getting local down vectors and gravity vectors 
        var localDown = Vector3D.Transform(downVector, MatrixD.Transpose(orientationMatrix));
        var localGravity = Vector3D.Transform(gravityVector, MatrixD.Transpose(gyro.WorldMatrix.GetOrientation()));

        //we need a rotation angle to feed into the gyro
        var rotation = Vector3D.Cross(localDown, localGravity);
        double ang = rotation.Length();

        //This is JoeTheDestroyer's method but it didn't make sense and either kept the ship perfectly level or didn't work at all with the tolerance value
        //ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang))); //More numerically stable than: ang=Math.Asin(ang)

        //Less stable but it can take in a tolerance value in radians in the arguement field and works
        //Same tolerance for all angles
        ang = Math.Acos(Vector3D.Dot(localDown, localGravity) / (Math.Abs(localDown.Length()) * Math.Abs(localGravity.Length())));

        if (Math.Abs(ang) < Math.Abs(angleTolerance))
        {//close enough
            gyro.SetValueBool("Override", false);//effectively turns off the gyro
            continue;//stop this loop
        }

        //Control speed to be proportional to distance (angle) we have left
        double ctrl_vel = gyro.GetMaximum<float>("Yaw") * (ang / Math.PI) * CTRL_COEFF;
        ctrl_vel = Math.Min(gyro.GetMaximum<float>("Yaw"), ctrl_vel);
        ctrl_vel = Math.Max(0.01, ctrl_vel); //Gyros don't work well at very low speeds so feed it a minimum value by taking a max between 0.01 and the found value
        rotation.Normalize();
        rotation *= ctrl_vel;
        gyro.SetValueFloat("Pitch", (float)rotation.GetDim(0));
        gyro.SetValueFloat("Yaw", -(float)rotation.GetDim(1));
        gyro.SetValueFloat("Roll", -(float)rotation.GetDim(2));

        gyro.SetValueFloat("Power", 1.0f);
        gyro.SetValueBool("Override", true);
    }




}

void setup()
{
    var l = new List<IMyTerminalBlock>();

    rc = (IMyRemoteControl)GridTerminalSystem.GetBlockWithName(REMOTE_CONTROL_NAME);
    if (rc == null)
    {
        GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(l, x => x.CubeGrid == Me.CubeGrid);
        rc = (IMyRemoteControl)l[0];
    }

    GridTerminalSystem.GetBlocksOfType<IMyGyro>(l, x => x.CubeGrid == Me.CubeGrid);
    gyros = l.ConvertAll(x => (IMyGyro)x);
    if (gyros.Count > LIMIT_GYROS)
        gyros.RemoveRange(LIMIT_GYROS, gyros.Count - LIMIT_GYROS);
}
