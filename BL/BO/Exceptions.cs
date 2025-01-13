
namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}
[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}
[Serializable]
public class BlDoesAlreadyExistException : Exception
{
    public BlDoesAlreadyExistException(string? message) : base(message) { }
    public BlDoesAlreadyExistException (string message, Exception innerException)
                : base(message, innerException) { }
}
[Serializable]
public class AdressDoesNotExistException : Exception
{
    public AdressDoesNotExistException(string? message) : base(message) { }

}
[Serializable]
public class PaswordDoesNotstrongException : Exception
{
    public PaswordDoesNotstrongException(string? message) : base(message) { }
}
[Serializable]

public class PaswordDoesNotExistException : Exception
{
    public PaswordDoesNotExistException(string? message) : base(message) { }
}

[Serializable]
public class IdDoesNotVaildException : Exception
{
    public IdDoesNotVaildException(string? message) : base(message) { }
}
[Serializable]
public class PasswordIsNotCorrectException : Exception
{
    public PasswordIsNotCorrectException(string? message) : base(message) { }
}
[Serializable]
public class VolunteerCantUpadeOtherVolunteerException : Exception
{
    public VolunteerCantUpadeOtherVolunteerException(string? message) : base(message) { }
}
[Serializable]
public class CantUpdatevolunteer : Exception
{
    public CantUpdatevolunteer(string? message) : base(message) { }
}
[Serializable]

public class EmailDoesNotcoretct : Exception
{
    public EmailDoesNotcoretct(string? message) : base(message) { }
}
[Serializable]

public class PhoneDoesNotcoretct : Exception
{
    public PhoneDoesNotcoretct(string? message) : base(message) { }
}
[Serializable]

public class MaxDistanceDoesNotcoretct : Exception
{
    public MaxDistanceDoesNotcoretct(string? message) : base(message) { }
}
[Serializable]

public class CantDeleteCallException : Exception
{
    public CantDeleteCallException(string? message) : base(message) { }
}
[Serializable]
public class BlWrongInputException : Exception
{
    public BlWrongInputException(string? message) : base(message) { }
}
[Serializable]

public class BlWrongItemException : Exception
{
    public BlWrongItemException(string? message) : base(message) { }
}
public class BlValidationException : Exception
{
    public BlValidationException(string? message) : base(message) { }
}
[Serializable]

public class AssignmentAlreadyClosedException : Exception
{
    public AssignmentAlreadyClosedException(string? message) : base(message) { }
}

[Serializable]
public class BlUpdateCallException : Exception
{
    public BlUpdateCallException(string message) : base(message) { }
}

    [Serializable]
    public class volunteerHandleCallException : Exception
    {
        public volunteerHandleCallException(string message) : base(message) { }
    }

[Serializable]
public class NoManagerException : Exception
{
    public NoManagerException(string message) : base(message) { }
}
public class cantFilterCallException: Exception
{
    public cantFilterCallException(string message) : base(message) { }
}