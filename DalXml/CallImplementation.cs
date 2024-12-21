using DalApi;
using DO;

namespace Dal;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        int newID = Config.NextCallId;
        Call newCall = item with { ID = newID };
       Calls.Add(newCall);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_Calls_xml);

    }

    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        int numRemove = Calls.RemoveAll(callToCheck => callToCheck.ID == id);
        if (numRemove == 0)
        {
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
        }
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_Calls_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_Calls_xml);

    }

    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        return Calls.FirstOrDefault(filter) ?? throw new DO.DalDoesNotExistException($"volunteer with  {filter} does not  exist"); ;
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        if (filter == null)
            return XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml) ;
        return XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml).Where(filter) ;


    }

    public void Update(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
            int numRemove = Calls.RemoveAll(callToCheck => callToCheck.ID == item.ID);
            if (numRemove == 0)
            {
            throw new DalDoesNotExistException($"Call with ID={item.ID} does Not exist");
        }
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_Calls_xml);

    }
}
