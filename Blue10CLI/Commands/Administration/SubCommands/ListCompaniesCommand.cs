﻿using Blue10CLI.Enums;
using Blue10CLI.Services;
using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Blue10CLI.Commands.AdministrationCommands
{
    public class ListCompaniesCommand : Command
    {
        private readonly CompanyService _service;
        private readonly IInOutService _utilities;
        private readonly ILogger<ListCompaniesCommand> _logger;

        public ListCompaniesCommand(
            CompanyService service,
            IInOutService utilities,
            ILogger<ListCompaniesCommand> logger) :
            base("list", "Lists all known Administrations (Companies) in a Blue10 environment")
        {
            _service = service;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<string?>(new[] { "-q", "--query" }, () => null, "A query used to filter out results. NOTE: Dependant on output format. If output is 'json', this is a JMESPath query to filter results. https://jmespath.org/. If output is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp"));
            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, "Enter path to write output of this command to file. Default output is console only"));
            Handler = CommandHandler.Create<string, EFormatType, FileInfo?>(ListAdministrationsHandler);
        }

        private async Task ListAdministrationsHandler(string query, EFormatType format, FileInfo? output)
        {
            var resultObject = await _service.ListCompanies();
            try
            {
                await _utilities.HandleOutput(format, resultObject, output, query);
            }
            catch (ArgumentOutOfRangeException e)
            {
                _logger.LogError($"{format} is not supported for this action: {e.Message}");
            }
            catch (XPathException xpe)
            {
                _logger.LogError("Filter '{0}' is not a valid XPATH", query, xpe.Message);
            }

        }
    }
}