param
(
    [string]$config
)

[System.Reflection.Assembly]::LoadFile("C:\src\dataventure-io\etl-tool\src\etl\bin\debug\etl.lib.dll");

 $arg = [etl.lib.util.CommandLineParser]::parse($config)





