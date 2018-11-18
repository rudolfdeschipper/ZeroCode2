/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

grammar ZeroCode2;



zcDefinition : (parameters|genericModel)+ EOF
             ;

parameters : PTOKEN ID (pairs+=pair)*
           ;

genericModel : MTOKEN ID (singlemodel)*
			;

singlemodel : ID inherits? COLON obj;

inherits
	: (INHERITS ID )
	;

obj : LC pairs+=pair (COMMA pairs+=pair)* RC #ObjFull
	| LC RC #ObjEmpty
    ;

pair : modifier=('-'|'+')? ID inherits? COLON value;

value : STRING #ValueString
	| NUMBER #ValueNumber
    | obj #ValueObject
	| bt='true' #ValueTrue
	| bf='false' #ValueFalse
    ;

// ------- LEXER ----------

PTOKEN : '#'
         ;


MTOKEN : '@'
         ;

ID : [a-zA-Z][a-zA-Z_0-9.]*;

COLON : ':';

NUMBER : '0' |([1-9][0-9]*);

LC : '{';
RC : '}';
COMMA : ',';

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
