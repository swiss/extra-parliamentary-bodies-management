using Bk.APG.Business.Dtos;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.Infrastructure.DataSource.Migration.Mapping;
using Bk.APG.Infrastructure.DataSource.Migration.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public class AddressService : IAddressService
{
    private readonly ILogger<AddressService> _logger;
    private readonly DateTime _now = DateTime.UtcNow;
    private readonly IAddressRepository _addressRepository;
    private readonly IPostService _postService;

    public AddressService(IPostService postService, IAddressRepository addressRepository, DataContext dataContext, ILogger<AddressService> logger)
    {
        _postService = postService;
        _addressRepository = addressRepository;
        _logger = logger;
    }

    public void MigrateAddresses(SqlConnection connection)
    {
        _logger.LogInformation("Start migrating addresses.");

        var commandText = "SELECT a.*, p.*, k.Guid as KantonGuid FROM Adresse a " +
                          "INNER JOIN PersonAddressMapping p ON p.AddressID = a.Id " +
                          "LEFT JOIN Kanton k ON k.Kanton = a.Kanton " +
                          "WHERE PersId is not null " +
                          "AND ((Strasse is not null AND Strasse <> '') OR " +
                          "(LänderCode is not null AND LänderCode <> '') OR " +
                          "(Plz is not null AND Plz <> '') OR " +
                          "(Ort is not null AND Ort <> '') OR " +
                          "(a.Kanton is not null AND a.Kanton <> '') OR " +
                          "(Mobile is not null AND Mobile <> '') OR " +
                          "(Telefon is not null AND Telefon <> '') OR " +
                          "(Email is not null AND Email <> '') OR " +
                          "(Firma is not null AND Firma <> '') OR " +
                          "(Postfach is not null AND Postfach <> '') OR " +
                          "(PlzZusatz is not null AND PlzZusatz <> '') " +
                          ") ";

        using var command = new SqlCommand(commandText, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var adresse = new Adresse();

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("Id")).ToString(), out var validNumber))
            {
                adresse.Id = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("PersId")).ToString(), out validNumber))
            {
                adresse.PersId = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("IdPlzVerzeichnis")).ToString(), out validNumber))
            {
                adresse.IdPlzVerzeichnis = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("PlzZusatz")).ToString(), out validNumber))
            {
                adresse.PlzZusatz = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("OriginalPER_ANSCHRIFT")).ToString(), out validNumber))
            {
                adresse.OriginalPER_ANSCHRIFT = validNumber;
            }

            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("AdrGeschäftlich")).ToString(), out var validBoolean))
            {
                adresse.AdrGeschäftlich = validBoolean;
            }

            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("Anschriftadresse")).ToString(), out validBoolean))
            {
                adresse.Anschriftadresse = validBoolean;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("InsertDate")).ToString(), out var validDate))
            {
                adresse.InsertDate = validDate.ToUniversalTime();
            }
            else
            {
                adresse.InsertDate = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("UpdateDate")).ToString(), out validDate))
            {
                adresse.UpdateDate = validDate.ToUniversalTime();
            }
            else
            {
                adresse.UpdateDate = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("Histo")).ToString(), out validDate))
            {
                adresse.Histo = validDate.ToUniversalTime();
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("Guid")).ToString(), out var validGuid))
            {
                adresse.Guid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("KantonGuid")).ToString(), out validGuid))
            {
                adresse.KantonGuid = validGuid;
            }
            else
            {
                adresse.KantonGuid = null;
            }

            adresse.Strasse = reader.GetValue(reader.GetOrdinal("Strasse")).ToString();
            adresse.LänderCode = reader.GetValue(reader.GetOrdinal("LänderCode")).ToString();
            adresse.Plz = reader.GetValue(reader.GetOrdinal("Plz")).ToString();
            adresse.Ort = reader.GetValue(reader.GetOrdinal("Ort")).ToString();
            adresse.Kanton = reader.GetValue(reader.GetOrdinal("Kanton")).ToString();
            adresse.Email = reader.GetValue(reader.GetOrdinal("Email")).ToString();
            adresse.Telefon = reader.GetValue(reader.GetOrdinal("Telefon")).ToString();
            adresse.Mobile = reader.GetValue(reader.GetOrdinal("Mobile")).ToString();
            adresse.Fax = reader.GetValue(reader.GetOrdinal("Fax")).ToString();
            adresse.Firma = reader.GetValue(reader.GetOrdinal("Firma")).ToString();
            adresse.Postfach = reader.GetValue(reader.GetOrdinal("Postfach")).ToString();
            adresse.LastupdateUser = reader.GetValue(reader.GetOrdinal("LastupdateUser")).ToString();

            var address = MigrationMapping.ToAddress(adresse);

            _addressRepository.CreateForMigration(address);
        }

        _logger.LogInformation("Addresses migrated completely.");
    }

    public async Task VerifyAddresses()
    {
        _logger.LogInformation("Now verifying person addresses with PostService.");

        var addresses = await _addressRepository.GetAllUnverifiedAddresses();
        var countValidations = 0;
        var countInvalid = 0;
        var countAmbiguous = 0;

        foreach (var address in addresses)
        {
            if (!string.IsNullOrEmpty(address.City) && !string.IsNullOrEmpty(address.Zip))
            {
                countValidations++;

                var dto = new AddressSearchDto { City = address.City, Zip = address.Zip, Street = address.Street };
                var result = await _postService.Verify(dto);

                if (result.Status == AddressVerificationStatus.Invalid)
                {
                    _logger.LogInformation("Address ID {AddressId} got validated invalid: {Street}, {Zip}, {City}", address.Id, address.Street, address.Zip, address.City);
                    countInvalid++;
                    address.VerificationCode = (int?)AddressVerificationStatus.Invalid;
                }
                else if (result.Status == AddressVerificationStatus.Ambiguous)
                {
                    _logger.LogInformation("Address ID {AddressId} has an ambiguous result: {Street}, {Zip}, {City}", address.Id, address.Street, address.Zip, address.City);
                    countAmbiguous++;
                    address.VerificationCode = (int?)AddressVerificationStatus.Ambiguous;
                }
                else
                {
                    address.VerifiedSuccessfully = true;
                }

                // currently, we do not log OK or Corrected results
                //if (result.Status == AddressVerificationStatus.Corrected)
                //{
                //    _logger.LogInformation("ContactPoint ID {0} has a corrected result: {1}, {2}, {3} ", contactPoint.Id, contactPoint.Street, contactPoint.Zip, contactPoint.City);
                //    countCorrected++;
                //}
            }

            await _addressRepository.CommitChanges();
        }

        _logger.LogInformation("{ValidatedCount} addresses verified : {InvalidCount} invalid, {AmbiguousCount} ambiguous...", countValidations, countInvalid, countAmbiguous);
    }
}
