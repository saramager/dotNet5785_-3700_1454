
namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

/// <summary>
///  Implementation the func in  for ICall
/// </summary>
internal class CallImplementation : ICall
{

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
       int  newID = Config.nextCallId;
        Call newCall = item with { ID = newID};
       DataSource.Calls.Add(newCall);
       
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        int numRemove = DataSource.Calls.RemoveAll(callToCheck => callToCheck.ID == id);
        if (numRemove == 0)
        {
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        if (DataSource.Calls.Count != 0)
         DataSource.Calls.Clear(); 
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) //stage 2
      => filter == null
          ? DataSource.Calls.Select(item => item)
          : DataSource.Calls.Where(filter);


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        int numRemove = DataSource.Calls.RemoveAll(callToCheck => callToCheck.ID == item.ID);
        if (numRemove == 0)
        {
            throw new DalDoesNotExistException($"Call with ID={item.ID} does Not exist");
        }
        DataSource.Calls.Add(item);


    }
}
