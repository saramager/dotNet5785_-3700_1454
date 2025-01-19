

namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementation the func in  for IVolunteer
/// </summary>
internal class VolunteerImplementation : IVolunteer
{

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
       if(Read(vItem =>item.ID==vItem.ID)!=null)
            throw new DalAlreadyExistsException($"Volunteer with ID={item.ID} already exists");
        DataSource.Volunteers.Add(item);

    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        int numRemove = DataSource.Volunteers.RemoveAll(VolunteerToCheck => VolunteerToCheck.ID == id);
        if (numRemove == 0)
        {
            throw new DalDoesNotExistException($"Volunteer with ID={id} does Not exist");
        }
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        if (DataSource.Volunteers.Count != 0)
            DataSource.Volunteers.Clear();

    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
       return DataSource.Volunteers.FirstOrDefault(filter); 
    }



    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
     => filter == null
         ? DataSource.Volunteers.Select(item => item)
         : DataSource.Volunteers.Where(filter);




    [MethodImpl(MethodImplOptions.Synchronized)]
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
