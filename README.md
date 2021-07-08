# Intro

The Blue10 CLI is a windows command line interface implementation of the Blue10 SDK. And allows any process to connect to Blue10 through the Blue10 Rest API without having to implement either an HTTP client or a dotnet implementation of the Blue10 SDK. Simply call the Blue10 CLI over the commandline

> NOTICE: The current version of the Blue10 CLI inly provides limited functionality and is intended for preview purposes only. 


# Prerequisites

Before you can use the blue10 CLI check the following items

## 1. Dotnet Core 3.1 on windows
The Blue10 CLI is a windows executable that requires the DotNet Core 3.1 runtime. Before you can use the Blue10 CLI please make sure you are using a 64bit version of windows and have the DotNet Core 3.1 runtime installed. 

You can download the DotNet runtime at : https://dotnet.microsoft.com/download

## 2. Build from source

If you don't have a binary build yet, you could build the latest version of the Blue10 CLI.
To build the Blue10 SDK from source you require teh DotNet SDK, which can be downloaded from https://dotnet.microsoft.com/download

After you have downloaded the DotNet SDK, clone the source locally, using `git clone` or download the source from github and unzip it in a local directory.

After you have the source locally, open a windows terminal and navigate to the directory holding `Blue10CLI.csproj` and run the command:

```
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true -o <OutputDirectory>
```

Replace `<OutputDirectory>` with the directory you want the binary to be built. You can ommit the `-o` paramater , then the binary can be found in the `.\bin\Release\netcoreapp3.1-windows\win-x64\publish` directory

## 3. (Alternative for 1 and 2) Download the build

