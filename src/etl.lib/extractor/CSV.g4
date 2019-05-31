grammar CSV;

csvFile: row+ ;

row : field (',' field)* '\r'? '\n' ;

field
    : TEXT
    | STRING
    |
    ;

TEXT   : ~[,\n\r"]+ ;
STRING : '"' ('""'|~'"')* '"' ; // quote-quote is an escaped quote
