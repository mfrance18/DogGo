using DogGo.Models;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface IDogRepository
    {
        void AddDog(Dog dog);
        void DeleteDog(int dogId);
        List<Dog> GetAllDogs();
        List<Dog> GetDogsByOwnerId(int ownerId);
        Dog GetDogById(int id);
        void UpdateDog(Dog dog);
    }
}