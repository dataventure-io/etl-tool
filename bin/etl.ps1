param
(
    [string]$configFile
)


[System.Reflection.Assembly]::LoadFile("$($env:ETL_TOOL_HOME)\bin\Newtonsoft.Json.dll")
[System.Reflection.Assembly]::LoadFile("$($env:ETL_TOOL_HOME)\bin\Antlr4.Runtime.dll")
[System.Reflection.Assembly]::LoadFile("$($env:ETL_TOOL_HOME)\bin\etl.lib.dll")

$arg = [etl.lib.util.Arguments]::loadConfig($configFile)

$extractor = New-Object etl.lib.extractor.ExcelExtractor($arg)

$data = $extractor.extract()

$loader = New-Object etl.lib.loader.SqlServerLoader($arg)

$loader.load($data)



