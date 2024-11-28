
using DalApi;
using DO;
using System.Xml.Linq;

namespace Dal;

internal class VolunteerImplementation : IVolunteer
{
    static  Volunteer getStudent(XElement s)
    {
        return new DO.Volunteer()
        {
            ID = int.TryParse((string?)s.Element("ID"), out var id) ? id : throw new FormatException("can't convert id"),
            fullName = (string?)s.Element("full name") ?? "",
            phone = (string?)s.Element("phone") ?? "",
            email = (string?)s.Element("email") ?? "",
            active = bool.TryParse((string?)s.Element("is active"), out bool active) ? active : throw new FormatException("can't convert active"),
            role = RoleType.TryParse((string?)s.Element("role"), out RoleType role) ? role : throw new FormatException("can't convert role "),
            distanceType = Distance.TryParse((string?)s.Element("distance"), out Distance dis) ? dis : throw new FormatException("can't convert distance "),
            password = (string?)s.Element("password") ?? null,
            currentAddress = (string?)s.Element("address") ?? null,
            Longitude = double.TryParse((string?)s.Element("longitude"), out double longitude) ? longitude : null,
            Latitude = double.TryParse((string?)s.Element("latitude"), out double latitude) ? latitude : null,
            maxDistance=double.TryParse((string?)s.Element("max distance"),out double maxDis)?maxDis:null,
           };
    }
    static XElement createVolunteerElement(Volunteer volunteer)
    {
        XElement studentXml = new XElement("student",
        new XElement("ID", volunteer.ID),
                              new XElement("full name", volunteer.fullName),
                              new XElement("phone", volunteer.phone),
                                new XElement("email", volunteer.email),
                                 new XElement("is active", volunteer.active),
                                  new XElement("role", volunteer.role),
                                   new XElement("distance", volunteer.distanceType),
                                    (volunteer.password != null ? new XElement("password", volunteer.password!) : null),
                                    (volunteer.currentAddress != null ? new XElement("address", volunteer.currentAddress!) : null),
                                     (volunteer.Longitude != null ? new XElement("longitude", volunteer.Longitude!) : null),
                                      (volunteer.Latitude != null ? new XElement("latitude", volunteer.Latitude!) : null),
                                       (volunteer.maxDistance != null ? new XElement("max distance", volunteer.maxDistance) : null)


                              );
            return studentXml;
                            
                          
    }

    public void Create(Volunteer item)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        if ((volunteerRootElem.Elements().FirstOrDefault(st => (int?)st.Element("ID") == item.ID)) != null)
            throw new DO.DalAlreadyExistsException($"Student with ID={item.ID} already  exist");

        volunteerRootElem.Add(new XElement("Student", createVolunteerElement(item)));

        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_Volunteers_xml);
    }

    public void Delete(int id)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        if ((volunteerRootElem.Elements().FirstOrDefault(st => (int?)st.Element("ID") == id)) == null)
            throw new DO.DalAlreadyExistsException($"Student with ID={id} does not  exist");
        XElement volunteerElem = (from stu in volunteerRootElem.Elements()
                          where int.Parse(stu.Element("ID")!.Value) == id
                          select stu).FirstOrDefault()!;
        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_Volunteers_xml);

    }

    public void DeleteAll()
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);
       
        volunteerRootElem.Remove();
        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_Volunteers_xml);

    }


    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml).Elements().Select(s => getStudent(s)).FirstOrDefault(filter);

    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml).Elements().Select(s => getStudent(s));

    }

    public void Update(Volunteer item)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        (volunteerRootElem.Elements().FirstOrDefault(st => (int?)st.Element("ID") == item.ID)?? throw new DO.DalDoesNotExistException($"Student with ID={item.ID} does Not exist")).Remove();

        volunteerRootElem.Add(new XElement("Student", createVolunteerElement(item)));

        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_Volunteers_xml);
    }
}
