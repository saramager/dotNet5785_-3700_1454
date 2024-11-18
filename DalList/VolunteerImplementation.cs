

namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
/// <summary>
/// Implementation the func in  for IVolunteer
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
       if(Read(item.ID)!=null)
            throw new Exception($"Volunteer with ID={item.ID} already exists");
        DataSource.Volunteers.Add(item);

    }

    public void Delete(int id)
    {
        int numRemove = DataSource.Volunteers.RemoveAll(VolunteerToCheck => VolunteerToCheck.ID == id);
        if (numRemove == 0)
        {
            throw new Exception($"Volunteer with ID={id} does Not exist");
        }
    }

    public void DeleteAll()
    {
        if (DataSource.Volunteers.Count != 0)
            DataSource.Volunteers.Clear();

    }

    public Volunteer? Read(int id)
    {
        var VolunteerToReturn = DataSource.Volunteers.FirstOrDefault(VolunteerToCheck => VolunteerToCheck.ID == id);
        if (VolunteerToReturn == null)
            return null;
        return VolunteerToReturn;
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return new List<Volunteer>(DataSource.Volunteers);

    }
 
    public void Update(Volunteer item)
    {
        int numRemove = DataSource.Volunteers.RemoveAll(VolunteerToCheck => VolunteerToCheck.ID == item.ID);
        if (numRemove == 0)
        {
            throw new Exception($"Volunteer with ID={item.ID} does Not exist");
        }
        DataSource.Volunteers.Add(item);
    }
}
