using FluentValidation;
using IbanNet;
using IbanNet.FluentValidation;
using System.Linq;

namespace Blue10CLI.Validators
{
    public class VendorValidator : AbstractValidator<Blue10SDK.Models.Vendor>
    {
        public VendorValidator(IIbanValidator ibanValidator)
        {
            //Todo finish validator
            RuleFor(x => x.Blocked).NotEmpty();
            RuleFor(x => x.Iban).NotEmpty().ForEach(x => x.Iban(ibanValidator));
            RuleFor(x => x.Name).NotNull().NotEmpty();

            RuleFor(x => x.CountryCode).Must(x => Region.Codes.Contains(x.ToUpper()));
            RuleFor(x => x.CurrencyCode).Must(x => Region.Currencies.Contains(x.ToUpper()));

            RuleFor(x => x.VatNumber);
            RuleFor(x => x.VendorCustomerCode);

            RuleFor(x => x.DefaultLedgerCode);
            RuleFor(x => x.DefaultVatCode);
            RuleFor(x => x.DefaultPaymentTermCode);
            RuleFor(x => x.DefaultVatScenarioCode);
            RuleFor(x => x.DefaultVatScenarioCode);
        }

        private bool BeAValidPostcode(string postcode)
        {
            // custom postcode validating logic goes here
            return true;
        }
    }
}