In the releases is also the zip `Blue10CLI.zip` in which an build is made for win-x64. There are two versions, the `cli` is a console application and the `headless` is a windows application. For more information on the difference see [Headless and Console modes](#Headless-and-Console-modes:)

Here you can find the latest release: https://github.com/blue-10/Blue10CLI/releases

## 4. Acquire a Blue10 API Key

Before you can start using the Blue10 API through the Blue10 CLI, you need a Blue10 Environment and an API Key.
If you don't yet have a blue10 environment or api key for your environment, please contact blue10 support at https://support.blue10.com/?lang=en

# Setting up the CLI

Once you have acquired the binaries to the Blue10 CLI or have built them from source. You can now open a terminal and navigate to `Blue10CLI.exe`. Run the following command:

```
.\Blue10CLI.exe
```

If this is your first time running the CLI you will be prompted to enter your API key:

```
PS C:\Home\Blue10CLI\bin\Debug\net5.0-windows\win-x64\publish> .\Blue10CLI.exe
Missing Blue10 API key, please insert here:

```

Enter the API key you have provided and press enter.

```
PS C:\dev\Blue10CLI\Blue10CLI\bin\Debug\net5.0-windows\win-x64\publish> .\Blue10CLI.exe
Missing Blue10 API key, please insert here:
****************************************************************
````

This will enter your API key in the AppConfiguration.json file. 
_Note that if the file doesn't exist it will be created automatically._

To check if the API key that you have entered is valid run the following command:

`.\Blue10CLI.exe credentials check`

And you should see something along the lines of:

```json
[
  {
    "EnvironmentName": "<Your environment>"
  }
]
```

Otherise contact Blue10 support and request a new API key and update your api key with the following command:

```
.\Blue10CLI.exe credentials set
```

# Using the CLI

## Headless and Console modes:

The Blue10CLI provides two variants of the CLI:
- `Blue10CLI.exe`
- `Blue10CLIHeadless.exe`

They are two compilations of the same source, but the difference being that `Blue10CLI.exe` is built as a console application. This means that all self-documenting commands can be called manually through the command line and studied before implementing the headless variant. Developers can use the `Blue10CLI.exe` to test out and debug different commands.

The `Blue10CLIHeadless.exe` variant is built as a windows application, this means that it can still be called through the system input or command line, but it will not open a terminal (or head) by itself. Developers should use `Blue10CLIHeadless.exe` when using a Blue10 as part of a script or external application to avoid console flicker.

## The help option.

Run following command 

```
.\Blue10Cli.exe -h
```

You should see something like this:

```
Usage:
  Blue10CLI [options] [command]

Options:
  --debug           Run command in debug mode to view detailed logs
  --version         Show version information
  -?, -h, --help    Show help and usage information

Commands:
  vendor            creates lists and manages vendors in the environments
  invoice           creates lists and manages invoices
  glaccount         creates lists and manages GLAccounts in the environments
  vatcode           creates lists and manages VatCodes in the environments
  credentials       Show and set API credentials
  company           Manage companies
```

The help option is an always available option that you can add to any command to see what the command does and how to manipulate it. Simply add `-h`, `-?` or `--help` at the end of a command to view all the sub-commands and options of any command.

The above command is the *root command* of the Blue10 CLI. From here you can see that we have several sub-commands. 
We have already used the `credentials` sub-command to check if we have a good connection to a blue10 environment. 

The current version of the Blue10 CLI provides 5 aditional sub-commands: `company`, `vendor`, `invoice`, `glaccount` and `vatcode`. These can be used to manage different parts of your Blue10 environment.


## Company 

_**NOTE:** From version v0.0.10 this is change to `company`, the older version named it `administration`_

Run following command 

```
.\Blue10Cli.exe company -h
```

You should see something like this:

```
company:
  Manage companies

Usage:
  Blue10CLI company [options] [command]

Options:
  -?, -h, --help    Show help and usage information

Commands:
  list    Lists all known Companies in your Blue10 environment
```

The `company` command currently has a single sub-command under it, that is the `list` sub-command. 

Run the following command:

```
.\Blue10CLI.exe company list
```

The response should look something like this:

```
[
  {
    "Id": "B10Api",
    "AdministrationCode": "B10Apiii",
    "LoginStatus": "login_ok",
    "AdministrationVatNumber": "NL823833598B01",
    "AdministrationCurrencyCode": "EUR"
  },
  {
    "Id": "B11Api",
    "AdministrationCode": "B11Api",
    "LoginStatus": "login_ok",
    "AdministrationVatNumber": "NL806633135B01",
    "AdministrationCurrencyCode": "EUR"
  },
  {
    "Id": "B12Api",
    "AdministrationCode": "B12Api",
    "LoginStatus": "login_ok",
    "AdministrationVatNumber": "",
    "AdministrationCurrencyCode": "EUR"
  },
  {
    "Id": "FeatureSetTest",
    "AdministrationCode": "FeatureSetTest",
    "LoginStatus": "unknown",
    "AdministrationVatNumber": "",
    "AdministrationCurrencyCode": "EUR"
  },
  {
    "Id": "FeatureSetTest2",
    "AdministrationCode": "FeatureSetTest2",
    "LoginStatus": "unknown",
    "AdministrationVatNumber": "",
    "AdministrationCurrencyCode": "EUR"
  }
]
```

## Vendor command

Run following command 

```
.\Blue10Cli.exe vendor -h
```

You should see something like this:

```
vendor:
  creates lists and manages vendors in the environments

Usage:
  Blue10CLI vendor [options] [command]

Options:
  -?, -h, --help    Show help and usage information

Commands:
  create    Creates new vendor in the system
  list      Lists all known vendors in environment
  sync      Sync vendors from a file to the Blue10 environment. AdministrationCode and CompanyId is required for each vendor.
            Updating existing vendors requires Id. Creating new vendors requires empty Id.
```

Using the vendor command you can create or list all vendors in a current environment.

To see how to create a new vendor run the following command:

```
.\Blue10Cli.exe vendor create -h
```

You should see something like this:

```
create:
  Creates new vendor in the system

Usage:
  Blue10CLI vendor create [options]

Options:
  -c, --company-id <company-id> (REQUIRED)                    The company identifyer under which this vendor will be
                                                              created
  -a, --administration-code <administration-code>             Unique identifyer of Vendor used in ERP
  (REQUIRED)
  --country <country> (REQUIRED)                              ISO 3166 two-letter country code of the Vendor's host
                                                              country
  --currency <currency> (REQUIRED)                            ISO 4217 three-letter currency code to set default
                                                              currency for vendor
  --iban <iban> (REQUIRED)                                    list of IBANs associated with this vendor
  -n, --name <name>                                           Name of the vendor. Default value will be the
                                                              administration-code
  -l, --ledger <ledger>                                       Documents from this vendor will be routed to this
                                                              ledger, leave empty to not associate [default: ]
  -p, --payment <payment>                                     Documents from this vendor will be associated with this
                                                              payment term, leave empty to not associate [default: ]
  -v, --vat <vat>                                             Documents from this vendor will be associated with this
                                                              VAT code, leave empty to not associate [default: ]
  -f, --format <CSV|JSON|SCSV|TSV|XML>                        Output format. [default: JSON]
  -o, --output <output>                                       Enter path to write output of this command to file.
                                                              Default output is console only [default: ]
  -?, -h, --help                                              Show help and usage information
```

You see that to create a vendor you need several pieces of vendor information. Options marked `(REQUIRED)` need to be enterred for the operation to be successfull. All other attributes are optional.

For example to create a new Vendor run the following command:

```bash
.\Blue10 CLI vendor create -c B10Api -a KPN31 --country NL --currency EUR --iban NL45RABO6143537119 -n KPN-NL --ledger 00005 --vat 12341234
```

Will create a vendor named `KPN-NL` with the AdministrationCode `KPN31` in company B10Api with the provided attributes. 

_**NOTE**: AdministrationCode is the unique identifier of the ERP. This code will also be used as name if the name is not given._

The IBANs has a special display in the CSV, SCSV or TSV format. We are working with a list/array of IBANs and to seperate them we use `|`. An Csv example would be:

```
...,[NL96ABNA2753394563|NL23ABNA2137951150],...
```

The list starts with a '[' sign and ends with a ']' sign. In here we see two IBAN numbers; `NL96ABNA2753394563` and `NL23ABNA2137951150`, seperated by the `|` sign.

## Invoice Command

Run following command 

```
.\Blue10Cli.exe invoice -h
```
You should see something like this:

```
invoice:
  creates lists and manages invoices

Usage:
  Blue10CLI invoice [options] [command]

Options:
  -?, -h, --help    Show help and usage information

Commands:
  peek    Peek invoices to be posted
  pull    Pull all invoices to be posted
  sign    Sign-off invoice with a ledger entry number
```

### Peek

The `peek` command retrieves a sumamry of all open invoices that are ready to be posted.

Run following command 

```
.\Blue10CLI.exe invoice peek -h
```

```
peek:
  Peek invoices to be posted

Usage:
  Blue10CLI invoice peek [options]

Options:
  -q, --query <query>                    A query used to filter out results. NOTE: Dependant on output format. If output is
                                         'json', this is a JMESPath query to filter results. https://jmespath.org/. If output
                                         is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp
                                         [default: ]
  -f, --format <CSV|JSON|SSV|TSV|XML>    Output format. [default: JSON]
  -o, --output <output>                  Enter path to write output of this command to file. Default output is console only
  -?, -h, --help                         Show help and usage information
```


**NOTE:** *`invoice peek` is a **read** transaction: After 'peeking' the list of available invoices, the invoices in blue10 remain 'waiting for ERP'. This means that the handshake with the ERP is not complete*

### Pull

The `pull` command retrieves the full invoice information and writes it to file together with the original pdf assigned to that file to a given directory.

Run following command:

```
.\Blue10CLI.exe invoice pull -h
```
```
pull:
  Pull all invoices to be posted

Usage:
  Blue10CLI invoice pull [options]

Options:
  -q, --query <query>                    Aquery used to filter out results. NOTE: Dependant on output format. If output is
                                         'json', this is a JMESPath query to filter results. https://jmespath.org/. If output
                                         is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp
                                         [default: ]
  -f, --format <CSV|JSON|SSV|TSV|XML>    Output format. [default: JSON]
  -o, --output <output>                  Enter path to write output of this command to the filesystem. Default output will
                                         create an 'invoices' directory in the root of the console [default: ./invoices/]
  -?, -h, --help                         Show help and usage information
```


`Pulling` the available invoices will create a file in the chosen format passed through the `-f` command into the chosen *output directory* passed through the `-o` command. The file will be named with the pattern: `<invoiceid>.<format extension>`. In the same output directory  alongside the text file, the CLI will download and save, the original PDF document associated with the invoice and name it with the naming pattern `<invoiceid>.pdf`

**NOTE:** *`invoice pull` is a **read** transaction: After 'pulling' the list of available invoices, the invoices in blue10 remain 'waiting for ERP'. This means that the handshake with the ERP is not complete*

An example command would look as follows:

```
.\Blue10CLI.exe invoice pull -o InvoicePull -f JSON 
```

The above command pulls all invoices ready to be posted to the directory *InvoicePull*:

The result would be as follows:

```
C:/dev/
│   Blue10CLI.exe
|   ...
│
└───InvoicePull
      ├ 12345a6b-78c9-01de-fg23-hi4j567k89lm.json
      └ 12345a6b-78c9-01de-fg23-hi4j567k89lm.pdf
```

### Sign

The `sign` command signs off a invoice with the given ledger entry number.

Run following command:

```
.\Blue10CLI.exe invoice sign -h
```
You should see something like this:

```
sign:
  Sign-off invoice with a ledger entry number

Usage:
  Blue10CLI invoice sign [options]

Options:
  -i, --invoice-id <invoice-id> (REQUIRED)                  The Id of the invoice to be signed off
  -c, --ledger-entry-code <ledger-entry-code> (REQUIRED)    The ledger entry code assigned to the invoice by the ERP system
  -f, --format <CSV|JSON|SSV|TSV|XML>                       Output format. [default: JSON]
  -o, --output <output>                                     Enter path to write output of this command to file. Default
                                                            output is console only [default: ]
  -?, -h, --help                                            Show help and usage information
```

An example command would look as follows:

```
.\Blue10CLI.exe invoice sign -i 12345a6b-78c9-01de-fg23-hi4j567k89lm -c 10000 -f JSON -o SignResult.json
```

**NOTE:** *`invoice sign` is an **update** transaction: After 'signing' an invoice, the document in is no longer 'waiting for ERP' and is assigned a Ledger Entry number passed through by the `-c` option. This completes the invoice posting handshake*

## GLAccount command

Run following command 

```
.\Blue10Cli.exe glaccount -h
```

You should see something like this:

```
glaccount:
  creates lists and manages GLAccounts in the environments

Usage:
  Blue10CLI glaccount [options] [command]

Options:
  -?, -h, --help    Show help and usage information

Commands:
  list    Lists all known GLAccounts in a company
  sync    Sync GLAccounts from a file to the Blue10 environment. AdministrationCode and CompanyId is required for each
          GLAccount. Updating existing GLAccounts requires Id. Creating new GLAccounts requires empty Id.
```

Using the GlAccount command you can list all accounts in a current environment or sync the GLAccounts from a file.

To see how to get a list of GLAccounts run the following command:

```
.\Blue10Cli.exe glaccount list -h
```

You should see something like this:

```
list:
  Lists all known GLAccounts in a company

Usage:
  Blue10CLI glaccount list [options]

Options:
  -c, --company-id <company-id> (REQUIRED)    The company identifier under which the GLAccounts exists [default: ]
  -q, --query <query>                         Aquery used to filter out results. NOTE: Dependant on output format. If
                                              output is 'json', this is a JMESPath query to filter results.
                                              https://jmespath.org/. If output is 'xml', this is an XPATH string.
                                              https://www.w3schools.com/xml/xpath_intro.asp [default: ]
  -f, --format <CSV|JSON|SCSV|TSV|XML>        Format of the output file. (JSON, XML...) [default: JSON]
  -o, --output <output>                       Enter path to write output of this command to file. Default output is
                                              console only. [default: ]
  -?, -h, --help                              Show help and usage information
```

You see that to list a set of GLAcocounts you need to include an company. Company is marked `(REQUIRED)` and is needed for the operation to be successfull. All other attributes are optional.

For example to get a list of GLAccounts from an specific company, run the following command: 

```bash
.\Blue10CLI.exe glaccount list -c B10Api -o GLAccountList.json -f JSON
```

Will create a list of GLAccounts from the company B10Api. GLAccountList.json file be created with the results. (This you can use for the `sync` subcommand)

## VatCode command

Run following command 

```
.\Blue10Cli.exe vatcode -h
```

You should see something like this:

```
vatcode:
  creates lists and manages VatCodes in the environments

Usage:
  Blue10CLI vatcode [options] [command]

Options:
  -?, -h, --help    Show help and usage information

Commands:
  list    Lists all known VatCodes in company
  sync    Sync VatCodes from a file to the Blue10 environment. AdministrationCode and CompanyId is required for each VatCode.
          Updating existing VatCodes requires Id. Creating new VatCodes requires empty Id.
```

Using the VatCode command you can list all accounts in a current environment or sync teh VatCode from a file.

To see how to get a list of VatCode run the following command:

```
.\Blue10Cli.exe vatcode list -h
```

You should see something like this:

```
list:
  Lists all known VatCodes in a company

Usage:
  Blue10CLI vatcode list [options]

Options:
  -c, --company-id <company-id> (REQUIRED)    The company identifier under which the VatCodes exists [default: ]
  -q, --query <query>                         Aquery used to filter out results. NOTE: Dependant on output format. If
                                              output is 'json', this is a JMESPath query to filter results.
                                              https://jmespath.org/. If output is 'xml', this is an XPATH string.
                                              https://www.w3schools.com/xml/xpath_intro.asp [default: ]
  -f, --format <CSV|JSON|SCSV|TSV|XML>        Format of the output file. (JSON, XML...) [default: JSON]
  -o, --output <output>                       Enter path to write output of this command to file. Default output is
                                              console only. [default: ]
  -?, -h, --help                              Show help and usage information
```

You see that to list a set of VatCodes you need to include an company. Company is marked `(REQUIRED)` and is needed for the operation to be successfull. All other attributes are optional.

For example to get a list of VatCodes from an specific company, run the following command: 

```bash
.\Blue10CLI.exe vatcode list -c B10Api -o VatCodelist.json -f JSON
```

Will create a list of VatCodes from the company B10Api. VatCodelist.json file be created with the results. (This you can use for the `sync` subcommand)

# Advanced

## Format Option

The Blue10 CLI provides output by default in JSON, this can be alterred with the '-f' option. 

With this option you can pass an alternate format. The supported formats are:
- json
- xml
- csv
- scsv
- tsv

For example, the command:

```
.\Blue10CLI.exe company list -f xml
```

Returns this result:

```xml
<?xml version="1.0" encoding="utf-16"?>
<ArrayOfCompany xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Company>
    <Id>B10Api</Id>
    <AdministrationCode>B10Apiii</AdministrationCode>
    <LoginStatus>login_ok</LoginStatus>
    <AdministrationVatNumber>NL823833598B01</AdministrationVatNumber>
    <AdministrationCurrencyCode>EUR</AdministrationCurrencyCode>
  </Company>
  <Company>
    <Id>B11Api</Id>
    <AdministrationCode>B11Api</AdministrationCode>
    <LoginStatus>login_ok</LoginStatus>
    <AdministrationVatNumber>NL806633135B01</AdministrationVatNumber>
    <AdministrationCurrencyCode>EUR</AdministrationCurrencyCode>
  </Company>
  <Company>
    <Id>B12Api</Id>
    <AdministrationCode>B12Api</AdministrationCode>
    <LoginStatus>login_ok</LoginStatus>
    <AdministrationVatNumber />
    <AdministrationCurrencyCode>EUR</AdministrationCurrencyCode>
  </Company>
  <Company>
    <Id>FeatureSetTest</Id>
    <AdministrationCode>FeatureSetTest</AdministrationCode>
    <LoginStatus>unknown</LoginStatus>
    <AdministrationVatNumber />
    <AdministrationCurrencyCode>EUR</AdministrationCurrencyCode>
  </Company>
  <Company>
    <Id>FeatureSetTest2</Id>
    <AdministrationCode>FeatureSetTest2</AdministrationCode>
    <LoginStatus>unknown</LoginStatus>
    <AdministrationVatNumber />
    <AdministrationCurrencyCode>EUR</AdministrationCurrencyCode>
  </Company>
</ArrayOfCompany>
```

## Write output to file

The Blue10 CLI writes output to console, but you can also use the '-o' option to tell the CLI to write it's output to a given output file.

For example, the command:

```
.\Blue10CLI.exe company list -f xml - o Companies.xml
```

Returns this result:

```xml
<?xml version="1.0" encoding="utf-16"?>
<ArrayOfCompany xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Company>
    <Id>B10Api</Id>
    <AdministrationCode>B10Apiii</AdministrationCode>
    <LoginStatus>login_ok</LoginStatus>
    <AdministrationVatNumber>NL823833598B01</AdministrationVatNumber>
    <AdministrationCurrencyCode>EUR</AdministrationCurrencyCode>
  </Company>
  <Company>
    <Id>B11Api</Id>
    <AdministrationCode>B11Api</AdministrationCode>
    <LoginStatus>login_ok</LoginStatus>
    <AdministrationVatNumber>NL806633135B01</AdministrationVatNumber>
    <AdministrationCurrencyCode>EUR</AdministrationCurrencyCode>
  </Company>
  <Company>
    <Id>B12Api</Id>
    <AdministrationCode>B12Api</AdministrationCode>
    <LoginStatus>login_ok</LoginStatus>
    <AdministrationVatNumber />
    <AdministrationCurrencyCode>EUR</AdministrationCurrencyCode>
  </Company>
  <Company>
    <Id>FeatureSetTest</Id>
    <AdministrationCode>FeatureSetTest</AdministrationCode>
    <LoginStatus>unknown</LoginStatus>
    <AdministrationVatNumber />
    <AdministrationCurrencyCode>EUR</AdministrationCurrencyCode>
  </Company>
  <Company>
    <Id>FeatureSetTest2</Id>
    <AdministrationCode>FeatureSetTest2</AdministrationCode>
    <LoginStatus>unknown</LoginStatus>
    <AdministrationVatNumber />
    <AdministrationCurrencyCode>EUR</AdministrationCurrencyCode>
  </Company>
</ArrayOfCompany>
```

And will write the results to `Companies.xml` as a file.
This can be done with all supported formats.

## The Query option

If you use the xml or json output format, you can add a `-q` option to filter the results with a query.

If you use the `xml` format, you can use valid *xpath* strings to filter the results of the xml output.

```
.\Blue10CLI.exe company list -f xml - o Companies.xml -q <xpath query>
```

If you use the `json` format you can use valid *jmespath* queries to filter the results of the json output.

In both cases the results will be written to file if the `-o` option is used.

### XPath example

Pull invoices from specific company. (Replace *TheIdCompany*)
```
.\Blue10CLI.exe invoice pull -f xml -q "/ArrayOfPurchaseInvoice/PurchaseInvoice[IdCompany='TheIdCompany']"
```

Get vatcode list with specific AdministrationCode. (Replace *TheIdCompany* and *TheAdministrationCode*)
```
.\Blue10CLI.exe vatcode list -c TheIdCompany -f xml -q "/ArrayOfVatCode/VatCode[AdministrationCode='TheAdministrationCode']"
```

Get vendor  list with specific CurrencyCode. (Replace *TheIdCompany* and *TheCurrencyCode*)
```
.\Blue10CLI.exe vendor list -c TheIdCompany -f xml -q "/ArrayOfVatCode/VatCode[CurrencyCode='TheCurrencyCode']"
```