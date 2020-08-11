/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
lexer grammar ZeroCode2TemplateLexer;

FILEOVERWRITE: '%FileOverwrite:' ~(' '|'\r'|'\n')+ -> pushMode(LINE)
        ;

FILECREATE: '%FileCreate:' ~(' '|'\r'|'\n')+ -> pushMode(LINE)
        ;

INCLUDE: '%Include:' ~(' '|'\r'|'\n')+ -> pushMode(LINE)
       ;

ENDFILE: '%/File' -> pushMode(LINE)
         ;

LOOP: '%Loop:' (('#'|'@')? ((ID | REFSTART PATHPART REFEND) DOT)* ((ID | REFSTART PATHPART REFEND) | '$')) (' ' | ('\r'? '\n'))
    ;

ENDLOOP: 
       '%/Loop:' ~(' '|'\r'|'\n')* (' ' | ('\r'? '\n'))
       | '%/Loop'
       ;

IF: '%If:' (EXCL? ('#'|'@')? (ID DOT)* (ID | '$') ((EQU ID) | QUEST)?) (' ' | ('\r'? '\n'))
  ;

ELSE: '%Else' (' ' | ('\r'? '\n'))
    ;

ENDIF: '%EndIf' (' ' | ('\r'? '\n'))
     ;

EXPR: '=<' (('#'|'@')? ((ID | REFSTART PATHPART REFEND) DOT)* ((ID | REFSTART PATHPART REFEND) | '$')) '>'
    ;
        
INFO	: '%Info:' ~('\r'|'\n')+ -> pushMode(LINE);
DEBUG	: '%Debug:' ~('\r'|'\n')+ -> pushMode(LINE);
ERROR	: '%Error:' ~('\r'|'\n')+ -> pushMode(LINE);
LOG	    : '%Log:' ~('\r'|'\n')+ -> pushMode(LINE);
TRACE	: '%Trace:' ~('\r'|'\n')+ -> pushMode(LINE);

EQU: '=';
PERC: '%';

TEXT: ~[%=]+
    ;

fragment ID : [a-zA-Z][a-zA-Z_0-9]*;

fragment PATHPART: ('#'|'@')? (ID DOT)* (ID | '$');

fragment DOT : '.';

fragment EXCL: '!';

fragment QUEST: '?';

fragment REFSTART: '[';

fragment REFEND: ']';

mode LINE;
NEWLINE: ('\r'? '\n') -> popMode
       ;
