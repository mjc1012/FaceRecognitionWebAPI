﻿using FaceRecognitionWebAPI.Data;
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
                return await _context.Persons.OrderBy(p => p.LastName).ThenBy(p => p.MiddleName).ThenBy(p => p.FirstName).Include(p => p.FacesToTrain).Include(p => p.FaceRecognitionStatuses).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Person>> GetPeople(List<int> pairIds)
        {
            try
            {
                return await _context.Persons.Where(p => pairIds.Contains(p.PairId)).OrderBy(p => p.LastName).ThenBy(p => p.MiddleName).ThenBy(p => p.FirstName).Include(p => p.FacesToTrain).Include(p => p.FaceRecognitionStatuses).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Person> GetPersonById(int id)
        {
            try
            {
                return await _context.Persons.Where(p => p.Id == id).Include(p => p.FacesToTrain).Include(p => p.FaceRecognitionStatuses).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Person> GetPersonByPairId(int pairId) 
        {
            try
            {
                return await _context.Persons.Where(p => p.PairId == pairId).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DetachPerson(Person person)
        {
            try
            {
                _context.Entry(person).State = EntityState.Detached;
            }
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception )
            {
                throw;
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeletePeople(List<Person> people)
        {
            try
            {
                _context.Persons.RemoveRange(people);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
