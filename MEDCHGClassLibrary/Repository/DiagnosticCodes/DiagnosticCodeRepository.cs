using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository.DiagnosticCodes;
using MEDCHGClassLibrary.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Linq.Expressions;

namespace MEDCHGClassLibrary.Repository.Implementation
{
    public class DiagnosticCodeRepository : IDiagnosticCodeRepository
    {
        private string _npgsqlDataSource;

        public string NpgsqlDataSource
        {
            get { return _npgsqlDataSource; }
            set { _npgsqlDataSource = value; }
        }
        private IConfiguration _configureOptions;

        private readonly MedChgContext _MedChgContext;
        public DiagnosticCodeRepository(MedChgContext MedChgContext, IConfiguration configureOptions)
        {
            _MedChgContext = MedChgContext;
            _configureOptions = configureOptions;
            _npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection");
        }
        public async Task AddDiagnosticCodeAsync(List<DiagnosticCode> model)
        {
            try
            {
                await _MedChgContext.DiagnosticCodes.AddRangeAsync(model);
                await _MedChgContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> DeleteDiagnosticCodeAsync(int DiagnosticCodeId, string UserId)
        {
            var DC = await _MedChgContext.DiagnosticCodes.FindAsync(DiagnosticCodeId);
            if (DC == null)
            {
                return false;
            }
            DC.UpdatedAt = TimeHelper.GetTexasTime();
            DC.UpdatedById = int.Parse(UserId);
            DC.IsActive = 2;
            await _MedChgContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<DiagnosticCode>> GetAllDiagnosticCodeAsync()
        {
            List<DiagnosticCode> diagnosticcodes = new List<DiagnosticCode>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"DiagnosticCodes\" WHERE \"IsActive\" = 1";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        DiagnosticCode diagnosticcode = new DiagnosticCode
                        {
                            // Map database columns to ApplicationUser properties
                            DiagnosticCodeName = reader.GetString(reader.GetOrdinal("DiagnosticCodeName")),
                            DiagnosticCodeDescription = reader.IsDBNull(reader.GetOrdinal("DiagnosticCodeDescription")) ? null : reader.GetString(reader.GetOrdinal("DiagnosticCodeDescription")),
                            IsFavourite = reader.GetBoolean(reader.GetOrdinal("IsFavourite")),
                            IsActive = reader.GetInt32(reader.GetOrdinal("IsActive")),
                            ProviderId = reader.GetInt32(reader.GetOrdinal("ProviderId")),
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
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

        public Task<DiagnosticCode> GetDiagnosticCodeByIdAsync(DiagnosticCode DiagnosticCodeId)
        {
            throw new NotImplementedException();
        }

        public Task<DiagnosticCode> UpdateDiagnosticCodeAsync(DiagnosticCode model)
        {
            throw new NotImplementedException();
        }
        public async Task<DiagnosticCode> GetDiagnosticCodeById(Expression<Func<DiagnosticCode, bool>> where)
        {
            var DiagnosticCode = await _MedChgContext.DiagnosticCodes.Where(where).FirstOrDefaultAsync();
            return DiagnosticCode;
        }
        public async Task UpdateDiagnosticCodeByIdAsync(int id, bool fav, int UpdatedById)
        {
            try
            {
                var DiagnosticCode = _MedChgContext.DiagnosticCodes.First(x => x.Id == id);
                DiagnosticCode.UpdatedAt = TimeHelper.GetTexasTime();
                DiagnosticCode.UpdatedById = UpdatedById;
                DiagnosticCode.IsFavourite = fav;
                await _MedChgContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Handle the exception or log it
            }
        }
    }
}
