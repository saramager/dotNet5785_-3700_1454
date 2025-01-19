
using DalApi;
using DO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Dal;
/// <summary>
/// the class is to help us use voulnteer with XML 
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// help func return the volunteer that is in the XElement 
    /// </summary>
    /// <param name="s">n XElement vatrible </param>
    /// <returns>  the volunteer  from the XElement</returns>
    /// <exception cref="FormatException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    static Volunteer getVolunteer(XElement vXml)
    {
        Volunteer v = new DO.Volunteer()
        {
            ID = int.TryParse((string?)vXml.Element("ID"), out var id) ? id : throw new FormatException("can't convert id"),
            fullName = (string?)vXml.Element("fullName") ?? "",
            phone = (string?)vXml.Element("phone") ?? "",
            email = (string?)vXml.Element("email") ?? "",
            active = bool.TryParse((string?)vXml.Element("isActive"), out bool active) ? active : throw new FormatException("can't convert active"),
            role = RoleType.TryParse((string?)vXml.Element("role"), out RoleType role) ? role : throw new FormatException("can't convert role "),
            distanceType = Distance.TryParse((string?)vXml.Element("distance"), out Distance dis) ? dis : throw new FormatException("can't convert distance "),
            password = (string?)vXml.Element("password") ?? null,
            currentAddress = (string?)vXml.Element("address") ?? null,
            Longitude = double.TryParse((string?)vXml.Element("longitude"), out double longitude) ? longitude : null,
            Latitude = double.TryParse((string?)vXml.Element("latitude"), out double latitude) ? latitude : null,
            maxDistance = double.TryParse((string?)vXml.Element("maxDistance"), out double maxDis) ? maxDis : null,
        };
        return v;
    }
    /// <summary>
    ///help func return the volunteer in XElement.
    /// </summary>
    /// <param name="volunteer"> volunteer from the user </param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    static XElement createVolunteerElement(Volunteer volunteer)
    {
        XElement volunteerXml = new XElement("Volunteer",
        new XElement("ID", volunteer.ID),
                              new XElement("fullName", volunteer.fullName),
                              new XElement("phone", volunteer.phone),
                                new XElement("email", volunteer.email),
                                 new XElement("isActive", volunteer.active),
                                  new XElement("role", volunteer.role),
                                   new XElement("distance", volunteer.distanceType),
                                    (volunteer.password != null ? new XElement("password", volunteer.password!) : null),
                                    (volunteer.currentAddress != null ? new XElement("address", volunteer.currentAddress!) : null),
                                     (volunteer.Longitude != null ? new XElement("longitude", volunteer.Longitude!) : null),
                                      (volunteer.Latitude != null ? new XElement("latitude", volunteer.Latitude!) : null),
                                       (volunteer.maxDistance != null ? new XElement("maxDistance", volunteer.maxDistance) : null)


                              );
        return volunteerXml;


    }
    /// <summary>
    /// add the volunteer to the Xml file 
    /// </summary>
    /// <param name="item"> volunteer to add to the </param>
    /// <exception cref="DO.DalAlreadyExistsException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        if ((volunteerRootElem.Elements().FirstOrDefault(st => (int?)st.Element("ID") == item.ID)) != null)
            throw new DO.DalAlreadyExistsException($"volunteer with ID={item.ID} already  exist");

        volunteerRootElem.Add(new XElement(createVolunteerElement(item)));

        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_Volunteers_xml);
    }
    /// <summary>
    /// get an id and delete the volunteer that owns that id for the XML file 
    /// </summary>
    /// <param name="id"> the id for the volunteer to delete </param>
    /// <exception cref="DO.DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        if ((volunteerRootElem.Elements().FirstOrDefault(st => (int?)st.Element("ID") == id)) == null)
            throw new DO.DalDoesNotExistException($"volunteer with ID={id} does not  exist");
        XElement volunteerElem = (from stu in volunteerRootElem.Elements()
                                  where int.Parse(stu.Element("ID")!.Value) == id
                                  select stu).FirstOrDefault()!;
        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_Volunteers_xml);

    }
    /// <summary>
    /// delete all the volunteers form the XML file 
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);
        volunteerRootElem.RemoveAll();
        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_Volunteers_xml);

    }

    /// <summary>
    /// return the volunteer from the file, the first volunteer that true from the filter 
    /// </summary>
    /// <param name="filter">  func to help us chose the volunteer </param>
    /// <returns> lhe first volunteer to true for the filter </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml).Elements().Select(s => getVolunteer(s)).FirstOrDefault(filter);

    }
    /// <summary>
    /// return all the volunteers 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        if (filter == null)
            return XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml).Elements().Select(v => getVolunteer(v));
        return XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml).Elements().Select(v => getVolunteer(v)).Where(filter);
    }
    /// <summary>
    /// get an voulntter item and update it to the new values.
    /// </summary>
    /// <param name="item"> the volunteer to update - </param>
    /// <exception cref="DO.DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        (volunteerRootElem.Elements().FirstOrDefault(st => (int?)st.Element("ID") == item.ID) ?? throw new DO.DalDoesNotExistException($"volunteer with ID={item.ID} does Not exist")).Remove();

        volunteerRootElem.Add(new XElement(createVolunteerElement(item)));

        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_Volunteers_xml);
    }
  

}
