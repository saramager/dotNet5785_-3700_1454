
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
/// <summary>
/// Implementation the func in  for IAssignment
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {   
        int newID = Config.nextAssignmentId;
        Assignment newAssignment = item with { ID = newID };
        DataSource.Assignments.Add(newAssignment);

    }

    public void Delete(int id)
    {
        int numRemove = DataSource.Assignments.RemoveAll(AssignmentToCheck => AssignmentToCheck.ID == id);
        if (numRemove == 0)
        {
            throw new Exception($"Assignment with ID={id} does Not exist");
        }

    }

    public void DeleteAll()
    {
        if (DataSource.Assignments.Count != 0)
            DataSource.Assignments.Clear();

    }

    public Assignment? Read(Func<Assignment, bool> filter)
    { 
        return DataSource.Assignments.FirstOrDefault(filter);
    }


    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return new List<Assignment>(DataSource.Assignments);

    }

    public void Update(Assignment item)
    {
        int numRemove = DataSource.Assignments.RemoveAll(AssignmentToCheck => AssignmentToCheck.ID == item.ID);
        if (numRemove == 0)
        {
            throw new Exception($"Assignment with ID={item.ID} does Not exist");
        }
        DataSource.Assignments.Add(item);

    }
}
