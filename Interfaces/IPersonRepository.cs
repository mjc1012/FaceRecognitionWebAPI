using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IPersonRepository
    {
        public Task<List<Person>> GetPeople();

        public Task<Person> GetPersonById(int id);

        public Task<List<Person>> GetPeople(List<int> pairIds);

        public Task<Person> GetPersonByPairId(int pairId);

        public void DetachPerson(Person person);

        public Task<Person> CreatePerson(Person person);

        public Task<Person> UpdatePerson(Person person);

        public Task<bool> DeletePerson(Person person);

        public Task<bool> DeletePeople(List<Person> people);
    }
}
