﻿using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class ListVendors : Command
    {
        private VendorService _vendorService;

        public ListVendors(VendorService vendorService) : base("list", "Lists all known vendors in environment")
        {
            
            _vendorService = vendorService;
            
            Add(new Option<string?>(new []{"-c","-a","--company","--administration"},() => null,  "The administration under which this vendor exists"){IsRequired = true});
            Add(new Option<string?>(new []{"-q","--query"}, () => null,"A query used to filter out results. NOTE: Dependant on output format. If output is 'json', this is a JMESPath query to filter results. https://jmespath.org/. If output is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp"));
            Add(new Option<EFormatType>(new []{"-f","--format"}, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new []{"-o","--output"}, () => null, "Enter path to write output of this command to file. Default output is console only"));

            Handler = CommandHandler.Create<string,string,EFormatType,FileInfo?>(ListVendorsHandler);
        }

        private async Task ListVendorsHandler(string? adminsitration, string? query, EFormatType format, FileInfo? outputFile)
        {
            var resultObject = await _vendorService.List(adminsitration);
            await format.HandleOutput(resultObject,outputFile,query);
        }
    }
}