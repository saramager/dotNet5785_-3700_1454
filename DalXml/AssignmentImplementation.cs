
using DalApi;
using DO;
using System.Linq;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        int newID = Config.NextAssignmentId;
        Assignment newAssignment = item with { ID = newID };
        Assignments.Add(newAssignment);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_Assignments_xml);

    }

    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        int numRemove = Assignments.RemoveAll(AssignmentToCheck => AssignmentToCheck.ID == id);
        if (numRemove == 0)
        {
            throw new DalDoesNotExistException($"Assignment with ID={id} does Not exist");
        }
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_Assignments_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_Assignments_xml);

    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        return Assignments.FirstOrDefault(filter);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);

    }

    public void Update(Assignment item)
    {

        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        int numRemove = Assignments.RemoveAll(AssignmentToCheck => AssignmentToCheck.ID == item.ID);
        if (numRemove == 0)
        {
            throw new DalDoesNotExistException($"Assignment with ID={item.ID} does Not exist");
        }
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_Assignments_xml);

    }
}
