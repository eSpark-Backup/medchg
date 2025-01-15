using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository.ChargeCodes;
using MEDCHGClassLibrary.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Linq;
using System.Linq.Expressions;

namespace MEDCHGClassLibrary.Repository.Implementation
{
    public class ChargeCodeRepository : IChargeCodeRepository
    {
        private string _npgsqlDataSource;

        public string NpgsqlDataSource
        {
            get { return _npgsqlDataSource; }
            set { _npgsqlDataSource = value; }
        }
        private IConfiguration _configureOptions;

        private readonly MedChgContext _MedChgContext;
        public ChargeCodeRepository(MedChgContext MedChgContext, IConfiguration configureOptions)
        {
            _MedChgContext = MedChgContext;
            _configureOptions = configureOptions;
            _npgsqlDataSource = _configureOptions.GetConnectionString("PostgreConnection");
        }

        public async Task AddChargeCodeAsync(List<ChargeCode> model)
        {
            try
            {
                await _MedChgContext.ChargeCodes.AddRangeAsync(model);
                await _MedChgContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> DeleteChargeCodeAsync(int ChargeCodeId, string UserId)
        {
            var CC = await _MedChgContext.ChargeCodes.FindAsync(ChargeCodeId);
            if (CC == null)
            {
                return false ;
            }
            CC.UpdatedAt = TimeHelper.GetTexasTime();
            CC.UpdatedById = int.Parse(UserId);
            CC.IsActive = 2;
            await _MedChgContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<ChargeCode>> GetAllChargeCodeAsync()
        {
            List<ChargeCode> chargecodes = new List<ChargeCode>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_npgsqlDataSource))
                {

                    string sqlQuery = "SELECT * FROM \"ChargeCodes\" WHERE \"IsActive\" = 1";
                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        ChargeCode chargecode = new ChargeCode
                        {
                            // Map database columns to ApplicationUser properties
                            ChargeCodeName = reader.GetString(reader.GetOrdinal("ChargeCodeName")),
                            ChargeCodeDescription = reader.IsDBNull(reader.GetOrdinal("ChargeCodeDescription")) ? null : reader.GetString(reader.GetOrdinal("ChargeCodeDescription")),
                            IsFavourite = reader.GetBoolean(reader.GetOrdinal("IsFavourite")),
                            IsActive = reader.GetInt32(reader.GetOrdinal("IsActive")),
                            ProviderId = reader.GetInt32(reader.GetOrdinal("ProviderId")),
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
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

        public Task<ChargeCode> GetChargeCodeByIdAsync(ChargeCode ChargeCodeId)
        {
            throw new NotImplementedException();
        }

        public Task<ChargeCode> UpdateChargeCodeAsync(ChargeCode model)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateChargeCodeByIdAsync(int id, bool fav, int UpdatedById)
        {
            try
            {
                var ChargeCode = _MedChgContext.ChargeCodes.First(x => x.Id == id);
                ChargeCode.UpdatedAt = TimeHelper.GetTexasTime();
                ChargeCode.UpdatedById = UpdatedById;
                ChargeCode.IsFavourite = fav;
                await _MedChgContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Handle the exception or log it
            }
        }

        public async Task<ChargeCode> GetChargeCodeById(Expression<Func<ChargeCode, bool>> where)
        {
            var ChargeCode = await _MedChgContext.ChargeCodes.Where(where).FirstOrDefaultAsync();
            return ChargeCode;
        }
    }
}
