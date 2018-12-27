/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

lexer grammar ZeroCode2Lexer;
// ------- LEXER ----------

PTOKEN : '#'
         ;


MTOKEN : '@'
         ;

TTRUE : 'true';

TFALSE : 'false';

ID : [a-zA-Z][a-zA-Z_0-9.]*;

COLON : ':';

NUMBER :  '0' |('-'?[1-9][0-9]*);

LC : '{';
RC : '}';
COMMA : ',';

PROPMODIFIER : ('-'|'+');

INHERITS
	: '<-'
	;

BLOCK_COMMENT
	: '/*' .*? '*/' -> channel(HIDDEN)
	;
LINE_COMMENT
	: '//' ~[\r\n]* -> channel(HIDDEN)
	;

STRING

   : '"' (ESC | SAFECODEPOINT)* '"'

   ;

fragment ESC

   : '\\' (["\\/bfnrt] | UNICODE)

   ;

fragment UNICODE

   : 'u' HEX HEX HEX HEX

   ;

fragment HEX

   : [0-9a-fA-F]

   ;

fragment SAFECODEPOINT

   : ~ ["\\\u0000-\u001F]

   ;
  

WS

   : [ \t\n\r\f] + -> skip

   ;

 
INCLUDE : '&' -> pushMode(FC);

mode FC;
NL: '\r'? '\n' -> more, popMode;
IGNORE: ~('\n'|'\r')+;

