using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using Blue10CLI.commands.credentials;
using Blue10CLI.services;
using Microsoft.Extensions.Logging;
using Administration = Blue10CLI.models.Administration;

namespace Blue10CLI.commands
{
    public class ListAdministrations : Command
    {
        private readonly AdministrationService _service;
        private readonly ILogger<ListAdministrations> _logger;

        public ListAdministrations(AdministrationService service,ILogger<ListAdministrations> logger) : base("list", "Lists all known Administrations (Companies) in a blue10 environment")
        {
            _service = service;
            _logger = logger;

            Add(new Option<string?>(new []{"-q","--query"}, () => null,"A query used to filter out results. NOTE: Dependant on output format. If output is 'json', this is a JMESPath query to filter results. https://jmespath.org/. If output is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp"));
            Add(new Option<EFormatType>(new []{"-f","--format"}, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new []{"-o","--output"}, () => null, "Enter path to write output of this command to file. Default output is console only"));
            Handler = CommandHandler.Create<string,EFormatType,FileInfo?>(ListAdministrationsHandler);
        }

        private async Task ListAdministrationsHandler( string query, EFormatType format, FileInfo? outputFile)
        {
            var resultObject = await _service.ListAdministrations();
            var administrations = resultObject.Select(company => 
                new Administration
                {
                    Id = company.Id,
                    AdministrationCode = company.AdministrationCode,
                    LoginStatus = company.LoginStatus,
                    AdministrationVatNumber = company.AdministrationVatNumber,
                    AdministrationCurrencyCode = company.AdministrationCurrencyCode
                })
                .ToList();
            try
            {
                await format.HandleOutput(administrations, outputFile, query);
            }
            catch (XPathException xpe)
            {
                _logger.LogError("Filter '{0}' is not a valid XPATH",query,xpe.Message);
            }
        }

    }
}