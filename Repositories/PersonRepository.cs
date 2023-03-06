using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Tensorflow;

namespace FaceRecognitionWebAPI.Respository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DataContext _context;
        public PersonRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Person>> GetPeople()
        {
            try
            {
                return await _context.Persons.OrderBy(p => p.Id).Include(p => p.FacesToTrain).Include(p => p.FaceRecognitionStatuses).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Person> GetPerson(int id)
        {
            try
            {
                return await _context.Persons.Where(p => p.Id == id).Include(p => p.FacesToTrain).Include(p => p.FaceRecognitionStatuses).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Person> GetPerson(string validIdNumber) 
        {
            try
            {
                return await _context.Persons.Where(p => p.ValidIdNumber.Trim() == validIdNumber.Trim()).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public bool PersonExists(int id)
        //{
        //    return _context.Persons.Any(p => p.Id == id);
        //}

        public void DetachPerson(Person person)
        {
            try
            {
                _context.Entry(person).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<Person> CreatePerson(Person person)
        {
            try
            {
                _context.Persons.Add(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            try
            {
                _context.Persons.Update(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeletePerson(Person person)
        {
            try
            {
                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
