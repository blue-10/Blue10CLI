# Intro

The Blue10 CLI is a windows command line interface implementation of the Blue10 SDK. And allows any process to connect to Blue10 through the Blue10 Rest API without having to implement either an HTTP client or a dotnet implementation of the Blue10 SDK. Simply call the Blue10 CLI over the commandline

> NOTICE: The current version of the Blue10 CLI inly provides limited functionality and is intended for preview purposes only. 


# Prerequisites

Before you can use the blue10 CLI check the following items

## 1. Dotnet 5 on windows
The Blue10 CLI is a windows executable that requires the DotNet 5 runtime. Before you can use the Blue10 CLI please make sure you are using a 64bit version of windows and have the DotNet 5 runtime installed. 

You can download the DotNet runtime at : https://dotnet.microsoft.com/download

## 2. Build from source

If you don't have a binary build yet, you could build the latest version of the Blue10 CLI.
To build the Blue10 SDK from source you require teh DotNet SDK, which can be downloaded from https://dotnet.microsoft.com/download

After you have downloaded the DotNet SDK, clone the source locally, using `git clone` or download  the source from github and unzip it in a local directory.

After you have the source locally, open a windows terminal and navigate to the directory holding `Blue10CLI.csproj` and run the command:

```
dotnet publish -r -c Release win-x64 -p:PublishSingleFile=true --self-contained true -o <OutputDirectory>
```

Replace `<OutputDirectory>` with the directory you want the binary to be built. You can ommit the `-o` paramater , then the binary can be found in the `.\bin\Release\net5.0-windows\win-x64\publish` directory


## 3. Acquire a Blue10 API Key

Before you can start useing the Blue10 API through the Blue10 CLI, you need a Blue10 Environment and an API Key.
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

Enter the API key you have provided and press enter 
```
PS C:\dev\Blue10CLI\Blue10CLI\bin\Debug\net5.0-windows\win-x64\publish> .\Blue10CLI.exe
Missing Blue10 API key, please insert here:
****************************************************************
````

This will enter your API key in windows secrets manager under the key `Blue10ApiKey`

To check if the api that you have entered is valid run the following command:

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

## The help option.

Run for following command 

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
  credentials       Show and set API credentials
  administration    Manage administration(companies)
```

The help option is an always available option that you can add to any command to see what the command does and how to manipulate it. Simply add `-h`, `-?` or `--help` at the end of a command to view all the sub-commands and options of any command.

This is the *root command* of the blue 10 cli. From here you can see that we have several sub-commands. 
We have already used the `credentials` sub-command to check if we have a good connection to a blue10 environment. 

The current version of the blue10 cli provides 3 aditional sub-commands: `administration`, `vendor` and `invoice`. These can be used to manage different parts of your blue10 environment.


## Administration 

Run for following command 

```
.\Blue10Cli.exe administration -h
```
You should see something like this:
```
administration:
  Manage administration(companies)

Usage:
  Blue10CLI administration [options] [command]

Options:
  -?, -h, --help    Show help and usage information

Commands:
  list    Lists all known Administrations (Companies) in a blue10 environment
```

The `administration` command currently has a single sub-command under it, that is the `list` sub-command. 

Run the following command:

```
.\Blue10CLI.exe administration list
```

The response should look something like this
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

Run for following command 

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
  -c, --code <code> (REQUIRED)           Unique Identifyer if Vendor in administration
  --country <country> (REQUIRED)         ISO 3166 two-letter country code of the Vendor's host country
  --currency <currency> (REQUIRED)       ISO 4217 three-letter currency code to set default currency for vendor
  --iban <iban> (REQUIRED)               list of IBANs associated with this vendor
  -l, --ledger <ledger>                  [default: Documents from this vendor will be routed to this ledger, leave
                                         empty to not associate]
  -p, --payment <payment>                [default: Documents from this vendor will be associated with this payment
                                         term, leave empty to not associate]
  -v, --vat <vat>                        [default: Documents from this vendor will be associated with this VAT code,
                                         leave empty to not associate]
  -b, --blocked                          Block vendor upon creation, default false [default: False]
  -f, --format <CSV|JSON|SSV|TSV|XML>    Output format. [default: JSON]
  -o, --output <output>                  Enter path to write output of this command to file. Default output is console
                                         only [default: ]
  -?, -h, --help                         Show help and usage information

```

You see that to create a vendor you need several pieces of vendor information. Options marked `(REQUIRED)` need to be enterred for the operation to be successfull. All other attributes are optional.

For example to create a new Vendor run the following command:

```bash
.\Blue10 CLI vendor create -a B10Api -c KPN32 --country NL --currency EUR --iban NL45RABO6143537119 --ledger 00005 --vat 12341234
```

Will create a vendor named `KPN32` in administration B10Api with the provided attributes


## Invoice Command

Run for following command 

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
  pull    Pull invoices to be posted
```


the `peek` command retreives a sumamry of all open invoices that are ready to be posted.

the `pull` command retreives the full invoice information and writes it to file together with the original pdf assigned to that file to a given directory.


# Advanced

## Format Option

The Blue10 CLI provides output by default in JSON, this can be alterred with the '-f' option. 

With this option you can pass an alternate format. The supported formats are:
- json
- xml
- csv
- ssv
- tsv

For example, the command:
```
.\Blue10CLI.exe administration list -f xml
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

The Blue10 CLI writes output to console, but you can also use the '-o' option to tell the CLI to write it's output to a fiven output file 

For example, the command:
```
.\Blue10CLI.exe administration list -f xml - o Administrations.xml
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

And will write the results to `Administrations.xml` as a file
This can be done with all supported formats


## The Query option

If you use the xml or json output format, you can add a `-q` option to filter the results with a query.

If you use the `xml` format, you can use valid *xpath* strings to filter the results of the xml output
.\Blue10CLI.exe administration list -f xml - o Administrations.xml -q 

If you use the `json` format you can use valid *jmespath* queries to filter the results of the json output

In both cases the results will be written to file if the `-o` option is used