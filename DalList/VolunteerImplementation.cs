

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
       if(Read(vItem =>item.ID==vItem.ID)!=null)
            throw new Exception($"Volunteer with ID={item.ID} already exists");
        DataSource.Volunteers.Add(item);

    }

    public void Delete(int id)
    {
        int numRemove = DataSource.Volunteers.RemoveAll(VolunteerToCheck => VolunteerToCheck.ID == id);
        if (numRemove == 0)
        {
            throw new DalDoesNotExistException($"Volunteer with ID={id} does Not exist");
        }
    }

    public void DeleteAll()
    {
        if (DataSource.Volunteers.Count != 0)
            DataSource.Volunteers.Clear();

    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
       return DataSource.Volunteers.FirstOrDefault(filter); 
    }


    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
     => filter == null
         ? DataSource.Volunteers.Select(item => item)
         : DataSource.Volunteers.Where(filter);



    public void Update(Volunteer item)
    {
        int numRemove = DataSource.Volunteers.RemoveAll(VolunteerToCheck => VolunteerToCheck.ID == item.ID);
        if (numRemove == 0)
        {
            throw new DalDoesNotExistException($"Volunteer with ID={item.ID} does Not exist");
        }
        DataSource.Volunteers.Add(item);
    }
}
