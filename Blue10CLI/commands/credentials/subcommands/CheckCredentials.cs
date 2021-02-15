using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Blue10CLI.services;
using Blue10SDK;
using Blue10SDK.Exceptions;
using Microsoft.Extensions.Logging;

namespace Blue10CLI.commands.credentials
{
    public class CheckCredentials : Command
    {
        private readonly CredentialsService _creds;
        private readonly IBlue10AsyncClient _blue10;
        private readonly ILogger<CheckCredentials> _logger;

        public CheckCredentials(CredentialsService creds, IBlue10AsyncClient blue10,ILogger<CheckCredentials> logger) : base("check",
            "checks if you can connect to blue10 ")
        {
            _creds = creds;
            _blue10 = blue10;
            _logger = logger;
            Handler = CommandHandler.Create(CheckConnection);
        }

        private void CheckConnection()
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
                var me = _blue10.GetMeAsync().GetAwaiter().GetResult();
                if (me != null)
                {
                    _logger.LogInformation($"Successfully connected to {me?.EnvironmentName!}");
                }
            }
            catch (Blue10ApiException apie) when (apie.Message.Contains("authentication required"))
            {
                _logger.LogError($"Your API : \"{apiKey}\" key is invalid, ple" + $"ase contact Blue10 to receive a valid API Key and use the 'credentials set'  command to update your credentials");
            }
            catch (Blue10ApiException apie)
            {
                _logger.LogError(apie.Message);
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