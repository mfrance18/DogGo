//using DogGo.Models;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
//using System.Collections.Generic;

//namespace DogGo.Repositories
//{
//    public class WalkRepository
//    {
//        private readonly IConfiguration _config;

//        public WalkRepository(IConfiguration config)
//        {
//            _config = config;
//        }

//        public SqlConnection Connection
//        {
//            get
//            {
//                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
//            }
//        }

//        public List<Walk> GetWalksByWalkerId(int walkerId)
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open()
//                using(SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText @""
//                }    
//            }
//        }
//    }
//}
