
namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddObserver(Action listObserver) =>
VolunteersManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
VolunteersManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
VolunteersManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
VolunteersManager.Observers.RemoveObserver(id, observer); //stage 5

    public void CreateVolunteer(BO.Volunteer volToAdd)
    {
       VolunteersManager.checkeVolunteerFormat(volToAdd);
        VolunteersManager.checkeVolunteerlogic(volToAdd);
        DO.Volunteer  DoVlo = VolunteersManager.convertFormBOVolunteerToDo(volToAdd);
        try 
        {
            _dal.Volunteer.Create(DoVlo);
            VolunteersManager.Observers.NotifyListUpdated(); // stage 5
        }
        catch(DO.DalAlreadyExistsException dEx) { throw new BO.BlDoesAlreadyExistException(dEx.Message, dEx); }

    }

    public void DeleteVolunteer(int id)
    {
        var assForVol = _dal.Assignment.ReadAll(ass => ass.VolunteerId == id);
        if (assForVol == null) { }
        else if (assForVol!.Count() > 0)
            throw new BO.volunteerHandleCallException($"volunteer with id={id} has assigments in his name");
        try
        {
            _dal.Volunteer.Delete(id);
            VolunteersManager.Observers.NotifyListUpdated(); // stage 5
        }
        catch(DO.DalDoesNotExistException dEx ) { throw new BO.BlDoesNotExistException(dEx.Message, dEx); }


    }

    public BO.RoleType EnterToSystem(int id, string password)
    {
        DO.Volunteer? v;
       

            v = _dal.Volunteer.Read(v => v.ID == id)??throw new BO.BlDoesNotExistException($"there is no vlounteer with id : {id} ");
            if (v.password == null || v.password.Length == 0)
                throw new PaswordDoesNotExistException("The passport is not Exist");
        string VpassWord = VolunteersManager.Decrypt(v.password!);
            if (/*VolunteersManager.Decrypt(v.password!)*/ VpassWord != password)
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
            volunteers = volunteers.OrderBy(v => v.ID).ToList();
        else switch (filedToSort)
            {
                case FiledOfVolunteerInList.ID:
                    volunteers = volunteers.OrderBy(v => v.ID).ToList();
                    break;

                case FiledOfVolunteerInList.fullName:
                    volunteers = volunteers.OrderBy(v => v.fullName).ToList();
                    break;

                case FiledOfVolunteerInList.active:
                    volunteers = volunteers.OrderBy(v => v.active).ToList();
                    break;

                case FiledOfVolunteerInList.numCallsHandled:
                    volunteers = volunteers.OrderBy(v => v.numCallsHandled).ToList();
                    break;

                case FiledOfVolunteerInList.numCallsCancelled:
                    volunteers = volunteers.OrderBy(v => v.numCallsCancelled).ToList();
                    break;

                case FiledOfVolunteerInList.numCallsExpired:
                    volunteers = volunteers.OrderBy(v => v.numCallsExpired).ToList();
                    break;

                case FiledOfVolunteerInList.CallId:
                    volunteers = volunteers.OrderBy(v => v.CallId).ToList();
                    break;

                case FiledOfVolunteerInList.callT:
                    volunteers = volunteers.OrderBy(v => v.callT).ToList();
                    break;
            }
        return volunteers;

 }
    
    public BO.Volunteer ReadVolunteer(int id)
    {
        DO.Volunteer? v;
        
            v = _dal.Volunteer.Read(v => v.ID == id);
            if (v == null)
                throw new BO.BlDoesNotExistException("($\"there is no Volunteer with id : {id} \"");
            return Helpers.VolunteersManager.convertDOToBOVolunteer(v);
    }
   
    public void UpdateVolunteer(int id, BO.Volunteer vol)
    {
     
       if (id!=vol.Id )
       {

            if (_dal.Volunteer.Read(vol => vol.ID == id)?.role != 0)
                throw new BO.VolunteerCantUpadeOtherVolunteerException("volunteer can't update other volunteer ");
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
       if (role == DO.RoleType.Volunteer )
            if (vol.role ==0 && vDo.role == DO.RoleType.Volunteer)
                throw new CantUpdatevolunteer($"vlounteer with id {id} cant change to manager  ");

        if (vol.Id != vDo.ID)
            throw new CantUpdatevolunteer($"vlounteer with id {id} can't change his id  ");

        try
        { 
            _dal.Volunteer.Update(Helpers.VolunteersManager.convertFormBOVolunteerToDo(vol));
            VolunteersManager.Observers.NotifyListUpdated(); // stage 5
            VolunteersManager.Observers.NotifyItemUpdated(vol.Id); // stage 5
        } 
        catch (DO.DalDoesNotExistException dEx) { throw new BO.BlDoesNotExistException(dEx.Message, dEx); }
    }
    public int ManagerID()
    {
       var manager= _dal.Volunteer.Read(v=>v.role==DO.RoleType.Manager);
        if (manager!= null)
            return manager.ID;
        else
           throw new BO.NoManagerException("there is no manger avilble ");
    }

}
