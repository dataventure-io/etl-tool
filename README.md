# ETL Tool #

A simple tool for extracting, transforming, and loading data using PowerShell and .NET.

The ETL tool:
* extracts from SQL Server, Excel, or CSV data sources.
* loads into SQL Server, Excel, or CSV targets
* uses PowerShell to script the execution
* can use PowerShell lambda functions to transform data


## Sample ETL ##

The example below loads data from an Excel data source, transforms the data using a 

```powershell 
param
(
    [string]$configFile
)

function Transform-DataTable
{
    param (
        [Parameter(Mandatory)]
        [ValidateScript({ $_.Ast.ParamBlock.Parameters.Count -eq 1 })]
        [Scriptblock] $Expression,
 
        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [System.Data.DataRow[]] $dataRows
    )
 
    $dataRows | % { &$Expression $_ }
}

$first_name_upper = 
    {   param($dataRow) 
        $first_name = ([string]$dataRow["first_name"]).ToUpper()
        Write-Host $first_name
        $dataRow["first_name"] = $first_name
    }


[System.Reflection.Assembly]::LoadFile("$($env:ETL_TOOL_HOME)\bin\Newtonsoft.Json.dll")
[System.Reflection.Assembly]::LoadFile("$($env:ETL_TOOL_HOME)\bin\Antlr4.Runtime.dll")
[System.Reflection.Assembly]::LoadFile("$($env:ETL_TOOL_HOME)\bin\etl.lib.dll")

$arg = [etl.lib.util.Arguments]::loadConfig($configFile)

$extractor = New-Object etl.lib.extractor.ExcelExtractor($arg)
$loader = New-Object etl.lib.loader.SqlServerLoader($arg)

$data = $extractor.extract()

Transform-DataTable $first_name_upper $data

$loader.load($data)
```

### Downloading and Binaries ###

The current release is

[Release 0.01](releases/etl-tool-0.01.zip )

To install:

* Unzip the release file to an installation folder
* Create an ETL_TOOL_HOME environment variable that references the installation folder
* Add %ETL_TOOL_HOME% to the PATH

### Command Line Usage ###

### Configuration File ###

Sample configuration file:

```json
{
	"ExcelExtractor": {

		"ExcelFile": "C:\\src\\dataventure-io\\etl-tool\\data\\names-500.xlsx",
		"SheetName": "us-500"
	},
	"SqlServerLoader": {
		"Server": "localhost",
		"Database": "EtlTest",
		"Table": "string_test"
	}
}
```

