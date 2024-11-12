
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
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

    public Assignment? Read(int id)
    {
        var AssignmentToReturn = DataSource.Assignments.Find(AssignmentToCheck => AssignmentToCheck.ID == id);
        if (AssignmentToReturn == null)
            return null;
        return AssignmentToReturn;

    }

    public List<Assignment> ReadAll()
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
