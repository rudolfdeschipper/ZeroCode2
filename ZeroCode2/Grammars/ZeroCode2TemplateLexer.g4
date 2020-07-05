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

EXPRO : '=<' -> pushMode(EX);

ID : [a-zA-Z][a-zA-Z_0-9]*;

PATHPART: ('#'|'@')? (ID DOT)* (ID | '$');

FILEPATH : [a-zA-Z_0-9\\]+;

WS     : [ \r\t\n]+;

TEXT : ~('%'|'=')+;

DOT : '.';
EXCL: '!';
EQU: '=';
QUEST: '?';

REFSTART: '[';
REFEND: ']';

mode FC;
NL: '\r'? '\n' -> more, popMode;
IGNORE: ~('\n'|'\r')+;

mode EX;
EXPRC : '>' -> popMode;
EXIGNORE: ('#'|'@')? ((ID | REFSTART PATHPART REFEND) DOT)* ((ID | REFSTART PATHPART REFEND) | '$');

mode IF_MODE;
IFTEXT : EXCL? ('#'|'@')? (ID DOT)* (ID | '$') ((EQU ID) | QUEST)?;
IF_WS     : [ \r\t\n]+ -> popMode;
