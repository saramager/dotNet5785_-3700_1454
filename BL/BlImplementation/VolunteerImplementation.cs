
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
    public void CreateVolunteer(Volunteer volToAdd)
    {
       VolunteersManager.checkeVolunteerFormat(volToAdd);
       VolunteersManager.checkeVolunteerlogic(volToAdd);
       



    }

    public void DeleteVolunteer(int id)
    {
        var assForVol = _dal.Assignment.ReadAll(ass => ass.VolunteerId == id);
        if (assForVol == null) {}
       else if (assForVol!.Count(ass => ass.finishT == null) > 0)
            throw new Exception();
        try{
            _dal.Volunteer.Delete(id); }
        catch(Exception ex) { }

    }

    public BO.RoleType EnterToSystem(int id, string password)
    {
        DO.Volunteer? v;
        try {

            v = _dal.Volunteer.Read(v => v.ID == id)??throw new Exception();
            if (v.password == null || v.password.Length == 0)
                throw new Exception("to do ");
            if (VolunteersManager.Decrypt(v.password!) != password)
                throw new Exception("to do ");
            return (BO.RoleType)v.role;
        }
      
        catch (DO.DalDoesNotExistException NotEx){ throw new Exception("",NotEx); }
        catch (Exception EX) { throw new Exception("   ",EX); }
    }

    public IEnumerable<VolunteerInList> GetVolunteerInList(bool? IsActive, FiledOfVolunteerInList? filedToSort)
    {
        IEnumerable<VolunteerInList> volunteers;
        try
        {
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
        catch (DO.DalDoesNotExistException NotEx) { throw new Exception(); }
    }

    public BO.Volunteer ReadVolunteer(int id)
    {
        DO.Volunteer? v;
        try
        {
            v = _dal.Volunteer.Read(v => v.ID == id);
            if (v == null)
                throw new Exception();//change 
            return Helpers.VolunteersManager.convertDOToBOVolunteer(v);

        }
        catch
        {
            throw new Exception();//change 

        }

    }

    public void UpdateVolunteer(int id, BO.Volunteer vol)
    {
     
       if (id!=vol.Id )
       {

            if (_dal.Volunteer.Read(vol => vol.ID == id)?.role != 0)
                throw new Exception();
        }
       Helpers.VolunteersManager.checkeVolunteerFormat(vol);
        VolunteersManager.checkeVolunteerlogic(vol);
        //לחלק לבדיקה לוגית ופורמט 
       var role = _dal.Volunteer.Read(vol => vol.ID == id)?.role;
        DO.Volunteer vDo = _dal.Volunteer.Read(vo => vo.ID == vol.Id)!;
       if (vol.active == false)
        {
            var assForVol = _dal.Assignment.ReadAll(ass => ass.VolunteerId == vol.Id );
           // לבדוק שזה לא null 
            if (assForVol.Count(ass => ass.finishT == null) > 0)
               throw new Exception();
         }
       if (role == DO.RoleType.TVolunteer )
            if (vol.role ==0 && vDo.role == DO.RoleType.TVolunteer)
                throw new Exception();

        if (vol.Id != vDo.ID)
            throw new Exception();


        _dal.Volunteer.Update(Helpers.VolunteersManager.convertFormBOVolunteerToDo(vol));   
    }
}
