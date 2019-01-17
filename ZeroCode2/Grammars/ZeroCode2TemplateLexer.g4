/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

lexer grammar ZeroCode2TemplateLexer;
// ------- LEXER ----------

FILEC : '%FileCreate:' -> pushMode(FC);

FILEO : '%FileOverwrite:' -> pushMode(FC);

INCLUDE : '%Include:' -> pushMode(FC);

IF : '%If:' -> pushMode(IF_MODE);

ELSE : '%Else';

ENDIF : '%EndIf';

LOOP : '%Loop:' -> pushMode(FC);

ENDLOOP : '%/Loop' ~[\r\n]*;

ENDFILE : '%/File'  ~[\r\n]*;

INFO	: '%Info:' -> pushMode(FC);
DEBUG	: '%Debug:' -> pushMode(FC);
ERROR	: '%Error:' -> pushMode(FC);
LOG		: '%Log:' -> pushMode(FC);
TRACE	: '%Trace:' -> pushMode(FC);

DOT : '.';
EXCL: '!';
EQU: '=';

EXPRO : '=<' -> pushMode(EX);

ID : [a-zA-Z][a-zA-Z_0-9]*;

FILEPATH : [a-zA-Z_0-9\\]+;

WS     : [ \r\t\n]+;

TEXT : ~('%'|'=')+;

mode FC;
NL: '\r'? '\n' -> more, popMode;
IGNORE: ~('\n'|'\r')+;

mode EX;
EXPRC : '>' -> popMode;
EXIGNORE: ('#'|'@')? (ID DOT)* (ID | '$');

mode IF_MODE;
IF_WS     : [ \r\t\n]+ -> more, popMode;
IFTEXT : EXCL? ('#'|'@')? ID (DOT ID)* (EQU ID)?;