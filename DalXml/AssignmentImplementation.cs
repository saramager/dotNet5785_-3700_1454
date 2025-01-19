using DalApi;
using DO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment and adds it to the list of assignments stored in the XML file.
    /// </summary>
    /// <param name="item">The assignment object to be created.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
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
    [MethodImpl(MethodImplOptions.Synchronized)]
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
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_Assignments_xml);

    }
    /// <summary>
    /// Reads a single assignment based on a filter function.
    /// </summary>
    /// <param name="filter">A function that defines the filter criteria for the assignment.</param>
    /// <returns>The first assignment that matches the filter, or null if no match is found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        return Assignments.FirstOrDefault(filter)?? throw new DO.DalDoesNotExistException($"volunteer with   {filter} does not  exist"); ;
    }
    /// <summary>
    /// Reads all assignments. Optionally filters the list based on a given filter function.
    /// </summary>
    /// <param name="filter">A function that defines the filter criteria for the assignments (optional).</param>
    /// <returns>A collection of assignments that match the filter (if any).</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        try
        {
            // אם אין פילטר, מחזירים את כל הרשומות
            if (filter == null)
                return XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);

            // אם יש פילטר, מחזירים את הרשומות לאחר פילטור
            return XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml).Where(filter);
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            // טיפול בשגיאה וזריקת חריגה חדשה עם פרטים נוספים
            throw new ApplicationException($"Error while reading assignments from file {Config.s_Assignments_xml}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Updates an existing assignment by replacing it with the provided one.
    /// Throws an exception if the assignment does not exist.
    /// </summary>
    /// <param name="item">The updated assignment object.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
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
