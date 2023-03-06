using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IPersonRepository
    {
        public Task<List<Person>> GetPeople();

        public Task<Person> GetPerson(int id);

        public Task<Person> GetPerson(string validIdNumber);

        //public Task<bool> PersonExists(int id);

        public void DetachPerson(Person person);

        public Task<Person> CreatePerson(Person person);

        public Task<Person> UpdatePerson(Person person);

        public Task<bool> DeletePerson(Person person);
    }
}
