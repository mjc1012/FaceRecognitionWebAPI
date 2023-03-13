using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IPersonRepository
    {
        public Task<List<Person>> GetPeople();

        public Task<Person> GetPerson(int id);

        public Task<List<Person>> GetPeople(List<string> validIdNumbers);

        public Task<Person> GetPerson(string validIdNumber);

        public void DetachPerson(Person person);

        public Task<Person> CreatePerson(Person person);

        public Task<Person> UpdatePerson(Person person);

        public Task<bool> DeletePerson(Person person);

        public Task<bool> DeletePeople(List<Person> people);
    }
}
