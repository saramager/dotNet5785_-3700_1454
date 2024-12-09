
namespace BlImplementation;
using BlApi;
using BO;
using System.Collections.Generic;
using System.ComponentModel.Design;

internal class VolunteerImplementation : IVolunteer

{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void CreateVolunteer(Volunteer volToAdd)
    {

       
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
           v = _dal.Volunteer.Read(v=> v.ID==id&& v.password==password)?? throw new  Exception("to do ");
            return (BO.RoleType)v.role;
        }
      
        catch (DO.DalDoesNotExistException NotEx){ throw new Exception("",NotEx); }
        catch (Exception EX) { throw new Exception("   ",EX); }
    }

    //public IEnumerable<VolunteerInList> GetVolunteerInList(bool? IsActive, FiledOfVolunteerInList? filedToSort)
    //{

    //    //try
    //    //{
    //    //    if (IsActive != null)
    //    //        var volunteers = _dal.Volunteer.ReadAll(v => v.active == IsActive).Select(;
    //    //}
    //    //catch (DO.DalDoesNotExistException NotEx) { throw new Exception()}
    //}

    public BO.Volunteer ReadVolunteer(int id)
    {
        DO.Volunteer v;
        try
        {
            v = _dal.Volunteer.Read(v => v.ID == id);
            if (v == null)
                throw new Exception();//change 
            return Helpers.VolunteersManager.convertDOToBOVolunteer(v);

        }
        catch
        {

        }
         
    }

    public void UpdateVolunteer(int id, BO.Volunteer vol)
    {
     
       if (id!=vol.Id )
       {

            if (_dal.Volunteer.Read(vol => vol.ID == id)?.role != 0)
                throw new Exception();
        }
       Helpers.VolunteersManager.checkeVolunteer(vol);
       var role = _dal.Volunteer.Read(vol => vol.ID == id)?.role;
        DO.Volunteer vDo = _dal.Volunteer.Read(vo => vo.ID == vol.Id)!;
       if (vol.active == false)
        {
            var assForVol = _dal.Assignment.ReadAll(ass => ass.VolunteerId == vol.Id );
            if (assForVol.Count(ass => ass.finishT == null) > 0)
               throw new Exception();
         }
       if (role == 0)
            if (vol.role ==0 && vDo.role ==  (DO.RoleType) 1)
                throw new Exception();

        if (vol.Id != vDo.ID)
            throw new Exception();


        _dal.Volunteer.Update(Helpers.VolunteersManager.convertFormBOVolunteerToDo(vol));   
    }
}
