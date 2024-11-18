﻿
using DO;

namespace DalApi;
/// <summary>
/// generic interface - father  for all Curd interfaces 
/// </summary>
/// <typeparam name="T"></typeparam> generic type 
public interface ICrud<T> where T :class
{
    void Create(T item); //Creates new entity object in DAL
    T? Read(int id); //Reads entity object by its ID 
    List<T> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(T item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects

}
