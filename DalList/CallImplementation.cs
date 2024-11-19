
namespace Dal;
using DalApi;
using DO;

/// <summary>
///  Implementation the func in  for ICall
/// </summary>
internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
       int  newID = Config.nextCallId;
        Call newCall = item with { id = newID};
       DataSource.Calls.Add(newCall);
       
    }

    public void Delete(int id)
    {
        int numRemove = DataSource.Calls.RemoveAll(callToCheck => callToCheck.id == id);
        if (numRemove == 0)
        {
            throw new Exception($"Call with ID={id} does Not exist");
        }
    }
    public void DeleteAll()
    {
        if (DataSource.Calls.Count != 0)
         DataSource.Calls.Clear(); 
    }

    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) //stage 2
      => filter == null
          ? DataSource.Calls.Select(item => item)
          : DataSource.Calls.Where(filter);

    public void Update(Call item)
    {
        int numRemove = DataSource.Calls.RemoveAll(callToCheck => callToCheck.id == item.id);
        if (numRemove == 0)
        {
            throw new Exception($"Call with ID={item.id} does Not exist");
        }
        DataSource.Calls.Add(item);


    }
}
