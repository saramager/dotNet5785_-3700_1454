
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementation the func in  for IAssignment
/// </summary>
internal class AssignmentImplementation : IAssignment
{



    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {   
        int newID = Config.nextAssignmentId;
        Assignment newAssignment = item with { ID = newID };
        DataSource.Assignments.Add(newAssignment);

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        int numRemove = DataSource.Assignments.RemoveAll(AssignmentToCheck => AssignmentToCheck.ID == id);
        if (numRemove == 0)
        {
            throw new DalDoesNotExistException($"Assignment with ID={id} does Not exist");
        }

    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        if (DataSource.Assignments.Count != 0)
            DataSource.Assignments.Clear();

    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    { 
        return DataSource.Assignments.FirstOrDefault(filter);
    }



    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) //stage 2
      => filter == null
          ? DataSource.Assignments.Select(item => item)
          : DataSource.Assignments.Where(filter);



    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        int numRemove = DataSource.Assignments.RemoveAll(AssignmentToCheck => AssignmentToCheck.ID == item.ID);
        if (numRemove == 0)
        {
            throw new DalDoesNotExistException($"Assignment with ID={item.ID} does Not exist");
        }
        DataSource.Assignments.Add(item);

    }
}
