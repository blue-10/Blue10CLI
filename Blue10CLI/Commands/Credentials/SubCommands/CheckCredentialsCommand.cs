using Blue10CLI.Services;
using Blue10SDK;
using Blue10SDK.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Blue10CLI.Commands.CredentialsCommands
{
    public class CheckCredentialsCommand : Command
    {
        private readonly CredentialsService _creds;
        private readonly IBlue10AsyncClient _blue10;
        private readonly ILogger<CheckCredentialsCommand> _logger;

        public CheckCredentialsCommand(CredentialsService creds, IBlue10AsyncClient blue10, ILogger<CheckCredentialsCommand> logger) : base("check",
            "checks if you can connect to blue10 ")
        {
            _creds = creds;
            _blue10 = blue10;
            _logger = logger;

            Add(new Option<string?>(new[] { "-q", "--query" }, () => null, "A query used to filter out results. NOTE: Dependant on output format. If output is 'json', this is a JMESPath query to filter results. https://jmespath.org/. If output is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp"));
            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, "Enter path to write output of this command to file. Default output is console only"));

            Handler = CommandHandler.Create<string, EFormatType, FileInfo?>(CheckConnection);
        }

        private async Task CheckConnection(string query, EFormatType format, FileInfo? output)
        {
            var apiKey = _creds.GetApiKey();
            try
            {
                //Check ApiKey
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogError(
                        "Please contact Blue10 to receive an API key for your account and use the 'credentials set'  command to update your credentials");
                    return;
                }
                //Get Me test
                var me = await _blue10.GetMeAsync();//.GetAwaiter().GetResult();
                await format.HandleOutput(me, output, query);
            }
            catch (Blue10ApiException apie) when (apie.Message.Contains("authentication required"))
            {
                _logger.LogError($"Your API : \"{apiKey}\" key is invalid, please contact Blue10 to receive a valid API Key and use the 'credentials set'  command to update your credentials");
            }
            catch (Blue10ApiException apie)
            {
                _logger.LogError(apie.Message);
            }
            catch (XPathException xpe)
            {
                _logger.LogError("Filter '{0}' is not a valid XPATH", query, xpe.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        private bool Ping(string url)
        {
            Console.WriteLine("Checking if there is an open internet connection to Blue10");
            var ping = new System.Net.NetworkInformation.Ping();
            var result = ping.Send(url);
            return result.Status == System.Net.NetworkInformation.IPStatus.Success;
        }
    }
}