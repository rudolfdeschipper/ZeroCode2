/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

parser grammar ZeroCode2;

options { tokenVocab=ZeroCode2Lexer; }


zcDefinition : (parameters|genericModel|include)+ EOF
             ;

parameters : PTOKEN ID (pairs+=pair)*
           ;

genericModel : MTOKEN ID (smodels+=singlemodel)*
			;

include		: INCLUDE (IGNORE)+ #IncludeStatement
			;

singlemodel : ID inherits? COLON obj;

inherits
	: (INHERITS ID )
	;

obj : LC pairs+=pair (COMMA pairs+=pair)* orderstatement? RC #ObjFull
	| LC orderstatement? RC #ObjEmpty
    ;

pair	: modifier=PROPADD? ID pairvalue
		| modifier=PROPREMOVE ID
		;

pairvalue	: inherits? COLON value
			| inherits
			;

value : STRING #ValueString
	| NUMBER #ValueNumber
    | obj #ValueObject
	| TTRUE #ValueTrue
	| TFALSE #ValueFalse
    ;

orderstatement : ORDERING orderby+=ID (COMMA orderby+=ID)*
	;
