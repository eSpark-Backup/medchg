using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Services;
using MEDCHGClassLibrary.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Repository.User
{
    public class UserRepository : IUserRepository, IUsersChargeCode, IUsersDiagnosticCode, IUsersHospital
    {
        private string _npgsqlDataSource = null!;
        public string NpgsqlDataSource
        {
            get { return _npgsqlDataSource; }
            set { _npgsqlDataSource = value; }
        }
        private IConfiguration _configureOptions;
        private readonly MedChgContext _MedChgContext;
        public UserRepository(MedChgContext MedChgContext, IConfiguration configureOptions)
        {
            _configureOptions = configureOptions;
            _MedChgContext = MedChgContext;
            _npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection");
        }

        public async Task<Model.User> GetUserById(int id)
        {
            return await _MedChgContext.Users.SingleAsync(x => x.Id == id);
        }

        public async Task<Dictionary<int, string>> GetUsers()
        {
            return await _MedChgContext.Users.ToDictionaryAsync(x => x.Id, x => x.FullName);
        }

        public Task<List<UserDTO>> List()
        {
            throw new NotImplementedException();
        }

        public async Task<Model.User> SaveUser(UserDTO model)
        {
            var user = new Model.User()
            {
                FullName = model.FullName,
                UserRole = model.UserRole,
                UserName = model.UserName,
                Email = model.Email,
                ImgUrl = "/images/defaultavatar.jpg",
                Designation = model.Designation,
                Device = "Web",
                PhoneNumber = model.PhoneNumber,
                CreatedAt = TimeHelper.GetTexasTime(),
                CreatedById = model.CreatedAndUpdatedById,
                Date = TimeHelper.GetTexasTime(),
                Password = EncryptDecrypt.HashStringWithSalt(model.Password)
            };
            if (model.ProviderId != null)
            {
                user.ProviderId = model.ProviderId.Value;
            }
            if (model.Id > 0)
            {
                if (model.IsActive != 2)
                {
                    _MedChgContext.Users.Update(user);
                }
                else
                {
                    _MedChgContext.Users.Remove(user);
                }
            }
            else
            {
                user.UserId = new Guid().ToString();
                _MedChgContext.Users.Add(user);
            }
            
            await _MedChgContext.SaveChangesAsync();
            return user;
        }

        public async Task<Model.User> AddUserAsync(UserDTO model)
        {
            var user = new Model.User()
            {
                FullName = model.FullName,
                UserRole = model.UserRole,
                UserName = model.UserName,
                Email = model.Email,
                UserId = model.UserId,
                ImgUrl = "/images/defaultavatar.jpg",
                Designation = model.Designation,
                Device = "Web",
                PhoneNumber = model.PhoneNumber,
                CreatedAt = TimeHelper.GetTexasTime(),
                CreatedById = model.CreatedAndUpdatedById,
                Date = TimeHelper.GetTexasTime()
            };
            if (model.ProviderId != null)
            {
                user.ProviderId = model.ProviderId.Value;
            }
            _MedChgContext.Users.Add(user);
            await _MedChgContext.SaveChangesAsync();
            return user;
        }

        public async Task<Model.User> UpdateUserAsync(Model.User model)
        {
            try
            {
                var User = await _MedChgContext.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (User != null)
                {
                    User.UserName = model.UserName;
                    User.FullName = model.FullName;
                    User.Email = model.Email;
                    User.Designation = model.Designation;
                    User.PhoneNumber = model.PhoneNumber;
                    User.UserRole = model.UserRole;
                    User.UpdatedById = model.UpdatedById;
                    User.IsActive = model.IsActive;
                    User.ImgUrl = model.ImgUrl == null ? "/images/defaultavatar.jpg" : model.ImgUrl;
                    User.UpdatedAt = TimeHelper.GetTexasTime();
                    if (model.IsActive != 2)
                    {
                        await _MedChgContext.SaveChangesAsync();
                    }
                    else
                    {
                        _MedChgContext.Users.Remove(User);
                        await _MedChgContext.SaveChangesAsync();
                    }
                    return User;
                }
                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Model.User> UpdateUserStatusAsync(UserDTO model)
        {
            try
            {
                var User = await _MedChgContext.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (User != null)
                {
                    User.IsActive = model.IsActive ?? 0;
                    User.UpdatedAt = TimeHelper.GetTexasTime();
                    await _MedChgContext.SaveChangesAsync();
                    return User;
                }
                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Model.User> DeleteUserAsync(int UserId)
        {
            var User = await _MedChgContext.Users.Where(x => x.Id == UserId).FirstOrDefaultAsync();
            try
            {
                _MedChgContext.Users.Remove(User);
                await _MedChgContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return User;
        }
        public async Task<List<Model.User>> GetAllUsersAsync()
        {
            List<Model.User> users = new List<Model.User>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"Users\"";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Model.User user = new Model.User
                        {
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            IsActive = reader.GetInt32(reader.GetOrdinal("IsActive")),
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            UserRole = reader.GetString(reader.GetOrdinal("UserRole")),
                            UserId = reader.GetString(reader.GetOrdinal("UserId")),
                            ProviderId = !reader.IsDBNull(reader.GetOrdinal("ProviderId")) ? reader.GetInt32(reader.GetOrdinal("ProviderId")) : 0
                        };
                        int value = reader.GetOrdinal("Designation");
                        if (!reader.IsDBNull(value)) // Check if the value is not DBNull
                        {
                            string Designations = reader.GetString(value);
                            user.Designation = string.IsNullOrEmpty(Designations) ? "" : Designations;
                        }
                        else
                        {
                            user.Designation = ""; // Assign an empty string if the value is DBNull
                        }
                        users.Add(user);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return users;
        }
        public Task<Model.User> GetEmailUser(string UserEmail)
        {
            throw new NotImplementedException();
        }
        public Task<Model.User> UpdateUserPasswordAsync(byte[] password)
        {
            throw new NotImplementedException();
        }
        public async Task<UserDTO> GetUserByLogin(string userName, string providerName)
        {
            var Users = await _MedChgContext.Users.Where(x => x.UserName.ToLower() == userName.ToLower()).ToListAsync();
            foreach (var user in Users)
            {
                var provider = await _MedChgContext.Providers.Where(x => x.Name.ToLower() == providerName.ToLower() && x.IsActive == 1).FirstOrDefaultAsync();
                if (provider != null)
                {
                    if (provider.Id == user.ProviderId)
                    {
                        var data = new UserDTO()
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            UserName = user.UserName,
                            UserRole = user.UserRole,
                            IsActive = user.IsActive,
                            Designation = user.Designation,
                            Email = user.Email,
                            UserId = user.UserId,
                            ImgUrl = user.ImgUrl,
                            ProviderId = user.ProviderId,
                            ProviderName = provider.Name,
                        };
                        return data;
                    }
                }
            }
            return null;
        }
        public async Task<Model.User?> GetUserByProviderAndUsername(int providerId, string userName)
        {
            return await _MedChgContext.Users.Where(x=>x.UserName.ToLower() == userName && x.ProviderId == providerId && x.IsActive != 3).FirstOrDefaultAsync();
        }
        public async Task<Model.User?> GetUserByUsername(int providerId, string userName)
        {
            return await _MedChgContext.Users.Where(x => x.UserName == userName && x.ProviderId == providerId).FirstOrDefaultAsync();
        }
        public async Task<UserDTO> GetAdminByLogin(string userName)
        {
            var user = await _MedChgContext.Users.Where(x => x.UserName.ToLower() == userName).FirstOrDefaultAsync();
            if (user != null)
            {
                var data = new UserDTO()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    UserRole = user.UserRole,
                    Email = user.Email,
                    UserId = user.UserId,
                    IsActive = user.IsActive,
                    ImgUrl = user.ImgUrl,
                    ProviderId = user.ProviderId,
                };
                return data;
            }
            return null;
        }
        public async Task<List<UsersChargeCode>> AddUsersChargeCodeAsync(List<UsersChargeCode> model)
        {
            try
            {
                var chrgcds = _MedChgContext.UsersChargeCodes.ToList();
                foreach (var item in model)
                {
                    var chk = chrgcds.Exists(x => x.UserId == item.UserId && x.ChargeCodeId == item.ChargeCodeId);
                    if (chk)
                    {
                        var updt = chrgcds.Where(x => x.UserId == item.UserId && x.ChargeCodeId == item.ChargeCodeId).FirstOrDefault();
                        updt.UpdatedAt = TimeHelper.GetTexasTime();
                        updt.IsFavourite = item.IsFavourite;
                    }
                    else
                    {
                        var newDataAdded = _MedChgContext.UsersChargeCodes.Add(item).Entity;
                    }
                }
                await _MedChgContext.SaveChangesAsync();
                return model;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public Task<UsersChargeCode> UpdateUsersChargeCodeAsync(UsersChargeCode model)
        {
            throw new NotImplementedException();
        }
        public Task<UsersChargeCode> DeleteUsersChargeCodeAsync(int UsersChargeCodeId)
        {
            throw new NotImplementedException();
        }
        public Task<List<UsersChargeCode>> GetAllUsersChargeCodeAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<List<UsersChargeCode>> GetUsersChargeCodeByIdAsync(int UsersChargeCodeId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    var usr = new List<UsersChargeCode>();
                    string sqlQuery = "SELECT * FROM  \"UsersChargeCodes\" WHERE \"UserId\" = '" + UsersChargeCodeId + "'";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var usrchrgcd = new UsersChargeCode
                        {
                            UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? 0 : reader.GetInt32(reader.GetOrdinal("UserId")),
                            ChargeCodeId = reader.IsDBNull(reader.GetOrdinal("ChargeCodeId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ChargeCodeId")),
                            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive")),
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                            IsFavourite = reader.IsDBNull(reader.GetOrdinal("IsFavourite")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFavourite")),
                        };
                        usr.Add(usrchrgcd);
                        // Process nbCertByExercice...
                    }
                    reader.Close();
                    return usr;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<UsersDiagnosticCode>> AddUsersDiagnosticCodeAsync(List<UsersDiagnosticCode> model)
        {
            try
            {
                var dgnstcds = _MedChgContext.UsersDiagnosticCodes.ToList();
                foreach (var item in model)
                {
                    var chk = dgnstcds.Exists(x => x.UserId == item.UserId && x.DiagnosticCodeId == item.DiagnosticCodeId);
                    if (chk)
                    {
                        var updt = dgnstcds.Where(x => x.UserId == item.UserId && x.DiagnosticCodeId == item.DiagnosticCodeId).FirstOrDefault();
                        updt.UpdatedAt = TimeHelper.GetTexasTime();
                        updt.IsFavourite = item.IsFavourite;
                    }
                    else
                    {
                        var newDataAdded = _MedChgContext.UsersDiagnosticCodes.Add(item).Entity;
                    }
                }
                await _MedChgContext.SaveChangesAsync();
                return model;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public Task<UsersDiagnosticCode> UpdateUsersDiagnosticCodeAsync(UsersDiagnosticCode model)
        {
            throw new NotImplementedException();
        }
        public Task<UsersDiagnosticCode> DeleteUsersDiagnosticCodeAsync(int UsersDiagnosticCodeId)
        {
            throw new NotImplementedException();
        }
        public Task<List<UsersDiagnosticCode>> GetAllUsersDiagnosticCodeAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<List<UsersDiagnosticCode>> GetUsersDiagnosticCodeByIdAsync(int UsersDiagnosticCodeId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    var usr = new List<UsersDiagnosticCode>();
                    string sqlQuery = "SELECT * FROM  \"UsersDiagnosticCodes\" WHERE \"UserId\" = '" + UsersDiagnosticCodeId + "'";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var usrchrgcd = new UsersDiagnosticCode
                        {
                            UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? 0 : reader.GetInt32(reader.GetOrdinal("UserId")),
                            DiagnosticCodeId = reader.IsDBNull(reader.GetOrdinal("DiagnosticCodeId")) ? 0 : reader.GetInt32(reader.GetOrdinal("DiagnosticCodeId")),
                            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive")),
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                            IsFavourite = reader.IsDBNull(reader.GetOrdinal("IsFavourite")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFavourite")),
                        };
                        usr.Add(usrchrgcd);
                        // Process nbCertByExercice...
                    }
                    reader.Close();
                    return usr;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<UsersHospital>> AddUsersHospitalAsync(List<UsersHospital> model)
        {
            try
            {
                var hsptls = _MedChgContext.UsersHospitals.ToList();
                foreach (var item in model)
                {
                    var chk = hsptls.Exists(x => x.UserId == item.UserId && x.HospitalId == item.HospitalId);
                    if (chk)
                    {
                        var updt = hsptls.Where(x => x.UserId == item.UserId && x.HospitalId == item.HospitalId).FirstOrDefault();
                        updt.UpdatedAt = TimeHelper.GetTexasTime();
                        updt.IsFavourite = item.IsFavourite;
                    }
                    else
                    {
                        var newDataAdded = _MedChgContext.UsersHospitals.Add(item).Entity;
                    }
                }
                await _MedChgContext.SaveChangesAsync();
                return model;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task UpdateUsersHospitalAsync(UsersHospital model)
        {
            try
            {
                var hospital = _MedChgContext.Hospitals.Where(x => x.Id == model.HospitalId).FirstOrDefault();
                if(hospital != null)
                {
                    var UserHospital = _MedChgContext.UsersHospitals.Where(x => x.HospitalId == model.HospitalId && x.UserId == model.UserId).FirstOrDefault();
                    if(UserHospital != null)
                    {
                        UserHospital.IsFavourite = model.IsFavourite;
                        UserHospital.UpdatedAt = model.UpdatedAt;
                        UserHospital.UpdatedById = model.UpdatedById;
                        await _MedChgContext.SaveChangesAsync();
                    }
                    else
                    {
                        var userHospital = new UsersHospital()
                        {
                            HospitalId = hospital.Id,
                            UserId = model.UserId,
                            IsActive = 1,
                            IsFavourite = model.IsFavourite,
                            Device = "Web",
                            CreatedAt = TimeHelper.GetTexasTime(),
                            CreatedById = model.UserId,
                            Date = TimeHelper.GetTexasTime(),
                        };
                        _MedChgContext.UsersHospitals.Add(userHospital);
                        await _MedChgContext.SaveChangesAsync();
                    }
                }
                //hospital.IsFavourite = model.IsFavourite;
                //hospital.UpdatedAt = model.UpdatedAt;
                //hospital.UpdatedById = model.UpdatedById;
                //_MedChgContext.Update(hospital);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public Task<UsersHospital> DeleteUsersHospitalAsync(int UsersHospitalId)
        {
            throw new NotImplementedException();
        }
        public Task<List<UsersHospital>> GetAllUsersHospitalAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<List<UsersHospital>> GetUsersHospitalByIdAsync(int UsersHospitalId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    var usr = new List<UsersHospital>();
                    string sqlQuery = "SELECT * FROM  \"UsersHospitals\" WHERE \"UserId\" = '" + UsersHospitalId + "'";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var usrchrgcd = new UsersHospital
                        {
                            UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? 0 : reader.GetInt32(reader.GetOrdinal("UserId")),
                            HospitalId = reader.IsDBNull(reader.GetOrdinal("HospitalId")) ? 0 : reader.GetInt32(reader.GetOrdinal("HospitalId")),
                            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive")),
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                            IsFavourite = reader.IsDBNull(reader.GetOrdinal("IsFavourite")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFavourite")),
                        };
                        usr.Add(usrchrgcd);
                        // Process nbCertByExercice...
                    }
                    reader.Close();
                    return usr;
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }
        public async Task<ResponseMessage> UpdateAllUserAsync(List<Model.User> model)
        {
            try
            {
                using (_MedChgContext)
                {
                    foreach (var user in model)
                    {
                        var existingUser = await _MedChgContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                        if (existingUser != null)
                        {
                            existingUser.UpdatedAt = TimeHelper.GetTexasTime();
                            existingUser.UpdatedById = user.UpdatedById;
                            existingUser.IsActive = 2;
                        }
                    }
                    await _MedChgContext.SaveChangesAsync();
                }
                return new ResponseMessage
                {
                    Success = true,
                    Message = "User deleted successfully",
                    Data = model
                };
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "SERVER_ERROR",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<Model.User> UpdateProfilePicture(Model.User model)
        {
            try
            {
                var User = await _MedChgContext.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (User != null)
                {
                    User.ImgUrl = model.ImgUrl == null ? "/images/defaultavatar.jpg" : model.ImgUrl;
                    User.UpdatedAt = TimeHelper.GetTexasTime();
                    if (model.IsActive != 2)
                    {
                        await _MedChgContext.SaveChangesAsync();
                    }
                    else
                    {
                        _MedChgContext.Users.Remove(User);
                        await _MedChgContext.SaveChangesAsync();
                    }
                    return User;
                }
                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<Model.User> RemoveProfilePicture(Model.User model)
        {
            try
            {
                var User = await _MedChgContext.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (User != null)
                {
                    User.ImgUrl = null;
                    User.UpdatedAt = TimeHelper.GetTexasTime();
                    if (model.IsActive != 2)
                    {
                        await _MedChgContext.SaveChangesAsync();
                    }
                    else
                    {
                        _MedChgContext.Users.Remove(User);
                        await _MedChgContext.SaveChangesAsync();
                    }
                    return User;
                }
                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<Model.User>> GetAllUsersByProviderIdAsync(int Id)
        {
            List<Model.User> users = new List<Model.User>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = $@"SELECT * FROM ""Users"" WHERE ""Users"".""ProviderId""={Id}";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Model.User user = new Model.User
                        {
                            UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserName")),
                            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive")),
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                            FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FullName")),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                            UserRole = reader.IsDBNull(reader.GetOrdinal("UserRole")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserRole")),
                            UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserId")),
                            ProviderId = reader.IsDBNull(reader.GetOrdinal("ProviderId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProviderId"))
                        };

                        int value = reader.GetOrdinal("Designation");
                        if (!reader.IsDBNull(value)) // Check if the value is not DBNull
                        {
                            string Designations = reader.GetString(value);
                            user.Designation = string.IsNullOrEmpty(Designations) ? string.Empty : Designations;
                        }
                        else
                        {
                            user.Designation = string.Empty; // Assign an empty string if the value is DBNull
                        }

                        users.Add(user);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return users;
        }

        public Task<UserDTO> GetUserByUsername(string userName, string providerName)
        {
            throw new NotImplementedException();
        }
    }
}
