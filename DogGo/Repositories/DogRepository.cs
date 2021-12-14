using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using DogGo.Models;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {

        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public DogRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id as dogId, d.Name as dogName, Breed, Notes, ImageUrl, o.Name as ownerName, o.Id as ownerId
                                        FROM Dog d
                                        JOIN Owner o on o.Id = d.OwnerId
                                        ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Dog> dogs = new List<Dog>();
                        while (reader.Read())
                        {
                            Dog dog = new Dog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("dogId")),
                                Name = reader.GetString(reader.GetOrdinal("dogName")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("ownerId")),
                                Owner = new Owner
                                {
                                    Name = reader.GetString(reader.GetOrdinal("ownerName"))
                                },
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null :
                                reader.GetString(reader.GetOrdinal("Notes")),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null :
                                reader.GetString(reader.GetOrdinal("ImageUrl"))
                            };
                            dogs.Add(dog);
                        }
                        return dogs;
                    }
                }
            }
        }

        public Dog GetDogById(int id)
        {
            Dog dog = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id as dogId, d.Name as dogName, Breed, Notes, ImageUrl, o.Name as ownerName, o.Id as ownerId
                                        FROM Dog d
                                        JOIN Owner o on o.Id = d.OwnerId
                                        where d.Id = @id
                                        ";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Dog newDog = new Dog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("dogId")),
                                Name = reader.GetString(reader.GetOrdinal("dogName")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("ownerId")),
                                Owner = new Owner
                                {
                                    Name = reader.GetString(reader.GetOrdinal("ownerName"))
                                },
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null :
                                reader.GetString(reader.GetOrdinal("Notes")),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null :
                                reader.GetString(reader.GetOrdinal("ImageUrl"))
                            };

                            return newDog;
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
            }
        }

        public void AddDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO DOG ([Name], Breed, OwnerId)
                                        OUTPUT INSERTED.ID
                                        VALUES (@name, @breed, @ownerId)
                                        ";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);

                    int id = (int)cmd.ExecuteScalar();

                    dog.Id = id;
                }
            }
        }

        public void UpdateDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Dog
                                        SET 
                                            [Name] = @name,
                                             Breed = @breed,
                                             Notes = @notes,
                                             ImageUrl = @imageurl,
                                             OwnerId = @ownerId
                                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@notes", dog.Notes);
                    cmd.Parameters.AddWithValue("@imageurl", dog.ImageUrl);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@id", dog.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteDog(int dogId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE
                                        FROM Dog
                                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", dogId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
