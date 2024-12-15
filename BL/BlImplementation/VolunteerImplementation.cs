
namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;

internal class VolunteerImplementation : IVolunteer

{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void CreateVolunteer(BO.Volunteer volToAdd)
    {
       VolunteersManager.checkeVolunteerFormat(volToAdd);
        VolunteersManager.checkeVolunteerlogic(volToAdd);
        DO.Volunteer  DoVlo = VolunteersManager.convertFormBOVolunteerToDo(volToAdd);
        try 
       { _dal.Volunteer.Create(DoVlo); }
        catch(DO.DalAlreadyExistsException dEx) { throw new BO.BlDoesAlreadyExistException(dEx.Message, dEx); }

    }

    public void DeleteVolunteer(int id)
    {
        var assForVol = _dal.Assignment.ReadAll(ass => ass.VolunteerId == id);
        if (assForVol == null) {}
       else if (assForVol!.Count(ass => ass.finishT == null) > 0)
            throw new Exception();
        try{
            _dal.Volunteer.Delete(id); }
        catch(DO.DalDoesNotExistException dEx ) { throw new BO.BlDoesNotExistException(dEx.Message, dEx); }

    }

    public BO.RoleType EnterToSystem(int id, string password)
    {
        DO.Volunteer? v;
       

            v = _dal.Volunteer.Read(v => v.ID == id)??throw new BO.BlDoesNotExistException($"there is no vlounteer with id : {id} ");
            if (v.password == null || v.password.Length == 0)
                throw new PaswordDoesNotExistException("The passport is not Exist");
            if (VolunteersManager.Decrypt(v.password!) != password)
                throw new PasswordIsNotCorrectException($"for Id: {v.ID} the password {password} is not true");
            return (BO.RoleType)v.role;
       
        
    }
 
    public IEnumerable<VolunteerInList> GetVolunteerInList(bool? IsActive, FiledOfVolunteerInList? filedToSort)
    {
        IEnumerable<VolunteerInList> volunteers;
       
            if (IsActive != null)
               volunteers = _dal.Volunteer.ReadAll(v => v.active == IsActive).Select(v=>VolunteersManager.convertDOToBOInList(v));
            else
                volunteers = _dal.Volunteer.ReadAll().Select(v => VolunteersManager.convertDOToBOInList(v));
            if (filedToSort == null)
                volunteers.OrderBy(v => v.ID);
            else switch (filedToSort)
                {
                    case FiledOfVolunteerInList.ID:
                        volunteers.OrderBy(v => v.ID);
                        break;

                    case FiledOfVolunteerInList.fullName:
                        volunteers.OrderBy(v => v.fullName);
                        break;

                    case FiledOfVolunteerInList.active:
                        volunteers.OrderBy(v => v.active);
                        break;

                    case FiledOfVolunteerInList.numCallsHandled:
                        volunteers.OrderBy(v => v.numCallsHandled);
                        break;

                    case FiledOfVolunteerInList.numCallsCancelled:
                        volunteers.OrderBy(v => v.numCallsCancelled);
                        break;

                    case FiledOfVolunteerInList.numCallsExpired:
                        volunteers.OrderBy(v => v.numCallsExpired);
                        break;

                    case FiledOfVolunteerInList.CallId:
                        volunteers.OrderBy(v => v.CallId);
                        break;

                    case FiledOfVolunteerInList.callT:
                        volunteers.OrderBy(v => v.callT);
                        break;

            
                }
            return volunteers;

 }
    
    public BO.Volunteer ReadVolunteer(int id)
    {
        DO.Volunteer? v;
        
            v = _dal.Volunteer.Read(v => v.ID == id);
            if (v == null)
                throw new BO.BlDoesNotExistException("($\"there is no vlounteer with id : {id} \"");
            return Helpers.VolunteersManager.convertDOToBOVolunteer(v);

      

    }
   
    public void UpdateVolunteer(int id, BO.Volunteer vol)
    {
     
       if (id!=vol.Id )
       {

            if (_dal.Volunteer.Read(vol => vol.ID == id)?.role != 0)
                throw new BO.VolunteerCantUpadeOtherVolunteerException("volunteer can't upduet other volunteer ");
        }
       VolunteersManager.checkeVolunteerFormat(vol);
        VolunteersManager.checkeVolunteerlogic(vol);
       var role = _dal.Volunteer.Read(vol => vol.ID == id)?.role;
        DO.Volunteer vDo = _dal.Volunteer.Read(vo => vo.ID == vol.Id)!;
       if (vol.active == false)
        {
            var assForVol = _dal.Assignment.ReadAll(ass => ass.VolunteerId == vol.Id );
            if (assForVol!= null)
           { if (assForVol.Count(ass => ass.finishT == null) > 0)
                    throw new  CantUpdatevolunteer($"vlounteer with id {id} have open assigments ");
            }
         }
       if (role == DO.RoleType.TVolunteer )
            if (vol.role ==0 && vDo.role == DO.RoleType.TVolunteer)
                throw new CantUpdatevolunteer($"vlounteer with id {id} cant change to manager  ");

        if (vol.Id != vDo.ID)
            throw new CantUpdatevolunteer($"vlounteer with id {id} can't chage his id  ");

        try
        { _dal.Volunteer.Update(Helpers.VolunteersManager.convertFormBOVolunteerToDo(vol));  } 
        catch (DO.DalDoesNotExistException dEx) { throw new BO.BlDoesNotExistException(dEx.Message, dEx); }
    }
}
