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



