using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MEDCHGClassLibrary.Repository.ProviderRepository
{
    public class ProviderRepository : IProviderRepository
    {
        private string _npgsqlDataSource;

        public string NpgsqlDataSource
        {
            get { return _npgsqlDataSource; }
            set { _npgsqlDataSource = value; }
        }

        private IConfiguration _configureOptions;

        private readonly MedChgContext _MedChgContext;
        public ProviderRepository(MedChgContext MedChgContext, IConfiguration configureOptions)
        {
            _MedChgContext = MedChgContext;
            _configureOptions = configureOptions;
            _npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection");
        }
        public async Task<Provider> AddProviderAsync(Provider model)
        {
            try
            {
                model.Name.Trim().ToLower();
                var newDataAdded = _MedChgContext.Providers.Add(model).Entity;
                await _MedChgContext.SaveChangesAsync();
                return newDataAdded;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Task<Provider> DeleteProviderAsync(int ProviderId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ChargeCode>> GetAllChargeCodesByProviderId(int id)
        {
            List<ChargeCode> chargecodes = new List<ChargeCode>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"ChargeCodes\" WHERE \"IsActive\" = 1 AND \"ProviderId\"=" + id;
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        ChargeCode chargecode = new ChargeCode
                        {
                            // Map database columns to ApplicationUser properties
                            ChargeCodeName = reader.IsDBNull(reader.GetOrdinal("ChargeCodeName")) ? string.Empty : reader.GetString(reader.GetOrdinal("ChargeCodeName")),
                            ChargeCodeDescription = reader.IsDBNull(reader.GetOrdinal("ChargeCodeDescription")) ? string.Empty : reader.GetString(reader.GetOrdinal("ChargeCodeDescription")),
                            IsFavourite = reader.IsDBNull(reader.GetOrdinal("IsFavourite")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFavourite")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive")),
                            ProviderId = reader.IsDBNull(reader.GetOrdinal("ProviderId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProviderId")),
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                            // Add other properties as needed
                        };
                        chargecodes.Add(chargecode);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return chargecodes;
        }

        public async Task<List<DiagnosticCode>> GetAllDiagnosticCodesByProviderId(int id)
        {
            List<DiagnosticCode> diagnosticcodes = new List<DiagnosticCode>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"DiagnosticCodes\" WHERE \"IsActive\" = 1 AND \"ProviderId\"=" + id;
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        DiagnosticCode diagnosticcode = new DiagnosticCode
                        {
                            // Map database columns to ApplicationUser properties
                            DiagnosticCodeName = reader.IsDBNull(reader.GetOrdinal("DiagnosticCodeName")) ? string.Empty : reader.GetString(reader.GetOrdinal("DiagnosticCodeName")),
                            DiagnosticCodeDescription = reader.IsDBNull(reader.GetOrdinal("DiagnosticCodeDescription")) ? string.Empty : reader.GetString(reader.GetOrdinal("DiagnosticCodeDescription")),
                            IsFavourite = reader.IsDBNull(reader.GetOrdinal("IsFavourite")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFavourite")),
                            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive")),
                            ProviderId = reader.IsDBNull(reader.GetOrdinal("ProviderId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProviderId")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id"))
                            // Add other properties as needed
                        };
                        diagnosticcodes.Add(diagnosticcode);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return diagnosticcodes;
        }


        public async Task<List<Model.Hospital>> GetAllHospitalsByProviderId(int id)
        {
            List<Model.Hospital> hospitals = new List<Model.Hospital>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"Hospitals\" where \"ProviderId\" =" + id;
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Model.Hospital hospital = new Model.Hospital
                        {
                            // Map database columns to ApplicationUser properties
                            HospitalName = reader.IsDBNull(reader.GetOrdinal("HospitalName")) ? string.Empty : reader.GetString(reader.GetOrdinal("HospitalName")),
                            IsFavourite = reader.IsDBNull(reader.GetOrdinal("IsFavourite")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFavourite")),
                            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive")),
                            ProviderId = reader.IsDBNull(reader.GetOrdinal("ProviderId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProviderId")),
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                            // Uncomment and add other properties as needed
                            // UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                        };
                        hospitals.Add(hospital);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return hospitals;
        }


        public async Task<List<Provider>> GetAllProviderAsync()
        {
            List<Provider> providers = new List<Provider>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"Providers\" where \"IsActive\" IN (1, 0)";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Provider provider = new Provider
                        {
                            // Map database columns to ApplicationUser properties
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                            SubscriptionId = reader.IsDBNull(reader.GetOrdinal("SubscriptionId")) ? string.Empty : reader.GetString(reader.GetOrdinal("SubscriptionId")),
                            Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                            DisplayName = reader.IsDBNull(reader.GetOrdinal("DisplayName")) ? string.Empty : reader.GetString(reader.GetOrdinal("DisplayName")),
                            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive")),
                            // Add other properties as needed
                        };
                        providers.Add(provider);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return providers;
        }


        public Task<Provider> GetAllProviderAsyncLast()
        {
            throw new NotImplementedException();
        }
        public async Task<List<Model.User>> GetAllUsersByProviderId(int id)
        {
            List<Model.User> users = new List<Model.User>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"Users\"where \"ProviderId\"=" + id + " AND \"IsActive\" != 2;";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Model.User user = new Model.User
                        {
                            // Map database columns to ApplicationUser properties
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                            ProviderId = reader.IsDBNull(reader.GetOrdinal("ProviderId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProviderId")),
                            Designation = reader.IsDBNull(reader.GetOrdinal("Designation")) ? string.Empty : reader.GetString(reader.GetOrdinal("Designation")),
                            UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserName")),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                            UserRole = reader.IsDBNull(reader.GetOrdinal("UserRole")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserRole")),
                            UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserId")),
                            FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FullName")),
                            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive")),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            ImgUrl = reader.IsDBNull(reader.GetOrdinal("ImgUrl")) ? string.Empty : reader.GetString(reader.GetOrdinal("ImgUrl"))
                            // Add other properties as needed
                        };
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

        public async Task<Provider> GetProviderByIdAsync(int ProviderId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {
                    Provider prvder = new Provider { };
                    string sqlQuery = "SELECT * FROM  \"Providers\" WHERE \"Id\" = '" + ProviderId + "'";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        prvder.Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id"));
                        prvder.SubscriptionId = reader.IsDBNull(reader.GetOrdinal("SubscriptionId")) ? string.Empty : reader.GetString(reader.GetOrdinal("SubscriptionId"));
                        prvder.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader["Name"].ToString();
                        prvder.DisplayName = reader.IsDBNull(reader.GetOrdinal("DisplayName")) ? string.Empty : reader["DisplayName"].ToString();
                        prvder.IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsActive"));
                        // Process nbCertByExercice...
                    }
                    reader.Close();
                    return prvder;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

		public async Task<Provider> GetProviderUsersAsync(int providerId)
		{
			try
			{
				using (var connection = new NpgsqlConnection(_npgsqlDataSource))
				{
					await connection.OpenAsync();

					string sql = @"
                                   SELECT 
                                       p.""Id"" AS ""ProviderId"",
                                       p.""Name"" AS ""ProviderName"",
                                       p.""DisplayName"" AS ""ProviderDisplayName"",
                                       p.""SubscriptionId"",
                                       p.""IsActive"" AS ""ProviderIsActive"",
                                       p.""CreatedAt"" AS ""ProviderCreatedAt"",
                                       p.""Date"" AS ""ProviderDate"",
                                       p.""CreatedById"" AS ""ProviderCreatedById"",
                                       p.""UpdatedAt"" AS ""ProviderUpdatedAt"",
                                       p.""UpdatedById"" AS ""ProviderUpdatedById"",
                                       p.""Device"" AS ""ProviderDevice"",
                                       
                                       u.""Id"" AS ""UserId"",
                                       u.""FullName"" AS ""UserFullName"",
                                       u.""UserName"" AS ""UserUserName"",
                                       u.""Email"" AS ""UserEmail"",
                                       u.""UserId"" AS ""UserUserId"",
                                       u.""ImgUrl"" AS ""UserImgUrl"",
                                       u.""PhoneNumber"" AS ""UserPhoneNumber"",
                                       u.""Designation"" AS ""UserDesignation"",
                                       u.""UserRole"" AS ""UserUserRole"",
                                       u.""IsActive"" AS ""UserIsActive"",
                                       u.""CreatedAt"" AS ""UserCreatedAt"",
                                       u.""Date"" AS ""UserDate"",
                                       u.""CreatedById"" AS ""UserCreatedById"",
                                       u.""UpdatedAt"" AS ""UserUpdatedAt"",
                                       u.""UpdatedById"" AS ""UserUpdatedById"",
                                       u.""Device"" AS ""UserDevice"",
                                       u.""Password"" AS ""UserPassword""
                                   FROM ""Providers"" p
                                   LEFT JOIN ""Users"" u ON p.""Id"" = u.""ProviderId""
                                   WHERE p.""Id"" = @ProviderId;";

					using (var command = new NpgsqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@ProviderId", providerId);

						using (var reader = await command.ExecuteReaderAsync())
						{
							Provider provider = null;
							List<Model.User> users = new List<Model.User>();

							while (await reader.ReadAsync())
							{
								// Initialize provider if it hasn't been done yet (first row will contain provider data)
								if (provider == null)
								{
									provider = new Provider
									{
										Id = reader.GetInt32(reader.GetOrdinal("ProviderId")),
										Name = reader.GetString(reader.GetOrdinal("ProviderName")),
										DisplayName = reader.IsDBNull(reader.GetOrdinal("ProviderDisplayName")) ? null : reader.GetString(reader.GetOrdinal("ProviderDisplayName")),
										SubscriptionId = reader.IsDBNull(reader.GetOrdinal("SubscriptionId")) ? null : reader.GetString(reader.GetOrdinal("SubscriptionId")),
										IsActive = reader.GetInt32(reader.GetOrdinal("ProviderIsActive")),
										CreatedAt = reader.GetDateTime(reader.GetOrdinal("ProviderCreatedAt")),
										Date = reader.IsDBNull(reader.GetOrdinal("ProviderDate")) ? TimeHelper.GetTexasTime() : reader.GetDateTime(reader.GetOrdinal("ProviderDate")),
										CreatedById = reader.IsDBNull(reader.GetOrdinal("ProviderCreatedById")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ProviderCreatedById")),
										UpdatedAt = reader.IsDBNull(reader.GetOrdinal("ProviderUpdatedAt")) ? TimeHelper.GetTexasTime() : reader.GetDateTime(reader.GetOrdinal("ProviderUpdatedAt")),
										UpdatedById = reader.IsDBNull(reader.GetOrdinal("ProviderUpdatedById")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ProviderUpdatedById")),
										Device = reader.IsDBNull(reader.GetOrdinal("ProviderDevice")) ? null : reader.GetString(reader.GetOrdinal("ProviderDevice"))
									};
								}

								// Map User data if it exists in the current row
								if (!reader.IsDBNull(reader.GetOrdinal("UserId")))
								{
									var user = new Model.User
									{
										Id = reader.GetInt32(reader.GetOrdinal("UserId")),
										FullName = reader.GetString(reader.GetOrdinal("UserFullName")),
										UserName = reader.GetString(reader.GetOrdinal("UserUserName")),
										Email = reader.IsDBNull(reader.GetOrdinal("UserEmail")) ? null : reader.GetString(reader.GetOrdinal("UserEmail")),
										UserId = reader.GetString(reader.GetOrdinal("UserUserId")),
										ImgUrl = reader.IsDBNull(reader.GetOrdinal("UserImgUrl")) ? null : reader.GetString(reader.GetOrdinal("UserImgUrl")),
										PhoneNumber = reader.IsDBNull(reader.GetOrdinal("UserPhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("UserPhoneNumber")),
										Designation = reader.IsDBNull(reader.GetOrdinal("UserDesignation")) ? null : reader.GetString(reader.GetOrdinal("UserDesignation")),
										UserRole = reader.IsDBNull(reader.GetOrdinal("UserUserRole")) ? null : reader.GetString(reader.GetOrdinal("UserUserRole")),
										IsActive = reader.GetInt32(reader.GetOrdinal("UserIsActive")),
										CreatedAt = reader.GetDateTime(reader.GetOrdinal("UserCreatedAt")),
										Date = reader.IsDBNull(reader.GetOrdinal("UserDate")) ? TimeHelper.GetTexasTime() : reader.GetDateTime(reader.GetOrdinal("UserDate")),
										CreatedById = reader.IsDBNull(reader.GetOrdinal("UserCreatedById")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UserCreatedById")),
										UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UserUpdatedAt")) ? TimeHelper.GetTexasTime() : reader.GetDateTime(reader.GetOrdinal("UserUpdatedAt")),
										UpdatedById = reader.IsDBNull(reader.GetOrdinal("UserUpdatedById")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UserUpdatedById")),
										Device = reader.IsDBNull(reader.GetOrdinal("UserDevice")) ? null : reader.GetString(reader.GetOrdinal("UserDevice")),
										Password = reader.IsDBNull(reader.GetOrdinal("UserPassword")) ? null : reader.GetString(reader.GetOrdinal("UserPassword"))
									};

									users.Add(user);
								}
							}

							if (provider != null)
							{
								provider.User = users;  // Set the list of users to the provider object
							}

							return provider;  // Return the provider with the list of users
						}
					}
				}

				}
			catch (Exception ex)
			{
				// Log the exception
				throw new ApplicationException("An error occurred while fetching the provider and users.", ex);
			}
		}

		public async Task<Provider> UpdateProviderStatusAsync(Provider model)
        {
            try
            {
                var newDataAdded = _MedChgContext.Providers.Where(x => x.Id == model.Id).First();
                newDataAdded.UpdatedAt = TimeHelper.GetTexasTime();
                newDataAdded.IsActive = model.IsActive;
                newDataAdded.UpdatedById = 1;
                if (model.IsActive != 2)
                {
                    await _MedChgContext.SaveChangesAsync();
                }
                else
                {
                    var DeletedData = _MedChgContext.Providers.Where(x=>x.Id==model.Id)
                    .Include(p => p.Hospitals)
                    .Include(p => p.User)
                    .Include(p => p.ChargeCode)
                    .Include(p => p.DiagnosticCode)
                    .FirstOrDefault();
                    if (DeletedData != null)
                    {
                        DeletedData.IsActive = 3;
                        foreach (var hospital in DeletedData.Hospitals)
                        {
                            hospital.IsActive = 3;
                        }
                        foreach (var user in DeletedData.User)
                        {
                            user.IsActive = 3;
                        }
                        foreach (var chargeCode in DeletedData.ChargeCode)
                        {
                            chargeCode.IsActive = 3;
                        }
                        foreach (var diagnosticCode in DeletedData.DiagnosticCode)
                        {
                            diagnosticCode.IsActive = 3;
                        }
                    }
                    await _MedChgContext.SaveChangesAsync();
                    return null;
                }
                return newDataAdded;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ProviderTimerSetting> GetTimerSettings(int id)
        {
            ProviderTimerSetting providerTimerSetting = new ProviderTimerSetting();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"ProviderTimerSettings\" where \"ProviderId\"=" + id;
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        providerTimerSetting = new ProviderTimerSetting
                        {
                            // Map database columns to ApplicationUser properties
                            GreenInDays = reader.IsDBNull(reader.GetOrdinal("GreenInDays")) ? string.Empty : reader.GetString(reader.GetOrdinal("GreenInDays")),
                            YellowInDays = reader.IsDBNull(reader.GetOrdinal("YellowInDays")) ? string.Empty : reader.GetString(reader.GetOrdinal("YellowInDays")),
                            ProviderId = reader.IsDBNull(reader.GetOrdinal("ProviderId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProviderId")),
                            Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                            // Add other properties as needed
                        };
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return providerTimerSetting;
        }
        public async Task<ProviderTimerSetting> SaveTimerSettings(ProviderTimerSetting model)
        {
            ProviderTimerSetting? newDataAdded = new ProviderTimerSetting();
            try
            {
                newDataAdded = _MedChgContext.ProviderTimerSettings.Where(x => x.ProviderId == model.ProviderId).FirstOrDefault();
                if (newDataAdded != null)
                {
                    newDataAdded.GreenInDays = model.GreenInDays;
                    newDataAdded.YellowInDays = model.YellowInDays;
                    newDataAdded.RedInDays = model.RedInDays;
                    newDataAdded.IsActive = model.IsActive;
                    newDataAdded.UpdatedById = 1;
                    newDataAdded.UpdatedAt = TimeHelper.GetTexasTime();
                    newDataAdded = _MedChgContext.ProviderTimerSettings.Update(newDataAdded).Entity;
                }
                else
                {
                    newDataAdded = _MedChgContext.ProviderTimerSettings.Add(model).Entity;
                }
                await _MedChgContext.SaveChangesAsync();
                return newDataAdded;
            }
            catch (Exception e)
            {

            }
            return newDataAdded;
        }
    }
}
