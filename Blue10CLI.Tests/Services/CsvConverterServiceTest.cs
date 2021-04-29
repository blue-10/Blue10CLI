using Blue10CLI.Services.Converters;
using Blue10SDK.Models;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoNSubstitute.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Blue10CLI.Tests.Services
{
    public class CsvConverterServiceTest
    {
        // All files contain the same data.
        // 3 items
        public readonly List<Vendor> vendorResults = new List<Vendor>
        {
            new Vendor()
            {
                Name = "KPN",
                CountryCode = "NL",
                Iban = new List<string>() { "NL96ABNA2753394563", "NL23ABNA2137951150" },
                CurrencyCode = "EUR",
                Blocked = false,

                Id = Guid.Parse("0611e62c-ea19-4ec4-924b-acd200bfe25d"),
                AdministrationCode = "B10Api",
                IdCompany = "B10Api",
            },
            new Vendor()
            {
                Name = "MKB Brandstof",
                VatNumber = "NL1814948030B01",
                CountryCode = "NL",
                CurrencyCode = "EUR",
                Blocked = false,

                Id = Guid.Parse("23099de6-19bc-4aed-b6eb-a6d700be8cea"),
                AdministrationCode = "10030",
                IdCompany = "B10Api",
            },
            new Vendor()
            {
                Name = "19383639237923087342",
                CountryCode = "NL",
                Iban = new List<string>() { "NL96ABNA2753394563" },
                CurrencyCode = "EUR",
                Blocked = false,

                Id = Guid.Parse("42b1db44-90b4-490e-8ba2-ad0200e54f20"),
                AdministrationCode = "19383639237923087342",
                IdCompany = "B10Api",
            }
        };


        [Theory]
        [InlineAutoMockData(",", "Vendor", "TestFiles/listVendors.csv")] // CSV
        [InlineAutoMockData("\t", "Vendor", "TestFiles/listVendors.tsv")] // TSV
        [InlineAutoMockData(";", "Vendor", "TestFiles/listVendors.scsv")] // SCSV
        public void GetRecords_Success_Full_info<T>(
            string pSeperator,
            string pType,
            string pSyncFilePath,
            CsvConverterService pCsvConverterService) where T : BaseObject
        {
            // Setup Data
            var fFile = File.ReadAllText(pSyncFilePath);

            // Test
            var result = pCsvConverterService.GetRecords<Vendor>(fFile, pSeperator);

            // Validate
            result.Should().BeOfType<List<Vendor>>();
            result.Count.Should().Be(3);
            result[0].Should().BeEquivalentTo(vendorResults[0]);
            result[1].Should().BeEquivalentTo(vendorResults[1]);
            result[2].Should().BeEquivalentTo(vendorResults[2]);
        }

        [Theory]
        [AutoMockData]
        public async Task GetRecords_Success_Part_info(
            CsvConverterService pCsvConverterService)
        {
            // csv tsv scsv
            // Vendor, Glaccount, vatcode
        }

        [Theory]
        [AutoMockData]
        public async Task GetRecords_Failed_Unkown_parameter(
            CsvConverterService pCsvConverterService)
        {
            // csv tsv scsv
            // Vendor, Glaccount, vatcode
        }

        [Theory]
        [AutoMockData]
        public async Task GetRecords_Failed_File_not_correct(
            CsvConverterService pCsvConverterService)
        {
            // csv tsv scsv
        }

        [Theory]
        [AutoMockData]
        public async Task GetRecords_Failed_Chosen_wrong_format(
            CsvConverterService pCsvConverterService)
        {
            // csv tsv scsv
        }

        [Theory]
        [AutoMockData]
        public async Task GetRecords_Failed_Wrong_paramter_type(
            CsvConverterService pCsvConverterService)
        {
            // csv tsv scsv
        }
    }
}
