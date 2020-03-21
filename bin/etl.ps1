param
(
    [string]$configFile
)



[System.Reflection.Assembly]::LoadFile("C:\src\dataventure-io\etl-tool\bin\Newtonsoft.Json.dll")
[System.Reflection.Assembly]::LoadFile("C:\src\dataventure-io\etl-tool\bin\Antlr4.Runtime.dll")
[System.Reflection.Assembly]::LoadFile("C:\src\dataventure-io\etl-tool\src\etl\bin\debug\etl.lib.dll")

$arg = [etl.lib.util.Arguments]::loadConfig($configFile)


$extractor = New-Object [etl.lib.extractor.ExcelExtractor] ($arg)

$data = $extractor.extract()

$loader = New-Object [etl.lib.loader.SqlServerLoader]($arg)


