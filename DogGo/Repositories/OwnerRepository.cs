﻿using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;


namespace DogGo.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public OwnerRepository(IConfiguration config)
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

        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Owner.Id as ownerId, Owner.Name as ownerName, n.Name as neighborhoodName, Address, Email, Phone
                        FROM Owner
                        join Neighborhood n on n.Id = Owner.NeighborhoodId
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Owner> owners = new List<Owner>();
                        while (reader.Read())
                        {
                            Owner owner = new Owner
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ownerId")),
                                Name = reader.GetString(reader.GetOrdinal("ownerName")),
                                Neighborhood = new Neighborhood
                                {
                                    Name = reader.GetString(reader.GetOrdinal("neighborhoodName"))
                                },
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone"))
                            };

                            owners.Add(owner);
                        }

                        return owners;
                    }
                }
            }
        }

        public Owner GetOwnerById(int id)
        {

            Owner owner = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select o.*, d.Id DogID, d.Name DogName, d.Breed Breed, d.Notes, d.ImageUrl, o.Id as ownerId, o.Name as ownerName, n.Name as neighborhoodName, Address, Email, Phone
                                        From owner o
                                        left join Dog d on d.OwnerId = o.Id
                                        join Neighborhood n on n.Id = o.NeighborhoodId
                                        where o.Id =  @id";

                    

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (owner == null)
                            {
                                owner = new Owner
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ownerId")),
                                    Name = reader.GetString(reader.GetOrdinal("ownerName")),
                                    Neighborhood = new Neighborhood
                                    {
                                        Name = reader.GetString(reader.GetOrdinal("neighborhoodName"))
                                    },
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                    Dogs = new List<Dog>()
                                };

                                                             
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("DogId")))
                            {
                                owner.Dogs.Add(new Dog
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                    Name = reader.GetString(reader.GetOrdinal("DogName")),
                                    Breed = reader.GetString(reader.GetOrdinal("Breed"))
                                });
                            }
                        }
                        return owner;
                    }
                   
                }
            }
        }
    }
}
