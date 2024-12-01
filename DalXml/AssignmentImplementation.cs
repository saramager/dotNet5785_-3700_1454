
using DalApi;
using DO;
using System.Linq;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment and adds it to the list of assignments stored in the XML file.
    /// </summary>
    /// <param name="item">The assignment object to be created.</param>
    public void Create(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        int newID = Config.NextAssignmentId;
        Assignment newAssignment = item with { ID = newID };
        Assignments.Add(newAssignment);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_Assignments_xml);

    }
    /// <summary>
    /// Deletes an assignment by its ID. Throws an exception if the assignment does not exist.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
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
    /// <summary>
    /// Deletes all assignments in the system by clearing the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_Assignments_xml);

    }
    /// <summary>
    /// Reads a single assignment based on a filter function.
    /// </summary>
    /// <param name="filter">A function that defines the filter criteria for the assignment.</param>
    /// <returns>The first assignment that matches the filter, or null if no match is found.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        return Assignments.FirstOrDefault(filter);
    }
    /// <summary>
    /// Reads all assignments. Optionally filters the list based on a given filter function.
    /// </summary>
    /// <param name="filter">A function that defines the filter criteria for the assignments (optional).</param>
    /// <returns>A collection of assignments that match the filter (if any).</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);

    }
    /// <summary>
    /// Updates an existing assignment by replacing it with the provided one.
    /// Throws an exception if the assignment does not exist.
    /// </summary>
    /// <param name="item">The updated assignment object.</param>
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
