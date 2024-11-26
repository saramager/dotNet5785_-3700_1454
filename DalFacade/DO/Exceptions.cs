
namespace DO;
[Serializable]

public class DalDoesNotExistException : Exception //An attempt to update, delete, or request an object with an ID number that does not exist.
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

public class DalAlreadyExistsException : Exception //It is not possible to add a new object with an ID number that is already taken.
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}
public class DalXMLFileLoadCreateException : Exception //Stage3
{
   public DalXMLFileLoadCreateException(string? message) : base(message) { }
}
