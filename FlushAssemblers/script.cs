public void Main(string argument, UpdateType updateSource)
{
    //Create list to store variables    
    List<IMyAssembler> AllAssemblers = new List<IMyAssembler>();
    //Use function to store all assemblers on the grid in variable    
    AllAssemblers = CreateAssemblerList();

    //for each assembler on the grid, clear the queue    
    foreach (var assembler in AllAssemblers) {
        assembler.ClearQueue();
    }
}

public List<IMyAssembler> CreateAssemblerList()
{
    List<IMyAssembler> allAssemblers_InFunction = new List<IMyAssembler>();
    //creating the assembler list of all the assemblers only on the same grid as the programmable block
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(allAssemblers_InFunction, b => b.CubeGrid == Me.CubeGrid);
    return allAssemblers_InFunction;
}
